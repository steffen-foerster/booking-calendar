using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Threading;
using de.fiok.service;
using log4net;

public partial class aspx_calendar_calendar : System.Web.UI.Page
{
  private static readonly ILog log = LogManager.GetLogger(typeof(aspx_calendar_calendar));

  private static Mutex mutex = new Mutex();

  protected ArrayList dates;
  protected int houseId = -1;
  protected String bgColor;
  protected String borderColor;
  protected bool showPromotion; 

  protected void Page_Load (object sender, EventArgs e)
  {
    // gleichzeitiger Aufruf des Belegungskalenders führt zu Fehlern
    // z.B. beim Dateizugriff während der Initialisierung der Themes
    // OLEDB-Exceptions usw.
    mutex.WaitOne ();

    try {
      // Ferienhaus laden
      HouseService hService = HouseService.GetInstance ();

      String domain = Context.Request.Params["domain"];
      String id = Context.Request.Params["objectId"];

      log.Warn("Calendar for referrer:" + GetReferrerString() + " - domain:" + domain + ", id:" + id);

      InitDates ();
      InitCustomStyles ();

      if (!String.IsNullOrEmpty (domain)) {
        if (String.IsNullOrEmpty (id)) {
          try {
            houseId = hService.RetrieveHouseIdByDomain (domain);
          }
          catch (Exception ex) {
            log.Error(ex + " domain = " + domain + ", referrer = " + GetReferrerString (), ex);
          }
        }
        else {
          try {
            houseId = hService.RetrieveHouseIdByDomain(domain, Int32.Parse(id));
          }
          catch (Exception ex) {
            log.Error(ex + " - domain:" + domain + ", id:" + id + ", referrer:" + GetReferrerString(), ex);
          }
        }

        showPromotion = false;
        setPromotionFlag (domain);
      }

      log.Debug ("domain = " + domain + ", houseId = " + houseId);

      this.DataBind();
    }
    catch (Exception ex) {
      log.Error (ex, ex);
    }
    finally {
      mutex.ReleaseMutex ();
    }
  }

  private void InitDates ()
  {
    dates = new ArrayList();

    DateTime startDate = DateTime.Now;

    // ggf. bestimmtes Jahr anzeigen
    try {
      String yearStr = Context.Request.Params["year"];
      int year = Int32.Parse (yearStr);
      startDate = new DateTime (year, 1, 1);
    }
    catch (Exception e) {
      // ignore
    }

    // je ein Datum für die nächsten 12 Monate in der Liste dates speichern
    for (int i = 0; i < 12; i++)
    {
      dates.Add (startDate);
      startDate = startDate.AddMonths (1);
    }
  }

  private void InitCustomStyles()
  {
    bgColor = "#FFFFFF";
    borderColor = "#FFFFFF";

    // ggf. die Style-Vorgaben ändern
    try
    {
      String bgColorReq = Context.Request.Params["bgColor"];
      if (! String.IsNullOrEmpty (bgColorReq)) {
        bgColor = bgColorReq;
      }

      String borderColorReq = Context.Request.Params["borderColor"];
      if (!String.IsNullOrEmpty(borderColorReq))
      {
        borderColor = borderColorReq;
      }
    }
    catch (Exception e)
    {
      // ignore
    }
  }

  private string GetReferrerString ()
  {
    try {
      Uri referrer = Request.UrlReferrer;
      if (referrer != null) {
        return Request.UrlReferrer.AbsoluteUri;
      }
      return "no referrer";
    }
    catch (Exception e) {
      log.Error (e, e);
      return "error get the referrer";
    }
  }

  private void setPromotionFlag (String domain)
  {
    showPromotion = false;
  }
}
