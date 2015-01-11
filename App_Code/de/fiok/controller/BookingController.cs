// TODO beim Zurücksprung aus der Seite confirmation.aspx die Mitteilung speichern

namespace de.fiok.controller
{
  using System;
  using System.Collections.Generic;
  using de.fiok.service;
  using de.fiok.core;
  using de.fiok.form;
  using de.fiok.state;
  using log4net;
  using System.Web;

  /// <summary>
  /// Controller für die Buchung eines Ferienhauses.
  /// </summary>
  /// <remarks>
  /// created by - Steffen Förster  
  /// </remarks>
  public class BookingController : BKBaseController
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(BookingController));
    private static readonly String BOOKING_STATE = "booking.state";

    private static readonly BookingService bService = BookingService.GetInstance ();
    private static readonly HouseService hService = HouseService.GetInstance ();

    /// <summary>
    /// Default contructor.
    /// </summary>
    public BookingController ()
    {
      log.Debug ("BookingController.init");
    }
    
    private BookingStateContainer Container
    {
      get {
        BookingStateContainer container = (BookingStateContainer)State[BOOKING_STATE];
        if (container == null) {
          container = new BookingStateContainer ();
          State[BOOKING_STATE] = container;
        }
        return container;
      }
    }

    /// <summary>
    /// Alle Daten aus dem Container löschen. Dies ist nötig nach erfolgreichem Abschluss der Buchung.
    /// </summary>
    private void CleanContainerData()
    {
      State[BOOKING_STATE] = new BookingStateContainer();
    }

    #region Navigations-Code

    /// <summary>
    /// Ruft eine neue Seite auf.
    /// </summary>
    public void NavigateToTarget(String target)
    {
      log.Debug("BookingController.NavigateToTarget");

      NavProvider.Instance.NavigateToTarget(target);
    }

    #endregion

    #region Methoden für die View 'startBooking'

    /// <summary>
    /// ID des aktuellen Hauses setzen.
    /// </summary>
    public void SetCurrentHouseId (int houseId)
    {
      log.Debug ("BookingController.SetCurrentHouseId");

      Container.HouseBean = hService.RetrieveHouse (houseId); ;
    }

    /// <summary>
    /// ID des aktuellen Hauses liefern.
    /// </summary>
    public int GetCurrentHouseId ()
    {
      log.Debug ("BookingController.GetCurrentHouseId");

      return (Container.HouseBean == null) ? 0 : Container.HouseBean.ID;
    }

    public HouseBean GetCurrentHouseBean ()
    {
      log.Debug ("BookingController.GetHouseBean");

      return Container.HouseBean;
    }

    /// <summary>
    /// Das BookingForm wird für die erste Anzeige vorbereitet. Sind Daten im State vorhanden,
    /// so wird das Form mit diesen Daten initialisiert.
    /// </summary>
    /// <returns>true, wenn vorhandene Daten geladen wurden</returns>
    public bool PreparePeriodSelection (BookingForm form)
    {
      log.Debug ("BookingController.PreparePeriodSelection");

      if (Container.BookingData != null) {
        form.ArrivalDate = Container.BookingData.ArrivalDate;
        form.DepartureDate = Container.BookingData.DepartureDate;
        form.Price = Container.BookingData.Price;
        form.EnableNextButton = true;
        return true;
      }
      else {
        return false;
      }
    }
    
    /// <summary>
    /// Ausgewählten Zeitraum validieren und Mietpreis berechnen.
    /// </summary>
    public bool CalculatePrice (BookingForm form)
    {
      log.Debug ("BookingController.CalculatePrice");
    
      ValidationResult valid = bService.ValidatePeriod (form.ArrivalDate, form.DepartureDate, Container.HouseBean.ID);
      
      if (valid.Valid) {
        MessageResult calculateResult = bService.CalculatePrice (form.ArrivalDate, form.DepartureDate, Container.HouseBean.ID);
        
        if (calculateResult.Result) {
          form.Price = (PriceBean)calculateResult.Value;
          form.EnableNextButton = true;
          return true;
        }
        else {
          form.Price = PriceBean.EMPTY_PRICE;
          form.EnableNextButton = false;
          form.ErrorMessage = calculateResult.Message;
          return false;
        }
      }
      else {
        form.Price = PriceBean.EMPTY_PRICE;
        form.EnableNextButton = false;
        form.ErrorMessage = valid.Message;
        return false;
      }
    }

    /// <summary>
    /// Liefert die Belegungsmonate als Liste zurück. Es sind die Belegungsmonate für den übergebenen Monat 
    /// sowie für den vorherigen und nachfolgenden Monat enthalten.
    /// </summary>
    public IList<MonthOccupancy> GetMonthOccupancyList (int year, int month)
    {
      log.Debug ("BookingController.GetMonthOccupancyList");

      IList<MonthOccupancy> result = new List<MonthOccupancy> ();
      DateTime now = new DateTime (year, month, 1);
      DateTime previous = now.AddMonths (-1);
      DateTime next = now.AddMonths (1);

      int houseId = (Container.HouseBean == null) ? 0 : Container.HouseBean.ID;

      result.Add (bService.CreateMonthOccupancy (houseId, previous.Year, previous.Month));
      result.Add (bService.CreateMonthOccupancy (houseId, now.Year, now.Month));
      result.Add (bService.CreateMonthOccupancy (houseId, next.Year, next.Month));

      return result;
    }
    
    /// <summary>
    /// Startet die Eingabe der persönlichen Daten, des zukünftigen Mieters und
    /// speichert den ausgewählten Zeitraum sowie den berechneten Preis im State.
    /// </summary>
    public void FinishPeriodSelection (BookingForm form)
    {
      log.Debug ("BookingController.FinishPeriodSelection");
    
      bool valid = CalculatePrice (form);
      if (valid) {
        // ausgewählten Zeitraum und den Mietpreis im State speichern
        Container.BookingData = new BookingData (form.ArrivalDate, form.DepartureDate, form.Price);
    
        // Navigation zur nächsten Seite
        NavigateToTarget (NavProvider.PAGE_TENANT);
      }
    }
    
    #endregion
    
    #region Methoden für die View 'tenantEntry'
    
    /// <summary>
    /// Das TenantEntryForm wird für die erste Anzeige vorbereitet. Sind Daten im State vorhanden,
    /// so wird das Form mit diesen Daten initialisiert.
    /// </summary>
    public void PrepareTenantEntry (TenantEntryForm form)
    {
      log.Debug ("BookingController.PrepareTenantEntry");
      
      if (Container.TenantData != null) {
        form.SetData (Container.TenantData);
      }
    }
    
    /// <summary>
    /// Springt zurück zur Auswahl der An- und Abreise.
    /// </summary>
    public void BackToPeriodSelection (TenantEntryForm form)
    {
      log.Debug ("BookingController.BackToPeriodSelection");
    
      Container.TenantData = form.GetData ();
      
      // Navigation zur voherigen Seite
      NavigateToTarget (NavProvider.PAGE_BOOKING);
    }
    
    /// <summary>
    /// Beendet die Eingabe der Daten des Mieters und startet den Bestätigungs-Dialog.
    /// </summary>
    public void FinishTenantEntry (TenantEntryForm form)
    {
      log.Debug ("BookingController.FinishTenantEntry");
    
      // Speichern der Daten des Mieters im State
      Container.TenantData = form.GetData ();
      
      // Navigation zur nächsten Seite
      NavigateToTarget(NavProvider.PAGE_CONFIRM);
    }
    
    #endregion
    
    #region Methoden für die View 'confirmation'
    
    /// <summary>
    /// Das ConfirmationForm wird für die erste Anzeige vorbereitet. Die Daten aus dem State 
    /// werden im Form gesetzt.
    /// </summary>
    public void PrepareConfirmation (ConfirmationForm form)
    {
      log.Debug ("BookingController.PrepareConfirmation");
      
      form.BookingData = Container.BookingData;
      form.TenantData = Container.TenantData;
    }
    
    /// <summary>
    /// Springt zurück Eingabe der Daten des Mieters.
    /// </summary>
    public void BackToTenantEntry (ConfirmationForm form)
    {
      log.Debug ("BookingController.BackToTenantEntry");
      
      // Navigation zur vorherigen Seite
      NavigateToTarget(NavProvider.PAGE_TENANT);
    }
    
    /// <summary>
    /// Reservierung wird ausgeführt. Nach der erfolgreichen Reservierung werden die Session-Daten gelöscht.
    /// </summary>
    public MessageResult FinishBooking (ConfirmationForm form)
    {
      log.Debug ("BookingController.FinishBooking");
      
      TenantEntryData tenantData = Container.TenantData;
      form.GetData (tenantData);
      
      MessageResult result = bService.PerformBooking (Container.BookingData, tenantData);
      if (result.Result)
      {
        CleanContainerData();
        NavProvider.Instance.CompleteNavigation(); // keine Navigation über Browser mehr möglich
      }

      return result;
    }
    
    #endregion
  }
}
