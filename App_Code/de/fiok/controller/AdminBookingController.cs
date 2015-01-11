namespace de.fiok.controller
{
  using System;
  using System.Collections;
  using de.fiok.service;
  using de.fiok.core;
  using de.fiok.form;
  using de.fiok.state;
  using de.fiok.type;
  using log4net;
  using System.Web;

  /// <summary>
  /// Controller für die Administration von Buchungen und Reservierungen.
  /// </summary>
  /// <remarks>
  /// created by - Steffen Förster  
  /// </remarks>
  public class AdminBookingController : BKBaseController
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(AdminBookingController));
    private static readonly String ADMIN_STATE = "admin.state";
    
    private static readonly HouseService houseService = HouseService.GetInstance ();
    private static readonly BookingService bookingService = BookingService.GetInstance ();

    /// <summary>
    /// Default contructor.
    /// </summary>
    public AdminBookingController ()
    {
      log.Debug ("AdminBookingController.init");
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

    #region Navigation

    /// <summary>
    /// Ruft eine neue Seite auf.
    /// </summary>
    public void NavigateToTarget(String target)
    {
      log.Debug("AdminBookingController.NavigateToTarget");

      NavProvider.Instance.NavigateToTarget(target);
    }
    
    #endregion

    #region Methoden für die View 'adminBooking'

    /// <summary>
    /// Das AdminBookingForm wird für die erste Anzeige vorbereitet. 
    /// </summary>
    public void PrepareForm (AdminBookingForm form, int landlordId)
    {
      log.Debug ("AdminBookingController.PrepareForm");
      
      // Haus-Auswahl initialisieren 
      KeyValue[] houses = houseService.RetrieveHousesByLandlord (landlordId);
      form.SetHouseList (houses);
      
      // erstes Haus laden
      if (houses.Length > 0) {
        HouseBean house = houseService.RetrieveHouse (Int32.Parse (houses[0].Key));
        form.House = house;
        Container.AdminBookingData.House = house;
        form.SetBookingList (new ArrayList ());
        
        // Datum-Filter setzen
        DateTime lastMonth = DateTime.Now.AddMonths (-1);
        form.DateFrom = new DateTime (lastMonth.Year, lastMonth.Month, 1);
        
        // nur Buchungen mit dem Status 'reserviert' und 'gebucht' zeigen
        int[] status = new int[] {(int)BookingStatus.RESERVED, (int)BookingStatus.BOOKED};
        form.SelectedBookingStatus = status;
        
        // Buchungen laden
        LoadBookings (form, house);
      }
    }
    
    /// <summary>
    /// Das AdminHouseForm wird für die Anzeige eines neuen Hauses vorbereitet.
    /// </summary>
    public void ChangeHouse (AdminBookingForm form, int newHouseId)
    {
      log.Debug ("AdminBookingController.ChangeHouse");
      
      HouseBean house = houseService.RetrieveHouse (newHouseId);
      form.House = house;
      Container.AdminBookingData.House = house;
    }
    
    /// <summary>
    /// Sucht Buchungen anhand von Such-Kriterien.
    /// </summary>
    public int SearchBookings (AdminBookingForm form)
    {
      log.Debug ("AdminBookingController.SearchBookings");
      
      HouseBean house = Container.AdminBookingData.House;
      return LoadBookings (form, house);
    }

    /// <summary>
    /// Berechnet den Buchungspreis.
    /// </summary>
    public MessageResult CalculatePrice (BookingItem item)
    {
      log.Debug ("AdminBookingController.CalculatePrice");
      
      return bookingService.CalculatePrice (item.Arrival, item.Departure, item.HouseID);
    }
    
    /// <summary>
    /// Speichert die geänderte Buchung.
    /// </summary>
    public MessageResult UpdateBooking (BookingItem item)
    {
      log.Debug ("AdminBookingController.UpdateBooking");
      
      return bookingService.PerformUpdateBooking (item);
    }

    /// <summary>
    /// Erstellt eine neue Buchung.
    /// </summary>
    public MessageResult InsertBooking (BookingItem item)
    {
      log.Debug ("AdminBookingController.InsertBooking");
      
      return bookingService.PerformInsertBooking (item);
    }
    
    /// <summary>
    /// Setzt die geladenen Buchungen des aktuellen Hauses im Form. 
    /// </summary>
    public void SetBookings (AdminBookingForm form)
    {
      log.Debug ("AdminBookingController.SetPrices");
      
      form.SetBookingList (Container.AdminBookingData.BookingList);
    }
    
    /// <summary>
    /// Löscht die geladenen Buchungen. 
    /// </summary>
    public void ResetBookings (AdminBookingForm form)
    {
      log.Debug ("AdminBookingController.ResetBookings");
      
      Container.AdminBookingData.BookingList = new ArrayList ();
      form.SetBookingList (Container.AdminBookingData.BookingList);
    }
    
    #endregion
    
    #region private methods
    
    private int LoadBookings (AdminBookingForm form, HouseBean house)
    {
      log.Debug ("AdminBookingController.LoadBookings");
      
      IList bookings = bookingService.RetrieveBookings (
        house.ID, form.DateFrom, form.DateTo, form.SelectedBookingStatus);
      form.SetBookingList (bookings);
      Container.AdminBookingData.BookingList = bookings;
      
      return bookings.Count;
    }
  
    #endregion
  }
}
