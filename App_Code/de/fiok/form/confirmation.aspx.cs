namespace de.fiok.form
{
  using System;
  using System.Web.UI;
  using System.Web.UI.HtmlControls;
  using System.Web.UI.WebControls;
  using System.Collections;
  using de.fiok.service;
  using de.fiok.controller;
  using de.fiok.controls;
  using de.fiok.state;
  using de.fiok.core;
  using log4net;

  /// <summary>
  /// Code-Behind Klasse für die Seite confirmation.aspx, auf der die Reservierungsbestätigung
  /// angezeigt wird.
  /// </summary>
  /// <remarks>
  /// created by - Steffen Förster
  /// </remarks>
  public class ConfirmationForm : BaseForm
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(ConfirmationForm));

    #region Properties

    protected HtmlButton btnPreviousPage;
    protected HtmlButton btnNextPage;
    protected LinkButton btnHomePage;
    protected TextBox notes;
    protected CheckBox bedClothes;
    protected UIMessage uiMsgBooking;
    protected BookingData bookingData;
    protected TenantEntryData tenantData;

    public BookingData BookingData
    {
      set { bookingData = value; }
    }

    public TenantEntryData TenantData
    {
      set { tenantData = value; }
    }

    public String Notes
    {
      get { return notes.Text; }
      set { notes.Text = value; }
    }

    #endregion

    #region Initialize Page

    override protected void OnInit(EventArgs e)
    {
      log.Debug("ConfirmationForm.OnInit");

      InitializeComponent();
      base.OnInit(e);

    }

    /// <summary>
    /// Komponenten initialisieren.
    /// </summary>
    private void InitializeComponent()
    {
      log.Debug("ConfirmationForm.InitializeComponent");

      this.btnNextPage.ServerClick += new System.EventHandler(this.NextSite);
      this.btnPreviousPage.ServerClick += new System.EventHandler(this.PreviousSite);
      this.Load += new System.EventHandler(this.PageLoad);
      this.PreRender += new System.EventHandler(this.PreRenderHandler);
    }

    #endregion

    #region Eventhandler

    /// <summary>
    /// Selektierten Zeitraum und persönliche Daten aus dem State holen.
    /// </summary>
    private void PageLoad(Object sender, EventArgs e)
    {
      log.Debug("ConfirmationForm.PageLoad");

      BookingController.PrepareConfirmation(this);
    }

    /// <summary>
    /// Daten werden an die Controls gebunden.
    /// </summary>
    private void PreRenderHandler(Object sender, EventArgs e)
    {
      log.Debug("ConfirmationForm.PreRenderHandler");

      this.DataBind();
    }

    /// <summary>
    /// Reservierung speichern und Bestätigungs-Mail senden.
    /// </summary>
    public void NextSite(Object sender, EventArgs e)
    {
      log.Debug("ConfirmationForm.NextSite");

      MessageResult result = BookingController.FinishBooking(this);

      // Reservierung war erfolgreich
      if (result.Result)
      {
        uiMsgBooking.InfoMessage = result.Message;

        btnPreviousPage.Visible = false;
        btnNextPage.Visible = false;
      }
      // Reservierung ist fehlgeschlagen
      else
      {
        uiMsgBooking.ErrorMessage = result.Message;
      }
    }

    /// <summary>
    /// Zurück zur vorherigen Seite.
    /// </summary>
    public void PreviousSite(Object sender, EventArgs e)
    {
      log.Debug("ConfirmationForm.PreviousSite");

      BookingController.BackToTenantEntry(this);
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
    /// Setzt die Daten des Formulars in das übergebene Bean.
    /// </summary>
    public void GetData(TenantEntryData data)
    {
      log.Debug("ConfirmationForm.GetData");

      data.Notes = notes.Text;
      data.BedClothes = bedClothes.Checked;
    }

    #endregion
  }
}

