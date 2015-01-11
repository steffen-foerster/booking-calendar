namespace de.fiok.form
{
  using System;
  using System.Configuration;
  using System.Web;
  using de.fiok.web;
  using de.fiok.controls;
  using de.fiok.controller;
  using log4net;
  using System.Web.UI;

  /// <summary>
  /// Basis-Klasse für alle Code-Behind-Klassen.
  /// </summary>
  /// <remarks>
  /// created by - Steffen Förster  
  /// </remarks>
  public abstract class BaseForm : Page
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(BaseForm));
    
    protected TimeoutDetection timeout;
    
    protected override void OnLoad (EventArgs e)
    {
      // Testen, ob eine Seite innerhalb eines Navigationsgraphen aufgerufen werden soll,
      // nachdem bereits ein Session-Timeout aufgetreten ist.
      //UserBean user = (UserBean)Session[WebConst.SESSION_USER_BEAN];
      if (timeout == null) {
        log.Warn ("no timeout detection for page: " + Request.Path);
      }
      else if (! timeout.IsValidSession ()) {
        log.Debug ("session timeout");
        Context.Response.Redirect (ConfigurationManager.AppSettings ["error_timeout_page"]);
        return;
      }     
    
      // Testen, ob eine View aufgerufen werden soll, obwohl kein Navigationsgraph initialisiert wurde
      if (! NavProvider.Instance.IsFlowActive) {
        log.Debug ("flow not active");
        Context.Response.Redirect(ConfigurationManager.AppSettings["error_cookie_page"]);
        return;
      }

      // Test, ob eine andere Seite aufgerufen wird, als die mit der aktuellen Page-ID verbunden ist
      // -> Benutzer hat die Navigationsbuttons des Browsers verwendet oder einen Link aufgerufen
      // -> Redirekt ruft dann ursprüngliche Seite auf
      if (! NavProvider.Instance.CheckPageFlow())
      {
        log.Debug("invalid page");
        return;
      }
     
      base.OnLoad (e);
    }
    
    /// <summary>
    /// Zur Homepage zurückspringen.
    /// </summary>
    protected void HomePage (Object sender, EventArgs e)
    {
      log.Debug ("BaseForm.HomePage");

      NavProvider.Instance.NavigateHome();
    }
  }
}
