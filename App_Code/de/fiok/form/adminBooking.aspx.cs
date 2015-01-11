namespace de.fiok.form
{
  using System;
  using System.Drawing;
  using System.Web.UI;
  using System.Web.UI.HtmlControls;
  using System.Web.UI.WebControls;
  using System.Collections;
  using de.fiok.service;
  using de.fiok.controls;
  using de.fiok.controller;
  using de.fiok.state;
  using de.fiok.core;
  using de.fiok.web;
  using de.fiok.type;
  using de.fiok.master;
  using log4net;
  using System.Reflection;
 
  /// <summary>
  /// Code-Behind Klasse für die Seite adminBooking.aspx, auf der Buchungen und Reservierungen
  /// verwaltet werden können.
  /// </summary>
  /// <remarks>
	/// created by - Steffen Förster
	/// </remarks>
  public class AdminBookingForm : BaseForm
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(AdminBookingForm));
    
    #region Properties
    private static readonly int VIEW_BOOKING = 0;
    private static readonly int VIEW_TENANT = 1;
    private static readonly int VIEW_EMAIL = 2;

    private static readonly int VIEW_CALENDAR = 0;
    private static readonly int VIEW_EDIT = 1;
    
    protected DropDownList houseSelect;
    protected ListBox statusSelect;
    protected TextBox dateFrom;
    protected TextBox dateTo;
    protected HtmlButton btnSearch;
    protected HtmlButton btnCancel;
    protected LinkButton btnHomePage;
    protected UIMessage uiMsgSearch;
    
    protected DataList bookingList;
    protected LinkButton addBookingButton;
    protected UIMessage uiMsgList;
    
    // aktuell gewähltes Haus    
    protected HouseBean house;

    // Controls zur Anzeige des Buchungskalenders oder einer Buchung
    protected MultiView itemCalendarMultiView;
    protected View calendarView;

    // Controls zur Bearbeitung einer Buchung
    protected ValidationSummary editValidationSummary;
    protected MultiView editItemMultiView;
    protected DropDownList salutationSelect;
    protected DropDownList promotionPartnerList;
    protected TextBox title;
    protected TextBox firstname;
    protected TextBox name;
    protected TextBox street;
    protected TextBox zipcode;
    protected TextBox location;
    protected TextBox email;
    protected TextBox telephone;
    protected TextBox fax;
    protected TextBox notes;
    protected TextBox arrival;
    protected TextBox departure;
    protected TextBox rent;
    protected TextBox cleaningCost;
    protected CheckBox calculatePrice;
    protected DropDownList adultCountSelect;
    protected DropDownList childrenCountSelect;
    protected TextBox ageChildren;
    protected DropDownList editStatusSelect;
    protected CheckBox bedClothes;
    protected HyperLink btnSendMail;
    protected HyperLink btnSendPaid1Mail;
    protected HyperLink btnSendPaid2Mail;
    protected Label msgMail;
    protected Label bookingDate;
    protected LinkButton btnApplyItem;
    protected LinkButton btnCancelItem;
    protected UIMessage uiMsgItem;
        
    public void SetHouseList (KeyValue[] houses)
    {
      log.Debug ("AdminBookingForm.SetHouseList");
      
      WebUtils.ConvertToListItems (houseSelect.Items, houses);
    }
    
    public HouseBean House 
    {
      get { return house; }
      set { house = value; }
    }
    
    public void SetBookingList (IList bookings)
    {
      log.Debug ("AdminBookingForm.SetBookingList");
      
      bookingList.DataSource = bookings;
    }
    
    public DateTime DateFrom 
    {
      get { 
        if (dateFrom.Text == String.Empty) {
          return DateTime.MinValue;
        }
        else {
          return Convert.ToDateTime (dateFrom.Text);
        }
      }
      set {dateFrom.Text = value.ToShortDateString ();}
    }
    
    public DateTime DateTo 
    {
      get { 
        if (dateTo.Text == String.Empty) {
          return DateTime.MinValue;
        }
        else {
          return Convert.ToDateTime (dateTo.Text);
        }
      }
    }
 
    public int[] SelectedBookingStatus
    {
      get {
        ArrayList selectedItems = new ArrayList ();
        foreach (ListItem item in statusSelect.Items) {
          if (item.Selected) {
            selectedItems.Add (Int32.Parse (item.Value));
          }
        }
        return (int[])selectedItems.ToArray (typeof (int));
      }
      set {
        for (int i = 0; i < value.Length; i++) {
          foreach (ListItem item in statusSelect.Items) {
            BookingStatus status = (BookingStatus)Int32.Parse (item.Value);
            if (status == (BookingStatus)value[i]) {
              item.Selected = true;
            }
          }
        }
      }
    }

    public UIMessage ItemMessage
    {
      get { return uiMsgItem; }
    }

    #endregion
    
    #region Initialize Page
    
    override protected void OnInit (EventArgs e)
    {
      log.Debug ("AdminBookingForm.OnInit");
      
      InitializeComponent();
      base.OnInit(e); 
    }

    /// <summary>
    /// Komponenten initialisieren.
    /// </summary>
    private void InitializeComponent ()
    {
      log.Debug ("AdminBookingForm.InitializeComponent");

			this.Load += new System.EventHandler (this.PageLoad);
			this.PreRender += new System.EventHandler (this.PreRenderHandler);
			
			// houseSelect
			this.houseSelect.SelectedIndexChanged += new System.EventHandler (this.HouseChanged);
			this.houseSelect.AutoPostBack = true;
			
			// Suchen
			this.btnSearch.ServerClick += new System.EventHandler (this.Search);
			
			// Abbrechen
			this.btnCancel.ServerClick += new System.EventHandler (this.Cancel);
			
			// bookingList
			this.bookingList.EditCommand += new DataListCommandEventHandler (this.EditBookingCmd);
			this.bookingList.ItemCommand += new DataListCommandEventHandler (this.BookingItemCmd);
			this.bookingList.ItemDataBound += new DataListItemEventHandler (this.BookingItemBound);
      this.addBookingButton.Click += new EventHandler (this.InsertBookingCmd);

      this.btnApplyItem.Click +=  new System.EventHandler (this.ApplyBookingItem);
      this.btnCancelItem.Click +=  new System.EventHandler (this.CancelBookingItem);
	  }
    
    #endregion
  
    #region Eventhandler
    
    /// <summary>
    /// Initialisierung des Forms.
    /// </summary>
    private void PageLoad (Object sender, EventArgs e)
    {
      log.Debug ("AdminBookingForm.PageLoad");

      if (!IsPostBack) {
        int landlordId = Int32.Parse (User.Identity.Name);
        InitBookingStatus (statusSelect);
        InitBookingStatus (editStatusSelect);
        InitSalutationSelect (salutationSelect);
        AdminBookingController.PrepareForm (this, landlordId);
      }
      else {
        // HouseBean aus dem ViewState laden
        house = (HouseBean)ViewState["houseBean"];
      }

      // Buchungen in der DataList setzen, da dieses Control die Daten nicht im ViewState speichert
      if (bookingList.DataSource == null) {
        AdminBookingController.SetBookings (this);
      }
      
      log.Debug ("end AdminBookingForm.PageLoad");
    }
    
    /// <summary>
    /// Daten werden an die Controls gebunden.
    /// </summary>
    private void PreRenderHandler (Object sender, EventArgs e)
    {
      log.Debug ("AdminBookingForm.PreRenderHandler");

      // HouseId für den Buchungskalender setzen
      if (calendarView != null) {
        Control calendar = calendarView.FindControl ("calendar");
        log.Debug ("calendar = " + calendar);
        
        PropertyInfo pInfo = calendar.GetType ().GetProperty ("HouseID");
        log.Debug ("pInfo = " + pInfo);

        pInfo.SetValue (calendar, house.ID, null);
      }

      // ggf. neue Seite aufrufen -> durch Event auf MasterPage
      CustomAdminMaster master = (CustomAdminMaster)this.Master;
      if (master.NextNavigation != null && !"adminBooking".Equals (master.NextNavigation)) {
        AdminBookingController.NavigateToTarget (master.NextNavigation);
      }
      else {
        // aktuelles Haus im ViewState speichern
        ViewState.Add ("houseBean", house);

        this.DataBind ();
      }
    }

    /// <summary>
    /// Ein neues Haus wurde ausgewählt.
    /// </summary>
    private void HouseChanged (Object sender, EventArgs e) 
    {
      log.Debug ("AdminBookingForm.HouseChanged");
      
      // neues Haus laden
      AdminBookingController.ChangeHouse (this, Int32.Parse (houseSelect.SelectedValue));
      
      // ggf. gestarteten Edit-Modus abbrechen
      bookingList.EditItemIndex = -1;

      // Edit-View ausblenden
      itemCalendarMultiView.ActiveViewIndex = VIEW_CALENDAR;

      // Liste löschen
      AdminBookingController.ResetBookings (this);

      // neue Suche durchführen
      Search (sender, e);
    }

    /// <summary>
    /// Buchungen für ein Haus entsprechend den Filterkriterien suchen.
    /// </summary>
    private void Search (Object sender, EventArgs e) 
    {
      log.Debug ("AdminBookingForm.Search");
      
      //WebUtils.EnableValidators (this, "search");
      this.Validate ("search");
      
      if (this.IsValid) {
        log.Debug ("is valid !");
        int count = AdminBookingController.SearchBookings (this);
        
        if (count > 0) {
          uiMsgSearch.InfoMessage = AppResources.GetMessage("AdminBooking_msg.items.found", count);
        }
        else {
          uiMsgSearch.WarnMessage = AppResources.GetMessage("AdminBooking_msg.items.not.found");
        }
        
        // Edit-Modus abbrechen
        bookingList.EditItemIndex = -1;

        // Edit-View ausblenden
        itemCalendarMultiView.ActiveViewIndex = VIEW_CALENDAR;
      }
    }

    /// <summary>
    /// Änderungen nicht speichern und Maske zurücksetzen.
    /// </summary>
    private void Cancel (Object sender, EventArgs e) 
    {
      log.Debug ("AdminBookingForm.Cancel");
  
      // Such-Kriterien zurücksetzen
      dateFrom.Text = String.Empty;
      dateTo.Text = String.Empty;
      foreach (ListItem item in statusSelect.Items) {
        item.Selected = false;
      }
      
      // Liste löschen
      AdminBookingController.ResetBookings (this);
      
      // ggf. gestarteten Edit-Modus abbrechen
      bookingList.EditItemIndex = -1;

      // Edit-View ausblenden
      itemCalendarMultiView.ActiveViewIndex = VIEW_CALENDAR;
    }

    // -- Änderungen der Ansichten im Bearbeitungspanel

    protected void TabBtnBooking_OnClick (Object sender, EventArgs e)
    {
      this.Validate ("edit");
      if (! this.IsValid) {
        return;
      }

      editItemMultiView.ActiveViewIndex = VIEW_BOOKING;
    }

    protected void TabBtnTenant_OnClick (Object sender, EventArgs e)
    {
      this.Validate ("edit");
      if (! this.IsValid) {
        return;
      }

      editItemMultiView.ActiveViewIndex = VIEW_TENANT;
    }

    protected void TabBtnEmail_OnClick (Object sender, EventArgs e)
    {
      this.Validate ("edit");
      if (! this.IsValid) {
        return;
      }

      editItemMultiView.ActiveViewIndex = VIEW_EMAIL;
    }
      
    /// <summary>
    /// Ein Button innerhalb eines Items wurde gedrückt.
    /// </summary>
    private void BookingItemCmd (Object sender, DataListCommandEventArgs e) 
    {
      log.Debug ("AdminBookingForm.BookingItemCmd");
    }  
      
    /// <summary>
    /// Eine Buchung wurde für die Bearbeitung ausgewählt.
    /// </summary>
    private void EditBookingCmd (Object sender, DataListCommandEventArgs e) 
    {
      log.Debug ("AdminBookingForm.EditBookingCmd");
      
      // zuerst Buchungstab zeigen
      editItemMultiView.ActiveViewIndex = VIEW_BOOKING;

      bookingList.EditItemIndex = e.Item.ItemIndex;
      BookingItem item = (BookingItem)((IList)bookingList.DataSource)[bookingList.EditItemIndex];

      // Controls initialisieren
      salutationSelect.SelectedValue = ((Int32)item.Tenant.Salutation).ToString ();
      title.Text = item.Tenant.Title;
      firstname.Text = item.Tenant.Firstname;
      name.Text = item.Tenant.Name;
      street.Text = item.Tenant.Street;
      zipcode.Text = item.Tenant.Zipcode;
      location.Text = item.Tenant.Location;
      email.Text = item.Tenant.Email;
      telephone.Text = item.Tenant.Telephone;
      fax.Text = item.Tenant.Fax;
      adultCountSelect.SelectedValue = item.CountAdults.ToString ();
      childrenCountSelect.SelectedValue = item.CountChildren.ToString ();
      ageChildren.Text = item.AgeChildren;
      notes.Text = item.Notes;
      bedClothes.Checked = item.BedClothes;
      promotionPartnerList.SelectedValue = ((Int32)item.PromotionPartner).ToString();
      bookingDate.Text = item.BookingDate.ToShortDateString ();
      arrival.Text = item.Arrival.ToShortDateString ();
      departure.Text = item.Departure.ToShortDateString ();
      rent.Text = item.Price.Rent.ToString ();
      cleaningCost.Text = item.Price.CleaningCost.ToString ();
      calculatePrice.Checked = false;

      // aktuellen Status anzeigen
      // der Status 'storniert' kann nicht mehr verändert werden
      editStatusSelect.SelectedValue = BookingStatusType.GetValueAsString (item.Status);
      if (item.Status == BookingStatus.CANCELED) {
        editStatusSelect.Enabled = false;
      }
      else {
        editStatusSelect.Enabled = true;
      }

      // E-Mail Links erstellen
      PrepareEmailLinks (item);

      // Edit-View aktivieren
      itemCalendarMultiView.ActiveViewIndex = VIEW_EDIT;
    }

    /// <summary>
    /// E-Mail Links erstellen, wenn ein Mail-Adresse gespeichert wurde.
    /// </summary>
    private void PrepareEmailLinks (BookingItem item)
    {
      log.Debug ("AdminBookingForm.PrepareEmailLinks");

      btnSendMail.Visible = false;
      btnSendPaid1Mail.Visible = false;
      btnSendPaid2Mail.Visible = false;
      msgMail.Text = "";

      if (!String.IsNullOrEmpty (item.Tenant.Email)) {
        btnSendMail.Visible = true;

        // normale Mail versenden
        String url =
          "mailto:" + item.Tenant.Email +
          "?body=\n" +
          AppResources.GetMessage("AdminBooking_standard.mail.salutation",
                               SalutationType.GetName (item.Tenant.Salutation),
                               item.Tenant.Title,
                               item.Tenant.Name) + "\n\n";

        btnSendMail.NavigateUrl = HtmlEncode(url);//.Replace ("+", " ");

        // Links für Information über Geldeingang erstellen -> nur bei Rolle Admin
        if (LandlordRoleProvider.IsInRole (LandlordRole.ADMIN)) {
          btnSendPaid1Mail.Visible = true;
          btnSendPaid2Mail.Visible = true;

          url =
            "mailto:" + item.Tenant.Email +
            "?subject=" +
            AppResources.GetMessage("AdminBooking_first.paid.mail.subject") +
            "&body=\n" +
            AppResources.GetMessage("AdminBooking_paid.mail.salutation",
                                   SalutationType.GetName (item.Tenant.Salutation),
                                   item.Tenant.Title,
                                   item.Tenant.Name) + "\n" +
            AppResources.GetMessage("AdminBooking_first.paid.mail.body");
  

          btnSendPaid1Mail.NavigateUrl = HtmlEncode(url);//.Replace ("+", " ");

          url =
            "mailto:" + item.Tenant.Email +
            "?subject=" +
            AppResources.GetMessage("AdminBooking_second.paid.mail.subject") +
            "&body=%0D%0A" +
            AppResources.GetMessage("AdminBooking_paid.mail.salutation",
                                   SalutationType.GetName(item.Tenant.Salutation),
                                   item.Tenant.Title,
                                   item.Tenant.Name) + "%0D%0A" +
            AppResources.GetMessage("AdminBooking_second.paid.mail.body");


          btnSendPaid2Mail.NavigateUrl = HtmlEncode(url);//.Replace ("+", " ");
        }
      }
      else {
        msgMail.Text = AppResources.GetMessage("AdminBooking_msg.no.mail.address");
      }
    }

    public static string HtmlEncode(string text)
    {
        System.Text.Encoding iso     = System.Text.Encoding.GetEncoding("ISO-8859-1");
        System.Text.Encoding unicode = System.Text.Encoding.Unicode;

        // Convert the string into a byte[].
        byte[] unicodeBytes = unicode.GetBytes(text);

        // Perform the conversion from one encoding to the other.
        byte[] isoBytes = System.Text.Encoding.Convert(unicode, iso, unicodeBytes);

        // Convert the new byte[] into a char[] and then into a string.
        // This is a slightly different approach to converting to illustrate
        // the use of GetCharCount/GetChars.
        char[] isoChars = new char[iso.GetCharCount(isoBytes, 0, isoBytes.Length)];
        iso.GetChars(isoBytes, 0, isoBytes.Length, isoChars, 0);
        return new string(isoChars);

        /*
        char[] chars = System.Web.HttpUtility.HtmlEncode(text).ToCharArray();
        System.Text.StringBuilder result = new System.Text.StringBuilder(text.Length + (int)(text.Length * 0.1));

        foreach (char c in chars)
        {
            int value = Convert.ToInt32(c);
            if (value > 127)
                result.AppendFormat("&#{0};", value);
            else
                result.Append(c);
        }

        return result.ToString();
        */
    }

    /// <summary>
    /// Validierung der einzelnen Views, da automatisch nur die aktive View validiert wird.
    /// </summary>
    private bool ValidateEditViews ()
    {
      // View Buchung
      editItemMultiView.ActiveViewIndex = VIEW_BOOKING;
      this.Validate ("edit");

      if (! this.IsValid) {
        return false;
      }

      // View Mieter
      editItemMultiView.ActiveViewIndex = VIEW_TENANT;
      this.Validate ("edit");

      if (! this.IsValid) {
        return false;
      }

      return true;
    }

    /// <summary>
    /// Änderungen für eine Buchung sollen gespeichert werden.
    /// </summary>
    private void ApplyBookingItem (Object sender, EventArgs e) 
    {
      log.Debug ("AdminBookingForm.ApplyBookingItem");

      if (! ValidateEditViews ()) {
        return;
      }

      bool insertMode = (bookingList.EditItemIndex == -1);

      // BookingItem für das Update erstellen -> im Falle eines Fehlers werden so die ursprünglichen 
      // Werte nicht überschrieben
      BookingItem newItem = new BookingItem ();

      newItem.Arrival = Convert.ToDateTime (arrival.Text);
      newItem.Departure = Convert.ToDateTime (departure.Text);
      newItem.Tenant = new TenantBean ();

      if (insertMode) {
        newItem.IsNew = true;
        newItem.HouseID = house.ID;
        newItem.BookingDate = DateTime.Now;
      }
      else {
        BookingItem oldItem = (BookingItem)((IList)bookingList.DataSource)[bookingList.EditItemIndex];

        newItem.ID = oldItem.ID;
        newItem.HouseID = oldItem.HouseID;
        newItem.Tenant.ID = oldItem.Tenant.ID;
        newItem.BookingDate = oldItem.BookingDate;

        // prüfen, ob der Buchungszeitraum verändert wurde
        newItem.PeriodModified = !newItem.Arrival.Equals (oldItem.Arrival) || 
                                 !newItem.Departure.Equals (oldItem.Departure); 
      }

      // andere Werte übernehmen
      newItem.Tenant.Salutation = (Salutation)Int32.Parse (salutationSelect.SelectedValue);
      newItem.Tenant.Title = title.Text;
      newItem.Tenant.Firstname = firstname.Text;
      newItem.Tenant.Name = name.Text;
      newItem.Tenant.Street = street.Text;
      newItem.Tenant.Zipcode = zipcode.Text;
      newItem.Tenant.Location = location.Text;
      newItem.Tenant.Email = email.Text;
      newItem.Tenant.Telephone = telephone.Text;
      newItem.Tenant.Fax = fax.Text;
      newItem.CountAdults = Int32.Parse (adultCountSelect.SelectedValue);
      newItem.CountChildren = Int32.Parse (childrenCountSelect.SelectedValue);
      newItem.Status = (BookingStatus)Int32.Parse (editStatusSelect.SelectedValue);
      newItem.Notes = notes.Text;

      // einige Angaben nur für die Buchungsversion
      if (LandlordRoleProvider.IsInRole (LandlordRole.ADMIN)) {
        newItem.BedClothes = bedClothes.Checked;
        newItem.AgeChildren = ageChildren.Text;
        newItem.PromotionPartner = Int32.Parse(promotionPartnerList.SelectedValue);

        // soll der Mietpreis berechnet werden ?
        if (calculatePrice.Checked) {
          MessageResult calculateResult = AdminBookingController.CalculatePrice (newItem);
          if (calculateResult.Result) {
            newItem.Price = (PriceBean)calculateResult.Value;
            rent.Text = newItem.Price.Rent.ToString ();
            cleaningCost.Text = newItem.Price.CleaningCost.ToString ();
          }
          else {
            // Fehler bei der Preisberechnung
            editItemMultiView.ActiveViewIndex = VIEW_BOOKING;
            uiMsgItem.ErrorMessage = calculateResult.Message;
            return;
          }
        }
        // Übernahme der Eingabedaten
        else {
          newItem.Price = new PriceBean ();
          if (!String.IsNullOrEmpty (rent.Text)) {
            newItem.Price.Rent = Convert.ToInt32 (rent.Text);
          }
          if (!String.IsNullOrEmpty (cleaningCost.Text)) {
            newItem.Price.CleaningCost = Convert.ToInt32 (cleaningCost.Text);
          }
        }
      }
      else {
        newItem.Price = new PriceBean ();
      }

      if (insertMode) {
        // Buchung neu erstellen
        MessageResult insertResult = AdminBookingController.InsertBooking (newItem);
        if (insertResult.Result) {
          uiMsgList.InfoMessage = AppResources.GetMessage("AdminBooking_msg.booking.created");

          // Buchung der Liste hinzufügen
          ((IList)bookingList.DataSource).Add (newItem);

          // Edit-View ausblenden
          itemCalendarMultiView.ActiveViewIndex = VIEW_CALENDAR;

          // kein Item ist ausgewählt
          bookingList.EditItemIndex = -1;
        }
        else {
          // Fehler bei der Buchung
          editItemMultiView.ActiveViewIndex = VIEW_BOOKING;
          uiMsgItem.ErrorMessage = insertResult.Message;
        }
      }
      else {
        // Buchung ändern
        MessageResult updateResult = AdminBookingController.UpdateBooking (newItem);
        if (updateResult.Result) {
          // BookingItem ersetzen
          ((IList)bookingList.DataSource)[bookingList.EditItemIndex] = newItem;

          uiMsgList.InfoMessage = AppResources.GetMessage("Globals_msg.modifications.saved");

          // Edit-View ausblenden
          itemCalendarMultiView.ActiveViewIndex = VIEW_CALENDAR;

          // kein Item ist ausgewählt
          bookingList.EditItemIndex = -1;
        }
        else {
          // Fehler bei der Buchung
          editItemMultiView.ActiveViewIndex = VIEW_BOOKING;
          uiMsgItem.ErrorMessage = updateResult.Message;
        }
      }
    }
    
    /// <summary>
    /// Änderungen nicht übernehmen.
    /// </summary>
    private void CancelBookingItem (Object sender, EventArgs e) 
    {
      log.Debug ("AdminBookingForm.CancelBookingItem");
           
      // Edit-View ausblenden
      itemCalendarMultiView.ActiveViewIndex = VIEW_CALENDAR;
      
      // kein Item ist ausgewählt
      bookingList.EditItemIndex = -1;
    }

    private void initEditControls ()
    {
      salutationSelect.SelectedIndex = 0;
      title.Text = "";
      firstname.Text = (LandlordRoleProvider.IsInRole (LandlordRole.ADMIN)) ? "" : "?";
      name.Text = (LandlordRoleProvider.IsInRole (LandlordRole.ADMIN)) ? "" : "?";
      street.Text = "";
      zipcode.Text = "";
      location.Text = "";
      email.Text = "";
      telephone.Text = "";
      fax.Text = "";
      adultCountSelect.SelectedValue = "2";
      childrenCountSelect.SelectedValue = "0";
      ageChildren.Text = "";
      notes.Text = "";
      bedClothes.Checked = false;
      arrival.Text = "";
      departure.Text = "";
      rent.Text = "";
      cleaningCost.Text = "";
      calculatePrice.Checked = false;
      editStatusSelect.SelectedValue = BookingStatusType.GetValueAsString (BookingStatus.BOOKED);
      editStatusSelect.Enabled = true;
      promotionPartnerList.SelectedIndex = 0;
      bookingDate.Text = "";
    }

    /// <summary>
    /// Fügt ein neues Item in die Liste ein.
    /// </summary>
    private void InsertBookingCmd (Object sender, EventArgs e)
    {
      log.Debug ("AdminBookingForm.InsertBookingCmd");

      // Edit-View aktivieren
      itemCalendarMultiView.ActiveViewIndex = VIEW_EDIT;

      // kein Item ist ausgewählt
      bookingList.EditItemIndex = -1;

      // zuerst Buchungstab zeigen
      editItemMultiView.ActiveViewIndex = VIEW_BOOKING;

      // Voreinstellungen
      initEditControls ();
    }

    /// <summary>
    /// Wird aufgerufen, bevor ein BookingItem für die Anzeige aufbereitet wird.
    /// </summary>
    private void BookingItemBound (Object sender, DataListItemEventArgs e)
    {
      log.Debug ("AdminBookingForm.BookingItemBound");

      if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem) {
        BookingItem item = (BookingItem)e.Item.DataItem;

        // Hintergrundfarbe je nach Status verändern
        if (item.Status == BookingStatus.CANCELED || item.Status == BookingStatus.RESERVED) {
          Color backColor = item.Status == 
            BookingStatus.CANCELED ? Color.FromArgb (255, 207, 207) : Color.FromArgb (255, 248, 175);
        
          TableRow row = null;
          row = (TableRow)e.Item.FindControl ("itemRow");
          row.BackColor = backColor;
        }

      //  // Bemerkungsbutton nur anzeigen, wenn der Eintrag nicht bearbeitet wird
      //  if ((StringUtils.IsNotBlank (item.Notes) || house.ExtraHirable) && 
      //      notesItemIndex != e.Item.ItemIndex) {
      //    LinkButton viewButton = (LinkButton)e.Item.FindControl ("cmdViewNotes");
      //    viewButton.Visible = true;
      //  }
        
      //  // Bemerkungszeile nur anzeigen, wenn die Bemerkung nicht bearbeitet wird
      //  if (notesItemIndex == e.Item.ItemIndex) {
      //    Panel notesPanel = (Panel)e.Item.FindControl ("notesPanel");
      //    notesPanel.Visible = true;
      //  }
        
        

      //  // E-Mail-Links erstellen
      //  HyperLink firstPaidMail = (HyperLink)e.Item.FindControl ("firstPaidMail");
      //  String url = 
      //    "mailto:" + item.Tenant.Email + 
      //    "?subject=" + Server.UrlEncode (
      //      Resources.GetMessage ("AdminBooking_first.paid.mail.subject")
      //    ) +
      //    "&body=\n" + Server.UrlEncode (
      //      Resources.GetMessage("AdminBooking_paid.mail.salutation", 
      //                           SalutationType.GetName(item.Tenant.Salutation),
      //                           item.Tenant.Title,
      //                           item.Tenant.Name) + "\n" +
      //      Resources.GetMessage("AdminBooking_first.paid.mail.body")
      //    );

      //  firstPaidMail.NavigateUrl = url.Replace ("+", " ");

      //  HyperLink secondPaidMail = (HyperLink)e.Item.FindControl ("secondPaidMail");
        
      //  url = 
      //    "mailto:" + item.Tenant.Email + 
      //    "?subject=" + Server.UrlEncode (
      //      Resources.GetMessage ("AdminBooking_second.paid.mail.subject")
      //    ) +
      //    "&body=\n" + Server.UrlEncode (
      //      Resources.GetMessage("AdminBooking_paid.mail.salutation", 
      //                           SalutationType.GetName(item.Tenant.Salutation),
      //                           item.Tenant.Title,
      //                           item.Tenant.Name) + "\n" +
      //      Resources.GetMessage("AdminBooking_second.paid.mail.body")
      //    );

      //  secondPaidMail.NavigateUrl = url.Replace ("+", " ");
      }
      
      if (e.Item.ItemType == ListItemType.EditItem) {
        Color backColor = Color.FromArgb (185, 211, 238);
        TableRow row = (TableRow)e.Item.FindControl ("itemRow");
        row.BackColor = backColor;
      }
    }

    
    #endregion
  
    #region Controller Code
  
    private AdminBookingController AdminBookingController
	  {	  
	    get {
        AdminBookingController ctrl = (AdminBookingController)Session["AdminBookingController"];
        if (ctrl == null)
        {
          ctrl = new AdminBookingController();
          Session["AdminBookingController"] = ctrl;
        }
        return ctrl;
      }
	  }

    #endregion
    
    #region sonstige Methoden
    
    /// <summary>
    /// Initialisiert die Auswahlliste mit allen möglichen BookingStatus.
    /// </summary>
    private void InitBookingStatus (ListBox listBox)
    {
      log.Debug ("AdminBookingForm.InitBookingStatus");
    
      listBox.SelectionMode = ListSelectionMode.Multiple;
      
      listBox.Items.Add (new ListItem (BookingStatusType.GetName (BookingStatus.BOOKED), 
                                       BookingStatusType.GetValueAsString (BookingStatus.BOOKED)));
      listBox.Items.Add (new ListItem (BookingStatusType.GetName (BookingStatus.RESERVED), 
                                       BookingStatusType.GetValueAsString (BookingStatus.RESERVED)));
      listBox.Items.Add (new ListItem (BookingStatusType.GetName (BookingStatus.CANCELED), 
                                       BookingStatusType.GetValueAsString (BookingStatus.CANCELED)));
            
      listBox.SelectedIndex = -1;
    }   
    
    /// <summary>
    /// Initialisiert die Auswahlliste mit allen möglichen BookingStatus.
    /// </summary>
    private void InitBookingStatus (DropDownList listBox)
    {
      log.Debug ("AdminBookingForm.InitBookingStatus");
    
      listBox.Items.Add (new ListItem (BookingStatusType.GetName (BookingStatus.BOOKED), 
                                       BookingStatusType.GetValueAsString (BookingStatus.BOOKED)));
      listBox.Items.Add (new ListItem (BookingStatusType.GetName (BookingStatus.RESERVED), 
                                       BookingStatusType.GetValueAsString (BookingStatus.RESERVED)));
      listBox.Items.Add (new ListItem (BookingStatusType.GetName (BookingStatus.CANCELED), 
                                       BookingStatusType.GetValueAsString (BookingStatus.CANCELED)));
    }   

    /// <summary>
    /// Initialisiert die Auswahlliste mit allen möglichen Anreden.
    /// </summary>
    private void InitSalutationSelect (DropDownList listBox)
    {
      log.Debug ("AdminBookingForm.InitSalutationSelect");
    
      listBox.Items.Add (new ListItem (SalutationType.GetName (Salutation.MR), 
                                       SalutationType.GetValueAsString (Salutation.MR)));
      listBox.Items.Add (new ListItem (SalutationType.GetName (Salutation.MRS), 
                                       SalutationType.GetValueAsString (Salutation.MRS)));
    }   
       
    #endregion
  }
}
