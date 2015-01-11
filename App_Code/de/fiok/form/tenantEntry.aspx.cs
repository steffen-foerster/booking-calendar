namespace de.fiok.form
{
  using System;
  using System.Web.UI;
  using System.Web.UI.HtmlControls;
  using System.Web.UI.WebControls;
  using System.Collections;
  using de.fiok.service;
  using de.fiok.controller;
  using de.fiok.state;
  using de.fiok.core;
  using de.fiok.type;
  using log4net;
  
  /// <summary>
  /// Code-Behind Klasse für die Seite tenantEntry.aspx, auf der die persönlichen Daten des 
  /// Mieters erfasst werden.
  /// </summary>
  /// <remarks>
	/// created by - Steffen Förster
	/// </remarks>
  public class TenantEntryForm : BaseForm
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(TenantEntryForm));
    
    #region Properties
    
    protected TextBox name;
    protected TextBox firstname;
    protected TextBox street;
    protected TextBox zipcode;
    protected TextBox location;
    protected TextBox title;
    protected TextBox telephone;
    protected TextBox email;
    protected TextBox fax;
    protected TextBox ageChildren;
    protected DropDownList adultCountSelect;
    protected DropDownList childrenCountSelect;
    protected DropDownList salutationSelect;
    protected HtmlButton btnPreviousPage;
    protected HtmlButton btnNextPage;
    protected LinkButton btnHomePage;
    protected DropDownList promotionPartnerList;
    protected RequiredFieldValidator ageChildrenValidator;
    
    #endregion
    
    #region Initialize Page
    
    override protected void OnInit (EventArgs e)
    {
      log.Debug ("TenantEntryForm.OnInit");
      
      InitializeComponent();
      base.OnInit(e);
      
    }

    /// <summary>
    /// Komponenten initialisieren.
    /// </summary>
    private void InitializeComponent ()
    {
      log.Debug ("TenantEntryForm.InitializeComponent");
    
		  this.btnNextPage.ServerClick += new System.EventHandler (this.NextSite);
		  this.btnPreviousPage.ServerClick += new System.EventHandler (this.PreviousSite);
			this.Load += new System.EventHandler (this.PageLoad);
			this.PreRender += new System.EventHandler (this.PreRenderHandler);

      // DropDownList Events
      this.childrenCountSelect.SelectedIndexChanged += new System.EventHandler(this.ChildrenCountListChanged); 
			
			// Initialisierung der Liste 'Anrede'
      salutationSelect.Items.Add (new ListItem (
        SalutationType.GetName (Salutation.MR), SalutationType.GetValueAsString (Salutation.MR)));
      salutationSelect.Items.Add (new ListItem (
        SalutationType.GetName (Salutation.MRS), SalutationType.GetValueAsString (Salutation.MRS)));
	  }
    
    #endregion
  
    #region Eventhandler
    
    /// <summary>
    /// Selektierten Zeitraum aus dem State holen, wenn die Seite zum ersten Mal geladen wird.
    /// </summary>
    private void PageLoad (Object sender, EventArgs e)
    {
      log.Debug ("TenantEntryForm.PageLoad");

      if (! IsPostBack) {
        BookingController.PrepareTenantEntry (this);
      }      
    }
    
    /// <summary>
    /// Daten werden an die Controls gebunden.
    /// </summary>
    private void PreRenderHandler (Object sender, EventArgs e)
    {
      log.Debug ("TenantEntryForm.PageInit");
      
      this.DataBind ();
    }

    /// <summary>
    /// Auswahl der Kinder wurde geändert. Sofern Kinder mitanreisen, benötigen wir auch das
    /// Alter der Kinder.
    /// </summary>
    private void ChildrenCountListChanged(Object sender, EventArgs e)
    {
      log.Debug("TenantEntryForm.ChildrenCountListChanged");

      String count = childrenCountSelect.SelectedValue;

      ageChildrenValidator.Enabled = !count.Equals("0");
    }
    
    /// <summary>
    /// Zurück zur nächsten Seite.
    /// </summary>
    public void NextSite (Object sender, EventArgs e)
    {
      log.Debug ("TenantEntryForm.NextSite");
      
      log.Debug ("isValid = " + this.IsValid);
      
      if (this.IsValid) {
        BookingController.FinishTenantEntry (this);
      }
    }
  
    /// <summary>
    /// Zurück zur vorherigen Seite.
    /// </summary>
    public void PreviousSite (Object sender, EventArgs e)
    {
      log.Debug ("TenantEntryForm.PreviousSite");
    
      BookingController.BackToPeriodSelection (this);
    }
    
    #endregion
  
    #region Controller Code
  
    private BookingController BookingController
	  {
      get
      {
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
    /// Liefert die Daten des Formulars als Bean zurück.
    /// </summary>
	  public TenantEntryData GetData ()
    {
      log.Debug ("TenantEntryForm.GetData");
    
      TenantEntryData data = new TenantEntryData ();
      data.Tenant.Name = name.Text;
      data.Tenant.Firstname = firstname.Text;
      data.Tenant.Street = street.Text;
      data.Tenant.Zipcode = zipcode.Text;
      data.Tenant.Location = location.Text;
      data.Tenant.Title = title.Text;
      data.Tenant.Telephone = telephone.Text;
      data.Tenant.Fax = fax.Text;
      data.Tenant.Email = email.Text;
      data.Tenant.Salutation = SalutationType.RetrieveType (salutationSelect.SelectedValue);
      data.AdultCount = Int32.Parse (adultCountSelect.SelectedValue);
      data.ChildrenCount = Int32.Parse (childrenCountSelect.SelectedValue);
      data.PromotionPartner = Int32.Parse(promotionPartnerList.SelectedValue);
      data.AgeChildren = ageChildren.Text;
      
      return data;
    }
    
    /// <summary>
    /// Setzt die Daten des Beans im Formular.
    /// </summary>
	  public void SetData (TenantEntryData data)
    {
      log.Debug ("TenantEntryForm.SetData");
    
      name.Text = data.Tenant.Name;
      firstname.Text = data.Tenant.Firstname;
      street.Text = data.Tenant.Street;
      zipcode.Text = data.Tenant.Zipcode;
      location.Text = data.Tenant.Location;
      title.Text = data.Tenant.Title;
      telephone.Text = data.Tenant.Telephone;
      fax.Text = data.Tenant.Fax;
      email.Text = data.Tenant.Email;
      salutationSelect.SelectedValue = ((Int32)data.Tenant.Salutation).ToString ();
      adultCountSelect.SelectedValue = data.AdultCount.ToString ();
      childrenCountSelect.SelectedValue = data.ChildrenCount.ToString ();
      promotionPartnerList.SelectedValue = data.PromotionPartner.ToString();
      ageChildren.Text = data.AgeChildren;

      // Validator aktivieren/deaktivieren
      ageChildrenValidator.Enabled = !childrenCountSelect.SelectedValue.Equals("0");
    }
     
    #endregion
  }
}

