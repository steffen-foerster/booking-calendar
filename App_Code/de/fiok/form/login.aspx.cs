namespace de.fiok.form
{
  using System;
  using System.Web.UI;
  using System.Web.Security;
  using System.Web.UI.HtmlControls;
  using System.Web.UI.WebControls;
  using de.fiok.service;
  using de.fiok.core;
  using de.fiok.controls;
  using de.fiok.web;
  using log4net;
  
  /// <summary>
  /// Code-Behind Klasse für die Seite login.aspx.
  /// </summary>
  /// <remarks>
	/// created by - Steffen Förster
	/// </remarks>
  public class LoginForm : Page
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(LoginForm));
    private static readonly HouseService houseService = HouseService.GetInstance ();
    private static readonly LandlordService landlordService = LandlordService.GetInstance ();
    
    #region Properties
    
    protected TextBox email;
    protected TextBox password;
    protected HtmlButton btnLogin;
    protected UIMessage uiMsgLogin;
    
    #endregion
    
    #region Initialize Page
    
    override protected void OnInit (EventArgs e)
    {
      log.Debug ("LoginForm.OnInit");
      
      InitializeComponent();
      base.OnInit(e);
    }

    /// <summary>
    /// Komponenten initialisieren.
    /// </summary>
    private void InitializeComponent ()
    {
      log.Debug ("LoginForm.InitializeComponent");
    
		  this.btnLogin.ServerClick += new System.EventHandler (this.Login);
			this.Load += new System.EventHandler (this.PageLoad);
			this.PreRender += new System.EventHandler (this.PreRenderHandler);
	  }
    
    #endregion
  
    #region Eventhandler
    
    /// <summary>
    /// Seite wurde geladen.
    /// </summary>
    private void PageLoad (Object sender, EventArgs e)
    {
      log.Debug ("LoginForm.PageLoad");      
    }
    
    /// <summary>
    /// Daten werden an die Controls gebunden.
    /// </summary>
    private void PreRenderHandler (Object sender, EventArgs e)
    {
      log.Debug ("LoginForm.PageInit");
      
      this.DataBind ();
    }
    
    /// <summary>
    /// Login durchführen.
    /// </summary>
    public void Login (Object sender, EventArgs e)
    {
      log.Debug ("LoginForm.Login");

      if (this.IsValid) {
        LandlordBean landlord = landlordService.RetrieveLandlordByCredentials (email.Text, password.Text);
        
        if (landlord == null) {
          uiMsgLogin.ErrorMessage = AppResources.GetMessage("Login_err.unknown.user");
        }
        else {
          FormsAuthentication.RedirectFromLoginPage (landlord.ID.ToString (), false);
        }
      }
    }
    
    #endregion
  }
}

