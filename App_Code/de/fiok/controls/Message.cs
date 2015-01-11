namespace de.fiok.controls
{
  using System;
  using System.Web;
  using System.Web.UI;
  using System.Collections;
  using de.fiok.core;
  using log4net;

  /// <summary>
  /// Control zur Ausgabe eines lokalisierten Textes aus der Resource-Datei.
  /// </summary>
  /// <remarks>
  /// created by - Steffen Förster  
  /// </remarks>
  public class Message : Control
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(Message));

    #region Properties

    private String msg = "???";
    private String key;
    private ArrayList argsList = new ArrayList();

    public object Arg0
    {
      set
      {
        log.Debug ("setting arg0 = " + value);
        argsList.Add(value);
      }
    }

    public object Arg1
    {
      set
      {
        argsList.Add(value);
      }
    }

    public String Key
    {
      set { key = value; }
    }

    #endregion

    /// <summary>
    /// Ausgabe des lokalisierten Textes der anhand des Keys gefunden wurde.
    /// </summary>
    protected override void Render(HtmlTextWriter html)
    {
      msg = AppResources.GetMessage(key);
      if (msg == null)
      {
        msg = "???" + key + "???";
      }
      else if (argsList.Count > 0)
      {
        object[] args = argsList.ToArray();
        for (int i = 0; i < args.Length; i++) {
          log.Debug ("arg = " + args[i]);
        }
        msg = String.Format(msg, args);
      }
      html.Write(msg);
    }
  }
}
