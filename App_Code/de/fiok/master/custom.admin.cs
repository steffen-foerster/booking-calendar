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
using log4net;

namespace de.fiok.master
{
  public partial class CustomAdminMaster : System.Web.UI.MasterPage
  {
    private static readonly ILog log = LogManager.GetLogger (typeof (CustomAdminMaster));

    private String nextNavigation;

    public String NextNavigation
    {
      get
      {
        log.Debug ("read CustomAdminMaster.NextNavigation");
        return nextNavigation;
      }
      set
      {
        log.Debug ("write CustomAdminMaster.NextNavigation");
        nextNavigation = value;
        log.Debug ("nextNavigationTask = " + nextNavigation);
      }
    }

    protected void Page_Load (object sender, EventArgs e)
    {
    }
  }
}
