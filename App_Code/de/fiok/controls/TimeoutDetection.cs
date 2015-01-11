namespace de.fiok.controls
{
  using System;
  using System.Web;
  using System.Web.UI;
  using System.Collections;
  using de.fiok.core;
  using de.fiok.web;
  using log4net;

  /// <summary>
  /// Control zur Feststellung eines Session-Timeouts. Hierzu wird die Session-Id im View-State
  /// gespeichert und bei jedem Page-Request mit der aktuellen Session-Id verglichen.
  /// </summary>
  /// <remarks>
	/// created by - Steffen Förster  
	/// </remarks>
  public class TimeoutDetection : Control
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(TimeoutDetection));
 
    private String sessionId;
 
    /// <summary>
    /// Speichert die aktuelle Session-Id im ViewState.
    /// </summary>
    protected override Object SaveViewState ()
    { 
      log.Debug ("TimeoutDetection.SaveViewState");
      
      Object savedState = Context.Session.SessionID;
      
      log.Debug ("save savedState: " + savedState);
      
      return savedState;
    }
    
    /// <summary>
    /// Läd die Session-Id der Seite aus dem ViewState.
    /// </summary>
    protected override void LoadViewState (Object savedState) 
    {
      log.Debug ("TimeoutDetection.LoadViewState");

      log.Debug ("load savedState: " + savedState);
      
      if (savedState != null) {
        sessionId = (String)savedState;
      }
    }
    
    
    /// <summary>
    /// Diese Methode prüft, ob die aktuelle Session noch gültig ist. Nachdem eine Session
    /// wegen eines Timeouts ungültig geworden ist, verwendet die ASP-Runtime zunächst die alte
    /// Session und ruft auf dieser Session.Abandon() auf. Der aktuelle Request wird dann mit dieser
    /// Session abgearbeitet, die nun aber keine Daten mehr enthält. Es läßt sich feststellen,
    /// dass diese Session bereits ungültig ist, wenn die Session neu ist, dass heißt sie wurde
    /// im aktuellen Request erstellt die Session-ID ist aber identisch mit der Session-ID aus 
    /// dem ViewState dieses Controls, was bei einer wirklich neuen Session nicht sein kann.
    /// </summary>
    public bool IsValidSession () 
    {
      log.Debug ("TimeoutDetection.IsValidSession");
      
      log.Debug ("sessionId: " + sessionId);
      log.Debug ("Session.SessionId: " + Context.Session.SessionID);
      log.Debug ("Context.Session.IsNewSession: " + Context.Session.IsNewSession);
      
      bool result;

      if (sessionId == null) {
        log.Debug ("saved SessionId == null");
        result = true;
      }
      else if (Context.Session.IsNewSession && sessionId.Equals (Context.Session.SessionID)) {
        log.Debug ("saved SessionId equals with current id, but current id is new");
        result = false;
      }
      else {
        result = true;
      }
      
      return result;
    }
  }
}
