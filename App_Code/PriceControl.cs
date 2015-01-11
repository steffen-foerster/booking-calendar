using System;
using System.Data;
using System.Configuration;
using System.Net;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using System.IO;
using log4net;

namespace House.Controls
{
  /// <summary>
  /// Summary description for PriceControl
  /// </summary>
  public class PriceControl : Control
  {
    private static readonly ILog log = LogManager.GetLogger (typeof(PriceControl));

    private String url;
    private String houseId;

    public String Url
    {
      set { this.url = value; }
    }

    public String HouseId
    {
      set { this.houseId = value; }
    }

    /// <summary>
    /// Ausgabe der Preistabelle.
    /// </summary>
    protected override void Render (HtmlTextWriter html)
    {
      html.Write(LoadPrices ());
    }

    private String LoadPrices ()
    {
      HttpWebRequest request = (HttpWebRequest)WebRequest.Create (url + "?houseId=" + houseId);
      request.KeepAlive = false;

      HttpWebResponse response = (HttpWebResponse)request.GetResponse ();
      Stream receiveStream = response.GetResponseStream ();
      StreamReader readStream = new StreamReader (receiveStream, Encoding.GetEncoding ("iso-8859-1"));
      String result = readStream.ReadToEnd ();

      response.Close ();
      readStream.Close ();

      return result;
    }
  }
}