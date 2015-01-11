namespace de.fiok.controller
{
  using System;
  using System.Collections;
  using de.fiok.service;
  using de.fiok.core;
  using de.fiok.form;
  using de.fiok.state;
  using log4net;
  using System.Web;

    /// <summary>
    /// Controller f�r die Pflege der Daten eines Ferienhauses.
    /// </summary>
    /// <remarks>
	  /// created by - Steffen F�rster  
	  /// </remarks>
    public class AdminHouseController : BKBaseController
    {
      private static readonly ILog log = LogManager.GetLogger(typeof(AdminHouseController));
      private static readonly String ADMIN_STATE = "admin.state";
      
      private static readonly HouseService houseService = HouseService.GetInstance ();

      /// <summary>
      /// Default contructor.
      /// </summary>
      public AdminHouseController ()
      {
        log.Debug ("AdminHouseController.init");
      }
      
      private AdminStateContainer Container
      {
        get {
          AdminStateContainer container = (AdminStateContainer)State[ADMIN_STATE];
          if (container == null) {
            container = new AdminStateContainer ();
            State[ADMIN_STATE] = container;
          }
          return container;
        }
      }

      #region Navigations-Code

      /// <summary>
      /// Ruft eine neue Seite auf.
      /// </summary>
      public void NavigateToTarget(String target)
      {
        log.Debug("AdminHouseController.NavigateToTarget");

        NavProvider.Instance.NavigateToTarget(target);
      }

      #endregion

      #region Methoden f�r die View 'adminHouse'

      /// <summary>
      /// Das AdminHouseForm wird f�r die erste Anzeige vorbereitet. 
      /// </summary>
      public void PrepareForm (AdminHouseForm form, int landlordId)
      {
        log.Debug ("AdminHouseController.PrepareForm");
        
        // Haus-Auswahl initialisieren 
        KeyValue[] houses = houseService.RetrieveHousesByLandlord (landlordId);
        form.SetHouseList (houses);
        
        // erstes Haus laden
        if (houses.Length > 0) {
          HouseBean house = houseService.RetrieveHouse (Int32.Parse (houses[0].Key));
          form.House = house;
          Container.AdminHouseData.House = house;
          
          // Preise f�r das aktuelle Jahr laden
          LoadPrices (form, house, DateTime.Now.Year);
        }
      }
      
      /// <summary>
      /// Das AdminHouseForm wird f�r die Anzeige eines neuen Hauses vorbereitet.
      /// </summary>
      public void ChangeHouse (AdminHouseForm form, int newHouseId)
      {
        log.Debug ("AdminHouseController.ChangeHouse");
        
        HouseBean house = houseService.RetrieveHouse (newHouseId);
        form.House = house;
        Container.AdminHouseData.House = house;
        
        // Preise neu laden
        int year = form.SelectedYear;
        LoadPrices (form, house, year);
      }
      
      /// <summary>
      /// Speichert die �nderungen zu einem Ferienhaus.
      /// </summary>
      public void UpdateHouseData (AdminHouseForm form)
      {
        log.Debug ("AdminHouseController.UpdateHouseData");
        
        // Hausdaten speichern
        HouseBean currentHouse = Container.AdminHouseData.House;
        form.GetHouseData (currentHouse);
        houseService.UpdateHouse (currentHouse);
        
        // Preise speichern
        IList prices = Container.AdminHouseData.HousePriceList;
        houseService.UpdateHousePrices (
          currentHouse, 
          Container.AdminHouseData.HousePriceList, 
          Container.AdminHouseData.RemovedPriceList
        );
        
        // Preise neu laden
        int year = form.SelectedYear;
        LoadPrices (form, currentHouse, year);
      }
      
      /// <summary>
      /// Setzt die geladenen Preise des aktuellen Hauses im Form. 
      /// </summary>
      public void SetPrices (AdminHouseForm form)
      {
        log.Debug ("AdminHouseController.SetPrices");
        
        // Preise im Form setzen
        IList priceList = Container.AdminHouseData.HousePriceList;
        form.SetPriceList (priceList);  
      }
      
      /// <summary>
      /// Es werden die Preise f�r ein anderes Jahr geladen.
      /// </summary>
      public void ChangePrices (AdminHouseForm form)
      {
        log.Debug ("AdminHouseController.ChangePrices");
        
        // Preise neu laden
        HouseBean currentHouse = Container.AdminHouseData.House;
        int year = form.SelectedYear;
        LoadPrices (form, currentHouse, year);
      }
      
      /// <summary>
      /// Pr�ft, ob es zwischen den Preis-Intervallen eines Jahres �berschneidungen gibt,
      /// wenn ein bestehenden Intervall ge�ndert wird. Weiterhin wird gepr�ft, ob die Zeitbereichsgrenzen
      /// innerhalb des �bergebenen Jahres liegen.
      /// Es muss au�erdem mindestens ein Abreise- und Anreise-Tag ausgew�hlt werden.
      /// </summary>
      public MessageResult ValidateChangedPriceInterval (HousePriceInterval newInterval, int index, int year) 
      {
        log.Debug ("AdminHouseController.ValidatePriceInterval");
        
        MessageResult result = new MessageResult (true);
        
        // Pr�fung des Jahres
        if (newInterval.Start.Year != year || newInterval.End.Year != year) {
          result = new MessageResult(false, AppResources.GetMessage("Validation_invalid.interval.year", year));
        }
        else if (newInterval.ArrivalDays.IsEmpty || newInterval.DepartureDays.IsEmpty) {
          result = new MessageResult(false, AppResources.GetMessage("Validation_required.least.arrival.departure.day"));
        }
        else {
          IList prices = Container.AdminHouseData.HousePriceList;
          int i = 0;
          foreach (HousePriceInterval interval in prices) {
            // keine Pr�fung mit dem urspr�nglichen Intervall
            if (index != i) {
              if (interval.Start <= newInterval.Start && interval.End >= newInterval.Start ||
                  interval.Start <= newInterval.End && interval.End >= newInterval.End) {
                    result = new MessageResult(false, AppResources.GetMessage("Validation_invalid.interval"));
                break;
              }
            }
            i++;
          }
        }
        
        return result;
      }
      
      /// <summary>
      /// Pr�ft, ob alle Tage des �bergebenen Jahres in den Preis-Intervallen liegen.
      /// </summary>
      public bool IsAllIntervalsContainsAllDays (int year) 
      {
        log.Debug ("AdminHouseController.IsAllIntervalsContainsAllDays");
        
        bool result = true;
        
        IList prices = Container.AdminHouseData.HousePriceList;
        if (prices != null && prices.Count > 0) {
          DateTime date = new DateTime (year, 1, 1);
        
          while (date.Year == year) {
            bool contains = false;
            foreach (HousePriceInterval interval in prices) {
              if (interval.Contains (date)) {
                contains = true;
              }
            }
            
            if (! contains) {
              result = false;
              break;
            }
          
            date = date.AddDays (1);
          }
        }
       
        return result;
      }
      
      /// <summary>
      /// Entfernt ein Preis-Intervall aus der aktuellen Liste der Preise und f�gt diesen in eine
      /// gesonderte Liste ein, deren Eintr�ge dann beim Speichern der �nderungen auch in der 
      /// Datenbank gel�scht werden.
      /// </summary>
      public void RemovePriceInterval (AdminHouseForm form, HousePriceInterval price)
      {
        log.Debug ("AdminHouseController.RemovePriceInterval");
        
        Container.AdminHouseData.HousePriceList.Remove (price);
        Container.AdminHouseData.RemovedPriceList.Add (price);
      }
      
      #endregion
      
      #region private methods
      
      /// <summary>
      /// Preise f�r ein Jahr laden, im Form und im Container speichern.
      /// </summary>
      private void LoadPrices (AdminHouseForm form, HouseBean house, int year)
      {
        log.Debug ("AdminHouseController.LoadPrices");
      
        // Preise zum Haus laden
        IList prices = houseService.RetrieveHousePrices (house.ID, year);
        form.SetPriceList (prices);
        Container.AdminHouseData.HousePriceList = prices;
        Container.AdminHouseData.RemovedPriceList = new ArrayList ();
      }
      
      #endregion
    }
}
