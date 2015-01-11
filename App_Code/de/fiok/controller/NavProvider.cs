namespace de.fiok.controller
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Web;
  using log4net;
  using System.Collections;

  /// <summary>
  /// Stellt die Funktionalität für die Navigation zwischen den einzelnen 
  /// Seiten der Anwendung bereit.
  /// </summary>
  public class NavProvider
  {
    #region öffentliche Konstanten
    public static readonly String PAGE_ADMIN_BOOKING = "adminBooking";

    public static readonly String PAGE_ADMIN_HOUSE = "adminHouse";

    public static readonly String PAGE_BOOKING = "booking";

    public static readonly String PAGE_TENANT = "tenant";

    public static readonly String PAGE_CONFIRM = "confirm";

    public static readonly String FLOW_BOOKING = "booking";

    public static readonly String FLOW_ADMIN = "admin";
    #endregion

    private static readonly String PAGE_ID = "nav.pageId";

    private static readonly String FLOW_ID = "nav.flowId";

    private static readonly NavProvider INSTANCE = new NavProvider();

    private static readonly ILog log = LogManager.GetLogger(typeof(NavProvider));

    private Hashtable pageToUrl;

    private Hashtable pageToFile;

    private NavProvider()
    {
      pageToUrl = new Hashtable();
      pageToUrl[PAGE_ADMIN_BOOKING] = "~/aspx/admin/adminBooking.aspx";
      pageToUrl[PAGE_ADMIN_HOUSE] = "~/aspx/admin/adminHouse.aspx";
      pageToUrl[PAGE_BOOKING] = "~/aspx/booking.aspx";
      pageToUrl[PAGE_TENANT] = "~/aspx/tenantEntry.aspx";
      pageToUrl[PAGE_CONFIRM] = "~/aspx/confirmation.aspx";

      pageToFile = new Hashtable();
      pageToFile[PAGE_ADMIN_BOOKING] = "adminBooking.aspx";
      pageToFile[PAGE_ADMIN_HOUSE] = "adminHouse.aspx";
      pageToFile[PAGE_BOOKING] = "booking.aspx";
      pageToFile[PAGE_TENANT] = "tenantEntry.aspx";
      pageToFile[PAGE_CONFIRM] = "confirmation.aspx";
    }

    public static NavProvider Instance {
      get { return INSTANCE; }
    }

    public Boolean IsFlowActive
    {
      get
      {
        String flowId = (String)HttpContext.Current.Session[FLOW_ID];
        String pageId = (String)HttpContext.Current.Session[PAGE_ID];

        return flowId != null && pageId != null;
      }
    }

    /// <summary>
    /// Prüft, ob die in der Session gespeicherte Seite auch tatsächlich aufgerufen wird.
    /// Wenn nein, wird diese Seite automatisch über eine Redirekt aufgerufen.
    /// </summary>
    public Boolean CheckPageFlow()
    {
      String pageId = (String)HttpContext.Current.Session[PAGE_ID];
      String file = (String)pageToFile[pageId];

      Boolean valid = HttpContext.Current.Request.Path.EndsWith(file);
      if (!valid)
      {
        log.Debug("Invalid request path - pageId:" + pageId + ", path:" + HttpContext.Current.Request.Path);
        NavigateToTarget(pageId);
        return false;
      }

      return true;
    }

    /// <summary>
    /// Ruft eine neue Seite auf.
    /// </summary>
    public void NavigateToTarget(String target)
    {
      log.Debug("AdminBookingController.NavigateToTarget");

      String flowId = (String)HttpContext.Current.Session[FLOW_ID];

      if (flowId.Equals(FLOW_ADMIN))
      {
        NavigateFlowAdmin(target);
      }
      else if (flowId.Equals(FLOW_BOOKING))
      {
        NavigateFlowBooking(target);
      }
    }

    /// <summary>
    /// Startet mit einer neuen Seiten-Navigation.
    /// </summary>
    public void StartNavigation(String flow)
    {
      if (FLOW_ADMIN.Equals(flow))
      {
        HttpContext.Current.Session[FLOW_ID] = FLOW_ADMIN;
        NavigateToTarget(PAGE_ADMIN_BOOKING);
      }
      else if (FLOW_BOOKING.Equals(flow))
      {
        HttpContext.Current.Session[FLOW_ID] = FLOW_BOOKING;
        NavigateToTarget(PAGE_BOOKING);
      }
    }


    /// <summary>
    /// Task beenden, indem alle Navigationsdaten aus dem State entfernt werden.
    /// </summary>
    public void CompleteNavigation()
    {
      HttpContext.Current.Session.Remove(FLOW_ID);
      HttpContext.Current.Session.Remove(PAGE_ID);
    }

    /// <summary>
    /// Navigiert zu einer Seite, von der aus die Buchung gestartet wurde.
    /// Gleichzeitig wird der der Flow beendet.
    /// </summary>
    public void NavigateHome()
    {
      CompleteNavigation();
      HttpContext.Current.Response.Redirect("~/aspx/exitBooking.aspx", true);
    }

    #region private section

    /// <summary>
    /// Steuert den Seitenablauf innerhalb des Admin-Bereiches.
    /// </summary>
    private void NavigateFlowAdmin(String target)
    {
      if (PAGE_ADMIN_BOOKING.Equals(target) || PAGE_ADMIN_HOUSE.Equals(target))
      {
        Redirect(target);
      }
    }

    /// <summary>
    /// Steuert den Seitenablauf innerhalb der Buchungsseite.
    /// </summary>
    private void NavigateFlowBooking(String target)
    {
      String oldPageId = (String)HttpContext.Current.Session[PAGE_ID];

      if (PAGE_BOOKING.Equals(target))
      {
        // Buchungsstart nur zu Beginn aufrufen und wenn der Besucher von der Mieterseite kommt
        if (oldPageId == null || PAGE_TENANT.Equals(oldPageId) || PAGE_BOOKING.Equals(oldPageId))
        {
          Redirect(target);
        }
      }
      else if (PAGE_TENANT.Equals(target))
      {
        // Mieterseite nur aufrufen, wenn der Besucher vom Buchungsstart oder der Bestätigungsseite kommt
        if (PAGE_BOOKING.Equals(oldPageId) || PAGE_CONFIRM.Equals(oldPageId) || PAGE_TENANT.Equals(oldPageId))
        {
          Redirect(target);
        }
      }
      else if (PAGE_CONFIRM.Equals(target))
      {
        // Bestätigungsseite nur aufrufen, wenn der Besucher von der Mieterseite kommt
        if (PAGE_TENANT.Equals(oldPageId) || PAGE_CONFIRM.Equals(oldPageId))
        {
          Redirect(target);
        }
      }
    }

    private void Redirect(String target)
    {
      HttpContext.Current.Session[PAGE_ID] = target;
      HttpContext.Current.Response.Redirect((String)pageToUrl[target], true);
    }

    #endregion
  }
}