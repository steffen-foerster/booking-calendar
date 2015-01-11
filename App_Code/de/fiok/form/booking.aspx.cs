namespace de.fiok.form
{
  using System;
  using System.Threading;
  using System.Drawing;
  using System.Collections.Generic;
  using System.Web.UI;
  using System.Web.UI.HtmlControls;
  using System.Web.UI.WebControls;
  using System.Collections;
  using de.fiok.service; 
  using de.fiok.core;
  using de.fiok.state;
  using de.fiok.controller;
  using log4net;

  /// <summary>
  /// Code-Behind Klasse für die Seite booking.aspx, die der Auswahl des Buchungszeitraumes dient.
  /// </summary>
  /// <remarks>
	/// created by - Steffen Förster
	/// </remarks>
  public class BookingForm : BaseForm
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(BookingForm));

    private static BookingService bService = BookingService.GetInstance ();

    #region Properties

    // ASP-Page Variablen
    protected Calendar arrivalCalendar;
    protected Calendar departureCalendar;
    protected Label message;
    protected Label errorMessage;
    protected Label arrivalDate;
    protected Label departureDate;
    protected HtmlButton btnNextPage;
    protected HtmlButton btnCancel;
    protected LinkButton btnHomePage;
    protected PriceBean price = PriceBean.EMPTY_PRICE;
    protected DropDownList arrivalMonthList;

    // private variables
    private IList<MonthOccupancy> arrivalOccupancy;
    private IList<MonthOccupancy> departureOccupancy;

    #endregion

    #region Initialize Page

    override protected void OnInit (EventArgs e)
    {
      log.Debug ("BookingForm.OnInit");
      
      InitializeComponent();
      base.OnInit(e);
    }

    /// <summary>
    /// Komponenten initialisieren.
    /// </summary>
    private void InitializeComponent ()
    {
      log.Debug ("BookingForm.InitializeComponent");
    
	    this.btnNextPage.ServerClick += new System.EventHandler (this.NextPage);
      this.btnCancel.ServerClick += new System.EventHandler (this.HomePage);

      // Kalender Events
	    this.arrivalCalendar.SelectionChanged += new System.EventHandler (this.ArrivalDayChanged);
	    this.arrivalCalendar.VisibleMonthChanged += new MonthChangedEventHandler (this.ArrivalMonthChanged);
      this.arrivalCalendar.DayRender += new DayRenderEventHandler (this.ArrivalDayRender);
	    this.departureCalendar.SelectionChanged += new System.EventHandler (this.DepartureDayChanged);
	    this.departureCalendar.VisibleMonthChanged += new MonthChangedEventHandler (this.DepartureMonthChanged);
      this.departureCalendar.DayRender += new DayRenderEventHandler (this.DepartureDayRender);

      // DropDownList Events
      this.arrivalMonthList.SelectedIndexChanged += new System.EventHandler (this.ArrivalMonthListChanged);  

	    this.Load += new System.EventHandler (this.PageLoad);
	    this.PreRender += new System.EventHandler (this.PreRenderHandler);
    }
	  
    #endregion
	  
    #region Eventhandler

    /// <summary>
    /// Selektierten Zeitraum aus dem State holen, wenn die Seite zum ersten Mal geladen wird.
    /// Weiterhin werden die Daten an die Controls gebunden.
    /// </summary>
    private void PageLoad (Object sender, EventArgs e)
    {
      log.Debug ("BookingForm.PageLoad");

      if (! IsPostBack) {
        // HouseId im State speichern
        try {
          BookingController.SetCurrentHouseId (Int32.Parse ((String)Session["houseId"]));
        }
        catch (Exception ex) {
          log.Error ("cannot set houseId - houseId: " + Session["houseId"], ex);
        }
        
        bool dataLoaded = BookingController.PreparePeriodSelection (this);

        if (! dataLoaded) {
          arrivalCalendar.VisibleDate = DateTime.Now;
          departureCalendar.VisibleDate = DateTime.Now;
          arrivalDate.Text = AppResources.GetMessage("Booking_msg.entry.arrival");
          departureDate.Text = AppResources.GetMessage("Booking_msg.entry.departure");
        }
        
        FillMonthSelect (arrivalMonthList, (dataLoaded ? arrivalCalendar.SelectedDate : DateTime.Now));
      }
    }
    
    /// <summary>
    /// Daten werden an die Controls gebunden.
    /// </summary>
    private void PreRenderHandler (Object sender, EventArgs e)
    {
      log.Debug ("BookingForm.PreRenderHandler");
      
      // Belegungsmonate laden
      LoadMonthOccupancyList ();

      this.DataBind ();
    }
    
    /// <summary>
    /// Monat des Anreise-Kalenders wurde verändert.
    /// Monat des Abreise-Kalenders soll gleich dem Monat des Anreise-Kalenders sein.
    /// </summary>
    private void ArrivalMonthChanged (Object sender, MonthChangedEventArgs e)
    {
      log.Debug ("BookingForm.ArrivalMonthChanged");
      
      DateTime visibleDate = new DateTime (
        arrivalCalendar.VisibleDate.Year,
        arrivalCalendar.VisibleDate.Month,
        1);

      if (visibleDate < DateTime.Now) {
        arrivalCalendar.VisibleDate = DateTime.Now;
        departureCalendar.VisibleDate = DateTime.Now;
      }
      else {
        departureCalendar.VisibleDate = arrivalCalendar.VisibleDate;
      }
      
      CalculatePrice ();

      // neuen Monat in DropDown-Liste selektieren
      arrivalMonthList.SelectedValue = arrivalCalendar.VisibleDate.ToString ("yyyy_MM");
    }
    
    /// <summary>
    /// Monat des Abreise-Kalenders wurde verändert.
    /// Monat des Anreise-Kalenders soll gleich dem Monat des Abreise-Kalenders sein.
    /// </summary>
    private void DepartureMonthChanged (Object sender, MonthChangedEventArgs e)
    {
      log.Debug ("BookingForm.DepartureMonthChanged");

      DateTime visibleDate = new DateTime (
        departureCalendar.VisibleDate.Year,
        departureCalendar.VisibleDate.Month,
        1);

      if (visibleDate < DateTime.Now) {
        arrivalCalendar.VisibleDate = DateTime.Now;
        departureCalendar.VisibleDate = DateTime.Now;
      }
      else {
        arrivalCalendar.VisibleDate = departureCalendar.VisibleDate;
      }
      
      CalculatePrice ();

      // neuen Monat in DropDown-Liste selektieren
      arrivalMonthList.SelectedValue = arrivalCalendar.VisibleDate.ToString ("yyyy_MM");
    }

    /// <summary>
    /// Anreise wurde gewählt -> Validierung und Preis berechnen.
    /// </summary>
    private void ArrivalDayChanged (Object sender, EventArgs e)
    {
      log.Debug ("BookingForm.ArrivalDayChanged");
      
      arrivalDate.Text = arrivalCalendar.SelectedDate.ToShortDateString();
      CalculatePrice ();    
    }

    /// <summary>
    /// Abreise wurde gewählt -> Validierung und Preis berechnen.
    /// </summary>
    private void DepartureDayChanged (Object sender, EventArgs e)
    {
      log.Debug ("BookingForm.DepartureDayChanged");
    
      departureDate.Text = departureCalendar.SelectedDate.ToShortDateString();
      CalculatePrice ();
    }

    /// <summary>
    /// Rendern der Tage beinflussen.
    /// </summary>
    private void ArrivalDayRender (Object sender, DayRenderEventArgs e)
    {
      DateTime compareDate = new DateTime (e.Day.Date.Year, e.Day.Date.Month, e.Day.Date.Day);

      // Sperren von reservierten Zeiträumen.
      foreach (MonthOccupancy month in arrivalOccupancy) {
        BookingDay bookingDay = month.GetBookingDay (compareDate);
        if (bookingDay != null && 
            (! bookingDay.Arrival && ! bookingDay.Departure ||
               bookingDay.Arrival && bookingDay.Departure)) {
          e.Day.IsSelectable = false;
          e.Cell.BackColor = Color.Empty;
          e.Cell.ForeColor = Color.Empty;
          e.Cell.CssClass = "calendar_blocked_day";
        }
      }

      // bereits gewählter Tag ist nicht mehr auswählbar -> kein Postback
      if (e.Day.Date.Equals (arrivalCalendar.SelectedDate)) {
        e.Day.IsSelectable = false;
      }
    }

    /// <summary>
    /// Rendern der Tage beinflussen.
    /// </summary>
    private void DepartureDayRender (Object sender, DayRenderEventArgs e)
    {
      DateTime compareDate = new DateTime (e.Day.Date.Year, e.Day.Date.Month, e.Day.Date.Day);

      // Sperren von reservierten Zeiträumen.
      foreach (MonthOccupancy month in departureOccupancy) {
        BookingDay bookingDay = month.GetBookingDay (compareDate);
        if (bookingDay != null && 
           (! bookingDay.Arrival && ! bookingDay.Departure ||
              bookingDay.Arrival && bookingDay.Departure)) {
          e.Day.IsSelectable = false;
          e.Cell.BackColor = Color.Empty;
          e.Cell.ForeColor = Color.Empty;
          e.Cell.CssClass = "calendar_blocked_day";
        }
      }

      // bereits gewählter Tag ist nicht mehr auswählbar -> kein Postback
      if (e.Day.Date.Equals (departureCalendar.SelectedDate)) {
        e.Day.IsSelectable = false;
      }
    }

    /// <summary>
    /// Neuer Monat wurde ausgewählt -> Kalendermonate ändern.
    /// </summary>
    private void ArrivalMonthListChanged (Object sender, EventArgs e)
    {
      log.Debug ("BookingForm.ArrivalMonthListChanged");

      String[] tokens = arrivalMonthList.SelectedValue.Split ('_');

      DateTime visibleDate = new DateTime (
        Int32.Parse (tokens[0]), Int32.Parse (tokens[1]), 1
      );

      arrivalCalendar.VisibleDate = visibleDate;
      departureCalendar.VisibleDate = visibleDate;

      CalculatePrice ();
    }

    /// <summary>
    /// Läd die Belegungskalender für die Anreise- und Abreisekalender.
    /// </summary>
    private void LoadMonthOccupancyList ()
    {
      arrivalOccupancy = 
        BookingController.GetMonthOccupancyList (arrivalCalendar.VisibleDate.Year, arrivalCalendar.VisibleDate.Month);
      departureOccupancy = 
        BookingController.GetMonthOccupancyList (departureCalendar.VisibleDate.Year, departureCalendar.VisibleDate.Month);
    }

    
    /// <summary>
    /// Validierung und Preis berechnen.
    /// </summary>
    private void CalculatePrice ()
    {
      if (arrivalCalendar.SelectedDate != DateTime.MinValue && 
          departureCalendar.SelectedDate != DateTime.MinValue) {
        arrivalDate.CssClass = "data_blue";
        departureDate.CssClass = "data_blue";
        BookingController.CalculatePrice (this);
      }
    }

    /// <summary>
    /// Füllt eine Auswahlliste für die direkte Monatsauswahl mit den nächsten 24 Monaten.
    /// </summary>
    private void FillMonthSelect (DropDownList list, DateTime selectedMonth)
    {
      log.Debug ("BookingForm.FillMonthSelect");
    
      DateTime date = DateTime.Now;
      int selectedIndex = 0;

      for (int i = 0; i < 24; i++) {
        ListItem item = new ListItem (
          date.ToString("yyyy MMMM"), 
          date.ToString("yyyy_MM")
        );

        list.Items.Add (item);

        if (date.Equals (selectedMonth)) {
          selectedIndex = i;
        }

        date = date.AddMonths (1);
      }

      list.SelectedIndex = selectedIndex;
    }

    /// <summary>
    /// Liefert die aktuelle HouseId.
    /// </summary>
    protected int CurrentHouseId
    {
      get {
        if (BookingController != null) {
          return BookingController.GetCurrentHouseId ();
        }
        else {
          return 0;
        }
      }
    }

    /// <summary>
    /// Liefert das aktuelle Haus.
    /// </summary>
    protected HouseBean CurrentHouseBean
    {
      get
      {
        if (BookingController != null)
        {
          return BookingController.GetCurrentHouseBean ();
        }
        else
        {
          return null;
        }
      }
    }

    #endregion
    
    #region Controller Code

	  private BookingController BookingController
	  {
	    get {
        BookingController ctrl = (BookingController)Session["BookingController"];
        if (ctrl == null)
        {
          ctrl = new BookingController();
          Session["BookingController"] = ctrl;
        }
        return ctrl;
      }
	  }
	  
	  /// <summary>
    /// Zur nächsten Seite verzweigen.
    /// </summary>
    private void NextPage (Object sender, EventArgs e)
    {
      log.Debug ("BookingForm.NextSite");
    
      BookingController.FinishPeriodSelection (this);
    }
	  
	  #endregion
    
    #region getter and setter
    
    public DateTime ArrivalDate
    {
      get {return arrivalCalendar.SelectedDate;}
      set {
        arrivalCalendar.SelectedDate = value;
        arrivalCalendar.VisibleDate = value;
        arrivalDate.Text = arrivalCalendar.SelectedDate.ToShortDateString();
      }
    }
    
    public DateTime DepartureDate
    {
      get {return departureCalendar.SelectedDate;}
      set {
        departureCalendar.SelectedDate = value;
        departureCalendar.VisibleDate = value;
        departureDate.Text = departureCalendar.SelectedDate.ToShortDateString();
      }
    }
    
    public bool EnableNextButton
    {
      set {btnNextPage.Disabled = ! value;}
    }
    
    public PriceBean Price
    {
      get {return price;}
      set {
        price = value;
      }
    }
    
    public String ErrorMessage
    {
      set {
        errorMessage.Text = value;
        arrivalDate.CssClass = "data_error";
        departureDate.CssClass = "data_error";
      }
    }
    
    #endregion
  }
}

