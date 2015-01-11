namespace de.fiok.controls
{
  using System;
  using System.Collections;
  using System.Web.UI;
  using System.Web.UI.WebControls;
  using de.fiok.web;
  using log4net;

  /// <summary>
  /// Das Control zeigt formatierte Meldungen an.
  /// </summary>
  public class UIMessage : WebControl
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(UIMessage));

    private String message;
    private String imageName;
    private String imagePath;
    private bool skipRendering;

    public String ImagePath
    {
      set
      {
        imagePath = value;
      }
    }

    public String InfoMessage
    {
      set
      {
        message = value;
        CssClass = "ui_info_msg";
        imageName = "information.png";
      }
    }

    public String WarnMessage
    {
      set
      {
        message = value;
        CssClass = "ui_warn_msg";
        imageName = "warning.png";
      }
    }

    public String ErrorMessage
    {
      set
      {
        message = value;
        CssClass = "ui_error_msg";
        imageName = "error.png";
      }
    }

    public override void RenderBeginTag(HtmlTextWriter writer)
    {
      log.Debug("UIMessage.RenderBeginTag");

      if (message == null || message == String.Empty)
      {
        skipRendering = true;
        return;
      }

      writer.WriteBeginTag("div");
      writer.WriteAttribute("class", this.CssClass);
      writer.WriteAttribute("style", WebUtils.BuildStyleString(this.Style));
      writer.Write(HtmlTextWriter.TagRightChar);
      writer.WriteLine();
    }

    protected override void RenderContents(HtmlTextWriter writer)
    {
      log.Debug("UIMessage.RenderContents");

      if (skipRendering)
      {
        return;
      }

      writer.Write("<img src='" + imagePath + "/" + imageName + "' class='normal'/>");
      writer.WriteLine();
      writer.Write("&nbsp;" + message);
      writer.WriteLine();
    }

    public override void RenderEndTag(HtmlTextWriter writer)
    {
      log.Debug("UIMessage.RenderEndTag");

      if (skipRendering)
      {
        return;
      }
      writer.WriteLine();
      writer.WriteEndTag("div");
      writer.WriteLine();
    }
  }
}
