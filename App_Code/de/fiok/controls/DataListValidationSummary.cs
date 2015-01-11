namespace de.fiok.controls
{
  using System;
  using System.Collections;
  using System.Web.UI;
  using System.Web.UI.WebControls;
  using log4net;

  /// <summary>
  /// Zeigt die Validator-Messages zu allen Validators eines DataList-EditItemTemplates an.
  /// </summary>
  public class DataListValidationSummary : WebControl
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(DataListValidationSummary));

    private IList errorMessages;
    private IList errorControls;
    private String headerText;
    private bool skipRendering;

    public IList ErrorMessages
    {
      set { errorMessages = value; }
    }

    public IList ErrorControls
    {
      set { errorControls = value; }
    }

    public void AddErrorMessage(String message)
    {
      if (errorMessages == null)
      {
        errorMessages = new ArrayList();
      }
      errorMessages.Add(message);
    }

    public void AddErrorControl(String control)
    {
      if (errorControls == null)
      {
        errorControls = new ArrayList();
      }
      errorControls.Add(control);
    }

    public String HeaderText
    {
      set
      {
        log.Debug("DataListValidationSummary.HeaderText");
        headerText = value;
      }
    }

    public String GetErrorStyle(String id)
    {
      if (errorControls != null)
      {
        foreach (String clientId in errorControls)
        {
          if (id.EndsWith(clientId))
          {
            return "border:1px solid red;";
          }
        }
      }

      return "";
    }

    public override void RenderBeginTag(HtmlTextWriter writer)
    {
      log.Debug("DataListValidationSummary.RenderBeginTag");

      if (errorMessages == null || errorMessages.Count == 0)
      {
        skipRendering = true;
        return;
      }

      writer.WriteBeginTag("div");
      writer.WriteAttribute("class", this.CssClass);
      writer.Write(HtmlTextWriter.TagRightChar);
      writer.WriteLine();
    }

    protected override void AddAttributesToRender(HtmlTextWriter writer)
    {
      log.Debug("DataListValidationSummary.AddAttributesToRender");
    }

    protected override void RenderContents(HtmlTextWriter writer)
    {
      log.Debug("DataListValidationSummary.RenderContents");

      if (skipRendering)
      {
        return;
      }

      writer.Write(headerText);
      writer.WriteLine();
      writer.WriteBeginTag("ul");
      writer.Write(HtmlTextWriter.SelfClosingTagEnd);
      writer.WriteLine();

      foreach (String msg in errorMessages)
      {
        writer.WriteBeginTag("li");
        writer.Write(HtmlTextWriter.SelfClosingTagEnd);
        writer.Write(msg);
        writer.WriteEndTag("li");
        writer.WriteLine();
      }
    }

    public override void RenderEndTag(HtmlTextWriter writer)
    {
      log.Debug("DataListValidationSummary.RenderEndTag");

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
