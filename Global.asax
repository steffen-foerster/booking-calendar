<%@ Application language="C#" %>
<%@ Import namespace="de.fiok.core" %>
<%@ Import namespace="de.fiok.log" %>
<%@ Import namespace="de.fiok.service" %>
<%@ Import namespace="de.fiok.web" %>
<%@ Import namespace="de.fiok.type" %>
<%@ Import namespace="log4net" %>
<%@ Import namespace="System.Threading" %>

<script runat="server">

    private static readonly ILog log = LogManager.GetLogger("Global.asax");

    public void Application_Start(Object sender, EventArgs e)
    {
      // log4net initialisieren
      LogConfig.ConfigLog4net (Server.MapPath("~"));
      
      // Datenbank initialisieren
      DatabaseService.InitService (Server.MapPath("~"));

      // CacheHandler initialisieren
      CacheHandler.Init(Context.Cache);
    }

    public void Application_End(Object sender, EventArgs e) 
    {
    }

    public void Application_Error(Object sender, EventArgs e)
    {
      using (NDC.Push ("request path: " + Context.Request.Path + 
        ", host name: " + Context.Request.UserHostName + 
        ", user agent: " + Context.Request.UserAgent))
      {
        // Code that runs when an unhandled error occurs
        Exception lastError = Server.GetLastError();
        log.Error ("application error: " + lastError.Message, lastError);
        if (lastError.InnerException != null) {
          log.Error ("inner exception: " +  lastError.InnerException.Message, lastError.InnerException);
        }
      }
    }

    public void Session_Start(Object sender, EventArgs e)
    {
      log.Debug ("start new session - ID = " + Session.SessionID);
      log.Debug ("Session.IsNewSession = " + Session.IsNewSession);
    }

    public void Session_End(Object sender, EventArgs e) 
    { 
      log.Debug ("end session - ID = " + Session.SessionID);
    }
    
    public void Application_BeginRequest (Object sender, EventArgs e)
    {
      log.Debug ("start request: " + Request.Path);

      // Locale auf Deutsch setzen
      Locale.SetDefaultLocale ();
    }

    public void Application_PostAcquireRequestState (Object sender, EventArgs e)
    {
      log.Debug("Application_PostAcquireRequestState");

      // Browser Cache deaktivieren
      Response.Cache.SetCacheability(HttpCacheability.NoCache);
      
      // Session-Inhalt löschen -> neuer Start
      if (Request.Path.EndsWith("bookingStart.aspx") || Request.Path.EndsWith("adminStart.aspx"))
      {
        Session.Clear();
      }

      // Prüfen, ob houseId richtig gesetzt ist
      if (Request.Path.EndsWith("bookingStart.aspx"))
      {
        String houseIdStr = Request.Params["houseId"];
        HouseBean houseBean = null;
        
        if (! String.IsNullOrEmpty (houseIdStr)) {
          try {
            houseIdStr = houseIdStr.Trim ();
            int houseId = Int32.Parse (houseIdStr);
            houseBean = HouseService.GetInstance().RetrieveHouse(houseId);
          }
          catch (Exception ex) {
            log.Warn ("cannot add houseId to session", ex);
          }    
        }
        
        if (houseBean == null) {
          Context.Response.Redirect (ConfigurationManager.AppSettings["error_house_init_page"]);
        }
        else {
          Session["houseId"] = houseIdStr;
        }
      }
      
      if (Request.Path.EndsWith(".aspx"))
      {
        try
        {
          // Rollen zum aktuellen Thread hinzufügen
          if (User.Identity.IsAuthenticated)
          {
            LandlordBean landlord = LandlordService.GetInstance().RetrieveLandlord(Int32.Parse(User.Identity.Name));
            if (landlord != null)
            {
              LandlordRoleProvider.AddRoles(landlord);
            }
          }
        }
        catch (Exception ex)
        {
          log.Warn(ex, ex);
        }
      }
    }
  
    public void Application_EndRequest (Object sender, EventArgs e)
    {
      // Rollen vom aktuelle Thread entfernen
      if (Request.Path.EndsWith(".aspx"))
      {
        LandlordRoleProvider.RemoveRoles ();
      } 
    }

</script>
