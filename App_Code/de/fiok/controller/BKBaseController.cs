namespace de.fiok.controller
{
  using System;
  using System.Web.SessionState;
  using de.fiok.service;
  using de.fiok.core;
  using log4net;
  using System.Collections;
  using System.Web;

  /// <summary>
  /// Basis-Klasse für alle Controller.
  /// </summary>
  public class BKBaseController
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(BKBaseController));

    /// <summary>
    /// Konstruktor.
    /// </summary>
    public BKBaseController ()
    {
    }

    /// <summary>
    /// Fügt einen Wert zum Session-State hinzu.
    /// </summary>
    public HttpSessionState State
    {
      get
      {
        return HttpContext.Current.Session;
      }
    }
  }
}
