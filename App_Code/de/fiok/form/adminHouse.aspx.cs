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
  using de.fiok.master;
  using log4net;
  
  /// <summary>
  /// Code-Behind Klasse für die Seite adminHouse.aspx, auf der die Daten für ein Ferienhaus
  /// gepflegt werden können.
  /// </summary>
  /// <remarks>
	/// created by - Steffen Förster
	/// </remarks>
  public class AdminHouseForm : BaseForm
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(AdminHouseForm));
    
    #region Properties
    protected DropDownList houseSelect;
    protected TextBox location;
    protected TextBox cleaningCost;
    protected TextBox minDaysSeason;
    protected TextBox minDaysTotal;
    protected CheckBox bedClothes;
    protected HtmlButton btnSaveHouseData;
    protected HtmlButton btnCancel;
    protected UIMessage uiMsgSave;
    
    protected DataListValidationSummary editValidationSummary;
    
    protected DropDownList yearSelect;
    protected DataList priceList;
    protected EditPriceItem editItem;
    protected Panel intervalWarning;
    protected Panel addPriceItem;
    protected LinkButton addPriceButton;
            
    private HouseBean house;
    
    public void SetHouseList (KeyValue[] houses)
    {
      log.Debug ("AdminHouseForm.SetHouseList");
      
      WebUtils.ConvertToListItems (houseSelect.Items, houses);
    }
    
    public HouseBean House 
    {
      set {
        house = value;
        location.Text = house.Location;
        cleaningCost.Text = house.CleaningCost.ToString ();
        minDaysSeason.Text = house.MinDaysSeason.ToString ();
        minDaysTotal.Text = house.MinDaysTotal.ToString ();
        bedClothes.Checked = house.BedClothesHirable;
      }
    }
    
    public void SetPriceList (IList prices)
    {
      log.Debug ("AdminHouseForm.SetPriceList");
      
      priceList.DataSource = prices;
    }
    
    /// <summary>
    /// Liefert das selektierte Jahr, für das die Preise bearbeitet werden sollen.
    /// </summary>
    public int SelectedYear 
    {
      get {return Int32.Parse (yearSelect.SelectedValue);}
    }
    
    #endregion
    
    #region Initialize Page
    
    override protected void OnInit (EventArgs e)
    {
      log.Debug ("AdminHouseForm.OnInit");
      
      InitializeComponent();
      base.OnInit(e);
    }

    /// <summary>
    /// Komponenten initialisieren.
    /// </summary>
    private void InitializeComponent ()
    {
      log.Debug ("AdminHouseForm.InitializeComponent");

			this.Load += new System.EventHandler (this.PageLoad);
			this.PreRender += new System.EventHandler (this.PreRenderHandler);
			
			// houseSelect
			this.houseSelect.SelectedIndexChanged += new System.EventHandler (this.HouseChanged);
			this.houseSelect.AutoPostBack = true;
			
			this.btnSaveHouseData.ServerClick += new System.EventHandler (this.SaveHouseData);
      this.btnCancel.ServerClick += new System.EventHandler (this.Cancel);
			
			// yearSelect
			this.yearSelect.SelectedIndexChanged += new System.EventHandler (this.YearChanged);
			this.yearSelect.AutoPostBack = true;
			
			// priceList
			this.priceList.EditCommand += new DataListCommandEventHandler (this.EditPriceCmd);
			this.priceList.UpdateCommand += new DataListCommandEventHandler (this.UpdatePriceCmd);
			this.priceList.CancelCommand += new DataListCommandEventHandler (this.CancelPriceCmd);
			this.priceList.DeleteCommand += new DataListCommandEventHandler (this.DeletePriceCmd);
      this.addPriceButton.Click += new EventHandler (this.InsertPriceCmd);
	  }
    
    #endregion
  
    #region Eventhandler
    
    /// <summary>
    /// Initialisierung des Forms.
    /// </summary>
    private void PageLoad (Object sender, EventArgs e)
    {
      log.Debug ("AdminHouseForm.PageLoad");
      
      if (! IsPostBack) {
        int landlordId = Int32.Parse (User.Identity.Name);
        AdminHouseController.PrepareForm (this, landlordId);

        // Preise nur anzeigen, wenn mindestens ein Ferienhaus gefunden wurde
        if (houseSelect.Items.Count > 0) {
          FillYearSelect ();
        }
      }
      else {
        // HouseBean aus dem ViewState laden
        house = (HouseBean)ViewState["houseBean"];
      }
      
      // Preise in der DataList setzen, da dieses Control die Daten nicht im ViewState speichert
      if (priceList.DataSource == null) {
        AdminHouseController.SetPrices (this);
      }
      
      log.Debug ("end AdminHouseForm.PageLoad");
    }
    
    /// <summary>
    /// Daten werden an die Controls gebunden.
    /// </summary>
    private void PreRenderHandler (Object sender, EventArgs e)
    {
      log.Debug ("AdminHouseForm.PreRenderHandler");
      
      // ggf. neue Seite aufrufen -> durch Event auf MasterPage
      CustomAdminMaster master = (CustomAdminMaster)this.Master;
      if (master.NextNavigation != null && !"adminHouse".Equals (master.NextNavigation)) {
        AdminHouseController.NavigateToTarget (master.NextNavigation);
      }
      else {
        // ggf. Warnung anzeigen
        CheckIntervalCompleteness ();

        // neuer Preis kann nur hinzugefügt werden, wenn kein Eintrag bearbeitet wird
        if (priceList.EditItemIndex == -1) {
          addPriceItem.Visible = true;
          btnSaveHouseData.Disabled = false;
        }
        else {
          btnSaveHouseData.Disabled = true;
        }

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
      log.Debug ("AdminHouseForm.HouseChanged");
      
      AdminHouseController.ChangeHouse (this, Int32.Parse (houseSelect.SelectedValue));
      
      // ggf. gestarteten Preis-Edit-Modus abbrechen
      priceList.EditItemIndex = -1;
    }

    /// <summary>
    /// Daten für ein Haus speichern.
    /// </summary>
    private void SaveHouseData (Object sender, EventArgs e) 
    {
      log.Debug ("AdminHouseForm.SaveHouseData");
      
      WebUtils.EnableValidators (this, "house");
      this.Validate ();
      
      if (this.IsValid) {
        AdminHouseController.UpdateHouseData (this);
        uiMsgSave.InfoMessage = AppResources.GetMessage("Globals_msg.modifications.saved");
      }
    }

    /// <summary>
    /// Änderungen verwerfen -> Daten neu laden.
    /// Ggf. Update eines Preises ändern.
    /// </summary>
    private void Cancel (Object sender, EventArgs e) 
    {
      log.Debug ("AdminHouseForm.Cancel");
      
      // Editieren eines Preises abbrechen
      priceList.EditItemIndex = -1;

      // Hausdaten neu laden
      AdminHouseController.ChangeHouse (this, Int32.Parse (houseSelect.SelectedValue));

      uiMsgSave.InfoMessage = AppResources.GetMessage("Globals_msg.modifications.reseted"); 
    }
      
    /// <summary>
    /// Ein neues Jahr wurde ausgewählt.
    /// </summary>
    private void YearChanged (Object sender, EventArgs e) 
    {
      log.Debug ("AdminHouseForm.YearChanged");
      
      AdminHouseController.ChangePrices (this);
      
      // ggf. gestarteten Preis-Edit-Modus abbrechen
      priceList.EditItemIndex = -1;
    }  
    
    /// <summary>
    /// Ein Preis soll entfernt werden.
    /// </summary>
    private void DeletePriceCmd (Object sender, DataListCommandEventArgs e) 
    {
      log.Debug ("AdminHouseForm.DeletePriceCmd");
      
      // ggf. ungültiges Item löschen (können nur neue Items sein)
      RemoveInvalidItem ();
      
      // aktuelles HousePriceInterval-Objekt holen und aus der Liste entfernen
      HousePriceInterval price = (HousePriceInterval)((IList)priceList.DataSource)[e.Item.ItemIndex];
      AdminHouseController.RemovePriceInterval (this, price);
      
      // ggf. Edit-Modus abbrechen
      priceList.EditItemIndex = -1;
    }
    
    /// <summary>
    /// Ein Preis wurde für die Bearbeitung ausgewählt.
    /// </summary>
    private void EditPriceCmd (Object sender, DataListCommandEventArgs e) 
    {
      log.Debug ("AdminHouseForm.EditPriceCmd");
      
      // ggf. ungültiges Item löschen (können nur neue Items sein)
      RemoveInvalidItem ();
      
      priceList.EditItemIndex = e.Item.ItemIndex;
      
      // aktuelles HousePriceInterval-Objekt holen
      HousePriceInterval price = (HousePriceInterval)((IList)priceList.DataSource)[priceList.EditItemIndex];
           
      // erstellt ein neues EditPriceItem -> Controls zur Bearbeitung lesen hier die Daten aus
      editItem = new EditPriceItem (price); 
    }
    
    /// <summary>
    /// Ein Preis soll gespeichert werden.
    /// </summary>
    private void UpdatePriceCmd (Object sender, DataListCommandEventArgs e) 
    {
      log.Debug ("AdminHouseForm.UpdatePriceCmd");
      
      // Validierung durchführen -> alle Validators im Item-Control aktivieren
      WebUtils.EnableValidators (e.Item);
      this.Validate ();

      if (this.IsValid)
      {
        log.Debug ("edit item is valid");
        
        // weitere Prüfung der Daten
        log.Debug ("index = " + priceList.EditItemIndex);
        log.Debug ("count = " + ((IList)priceList.DataSource).Count);
        
        HousePriceInterval oldInterval = (HousePriceInterval)((IList)priceList.DataSource)[priceList.EditItemIndex];
        HousePriceInterval newInterval = EditPriceItem.CreatePriceInterval (e.Item);
        MessageResult valid = AdminHouseController.ValidateChangedPriceInterval (
          newInterval, priceList.EditItemIndex, SelectedYear);
      
        if (valid.Result) {
          log.Debug ("edit item is valid - extended validation");
          
          // Daten aus den Eingabefeldern in das bearbeitete Bean übernehmen
          EditPriceItem.Update (e.Item, oldInterval);
          oldInterval.Valid = true;
 
          priceList.EditItemIndex = -1;
        }
        else {
          log.Debug ("edit item isn't valid - extended validation");
          
          editValidationSummary.AddErrorMessage (valid.Message);
          editValidationSummary.AddErrorControl ("editStart");
          editValidationSummary.AddErrorControl ("editEnd");
       
          // Daten aus den Eingabe-Feldern in ein neues EditItem übernehmen -> bearbeitete Werte 
          // können somit angezeigt werden
          editItem = new EditPriceItem (e.Item);
        }
      }
      else {
        log.Debug ("edit item isn't valid");
        
        // Fehlermeldungen der Validators einsammeln, da das ValidationSummary-Control
        // innerhalb eines EditListTemplates nicht zu funktionieren scheint
        WebUtils.CollectErrorMessages (e.Item, editValidationSummary);
        
        // Daten aus den Eingabe-Feldern in ein neues EditItem übernehmen -> bearbeitete Werte 
        // können somit angezeigt werden
        editItem = new EditPriceItem (e.Item);
      }
    }
    
    /// <summary>
    /// Änderungen zurücknehmen.
    /// </summary>
    private void CancelPriceCmd (Object sender, DataListCommandEventArgs e) 
    {
      log.Debug ("AdminHouseForm.CancelPriceCmd");
      
      // ggf. ungültiges Item löschen (können nur neue Items sein)
      RemoveInvalidItem ();
      
      priceList.EditItemIndex = -1;
    }
    
    /// <summary>
    /// Fügt ein neues Item in die Liste ein und bereitet das DataList-Control für den EditModus vor.
    /// </summary>
    private void InsertPriceCmd (Object sender, EventArgs e)
    {
      log.Debug ("AdminHouseForm.InsertPriceCmd");
      
      IList prices = (IList)priceList.DataSource;
      
      // neues HousePriceInterval erstellen und der Liste hinzufügen -> zunächst als ungültig markiert
      HousePriceInterval newInterval = new HousePriceInterval (
        DateTime.Now, DateTime.Now, -1, false, BookingDays.ALL_DAYS, BookingDays.ALL_DAYS, 1);
      newInterval.Valid = false;
      prices.Add (newInterval);
      
      // EditIndex setzen auf das neue Item
      priceList.EditItemIndex = prices.Count - 1;
            
      // leeres Edit-Item erstellen
      editItem = new EditPriceItem ();     
    }
    
    /// <summary>
    /// Prüft, ob alle Tage des Jahres in den erfassten Preisintervallen enthalten sind.
    /// Ist dies nicht so, so wird eine Warnung angezeigt.
    /// </summary>
    private void CheckIntervalCompleteness ()
    {
      log.Debug ("AdminHouseForm.CheckIntervalCompleteness");

      bool editModus = priceList.EditItemIndex >= 0;

      // Warnung nur zeigen, wenn keine Preise bearbeitet werden
      intervalWarning.Visible = ! AdminHouseController.IsAllIntervalsContainsAllDays (SelectedYear) && ! editModus; 
    }
      
    #endregion
  
    #region Controller Code
  
    private AdminHouseController AdminHouseController
	  {	  
	    get {
        AdminHouseController ctrl = (AdminHouseController)Session["AdminHouseController"];
        if (ctrl == null)
        {
          ctrl = new AdminHouseController();
          Session["AdminHouseController"] = ctrl;
        }

        return ctrl;
      }
	  }

	  /// <summary>
    /// Setzt die Daten des Formulars in das übergebene Bean.
    /// </summary>
	  public void GetHouseData (HouseBean house)
    {
      log.Debug ("AdminHouseForm.GetHouseData");
      
      house.Location = location.Text;
      house.CleaningCost = Int32.Parse (cleaningCost.Text);
      house.MinDaysSeason = Int32.Parse (minDaysSeason.Text);
      house.MinDaysTotal = Int32.Parse (minDaysTotal.Text);
      house.BedClothesHirable = bedClothes.Checked;
    }
     
    #endregion
    
    #region sonstige Methoden
    
    /// <summary>
    /// Füllt die Auswahlliste 'Jahr' mit gültigen Jahren.
    /// </summary>
    private void FillYearSelect ()
    {
      log.Debug ("AdminHouseForm.FillYearSelect");
    
      DateTime now = DateTime.Now;
      int currentYear = now.Year;
      
      yearSelect.Items.Add ((--currentYear).ToString ());
      yearSelect.Items.Add ((++currentYear).ToString ());
      yearSelect.Items.Add ((++currentYear).ToString ());
      yearSelect.Items.Add ((++currentYear).ToString ());
      yearSelect.Items.Add ((++currentYear).ToString ());
      
      yearSelect.SelectedIndex = 1;
    }
    
    /// <summary>
    /// Löscht ggf. ein ungültiges neues Preis-Intervall aus der Liste der Preise,
    /// und liefert den Index, von der es entfernt wurde.
    /// </summary>
    private int RemoveInvalidItem ()
    {
      log.Debug ("AdminHouseForm.RemoveInvalidItem");
    
      IList prices = (IList)priceList.DataSource;
      HousePriceInterval invalidItem = null;
      int itemIndex = -1;
      
      int index = 0;
      foreach (HousePriceInterval interval in prices) {
        if (! interval.Valid) {
          invalidItem = interval;
          itemIndex = index;
          break;
        }
        index ++;
      }
      
      if (invalidItem != null) {
        prices.Remove (invalidItem);
      }
      
      return itemIndex;
    }
    
    #endregion
  }
  
  public class EditPriceItem
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(EditPriceItem));

    public String start;
    public String end;
    public String price;
    public bool peakSeason;
    public BookingDays arrivalDays;
    public BookingDays departureDays;
    public String minBookingDays;
    
    public EditPriceItem ()
    {
      arrivalDays = new BookingDays ();
      arrivalDays.Add (DayOfWeek.Saturday);
      arrivalDays.Add (DayOfWeek.Sunday);

      departureDays = new BookingDays ();
      departureDays.Add (DayOfWeek.Saturday);
      departureDays.Add (DayOfWeek.Sunday);
    }
    
    public EditPriceItem (DataListItem item)
    {
      this.start = ((TextBox)item.FindControl ("editStart")).Text;
      this.end = ((TextBox)item.FindControl ("editEnd")).Text;
      this.price = ((TextBox)item.FindControl ("editPrice")).Text;
      this.peakSeason = ((CheckBox)item.FindControl ("editPeakSeason")).Checked;
      this.minBookingDays = ((TextBox)item.FindControl ("editMinBookingDays")).Text;

      // prüfen, welche Anreise-Tage ausgewählt wurden -> erzeugen eines neuen BookingDays-Objektes
      ListItemCollection items = ((CheckBoxList)item.FindControl ("arrivalDaysList")).Items;
      this.arrivalDays = new BookingDays ();
      foreach (ListItem listItem in items) {
        if (listItem.Selected) {
          String[] tokens = listItem.Value.Split('_');
          arrivalDays.Add ((DayOfWeek)Int32.Parse (tokens[1]));
        }
      }

      // prüfen, welche Abreise-Tage ausgewählt wurden -> erzeugen eines neuen BookingDays-Objektes
      items = ((CheckBoxList)item.FindControl ("departureDaysList")).Items;
      this.departureDays = new BookingDays ();
      foreach (ListItem listItem in items) {
        if (listItem.Selected) {
          String[] tokens = listItem.Value.Split('_');
          departureDays.Add ((DayOfWeek)Int32.Parse (tokens[1]));
        }
      }
    }
    
    public EditPriceItem (HousePriceInterval priceInterval)
    {
      this.start = priceInterval.Start.ToShortDateString ();
      this.end = priceInterval.End.ToShortDateString ();
      this.price = priceInterval.Price.ToString ();
      this.peakSeason = priceInterval.PeakSeason;
      this.minBookingDays = priceInterval.MinBookingDays.ToString ();
      this.arrivalDays = priceInterval.ArrivalDays;
      this.departureDays = priceInterval.DepartureDays;
    }
    
    public static void Update (DataListItem item, HousePriceInterval priceInterval)
    {
      EditPriceItem priceItem = new EditPriceItem (item);
      priceInterval.Start = Convert.ToDateTime (priceItem.start);
      priceInterval.End = Convert.ToDateTime (priceItem.end);
      priceInterval.Price = Convert.ToInt32 (priceItem.price);
      priceInterval.PeakSeason = priceItem.peakSeason;
      priceInterval.MinBookingDays = Convert.ToInt32 (priceItem.minBookingDays);
      priceInterval.ArrivalDays = priceItem.arrivalDays;
      priceInterval.DepartureDays = priceItem.departureDays;
    }
    
    public static HousePriceInterval CreatePriceInterval (DataListItem item)
    {
      EditPriceItem priceItem = new EditPriceItem (item);
      return new HousePriceInterval (
        Convert.ToDateTime (priceItem.start),
        Convert.ToDateTime (priceItem.end),
        Convert.ToInt32 (priceItem.price),
        priceItem.peakSeason,
        priceItem.arrivalDays,
        priceItem.departureDays,
        Convert.ToInt32 (priceItem.minBookingDays)
      );
    }
  }
}

