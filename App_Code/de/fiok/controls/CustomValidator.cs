namespace de.fiok.controls
{
  using System;
  using System.Drawing;
  using System.Web;
  using System.Web.UI;
  using System.Web.UI.WebControls;
  using System.Collections;
  using de.fiok.core;
  using log4net;

  /// <summary>
  /// Das Standard-Control wurde überschrieben, um die Standard-Konfiguration des Validators
  /// nicht bei jeder Verwendung zu wiederholen, außerdem wird der Border des Eingabefeldes
  /// rot dargestellt.
  /// </summary>
  /// <remarks>
  /// created by - Steffen Förster  
  /// </remarks>
  public class CustomValidator : System.Web.UI.WebControls.CustomValidator
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(CustomValidator));

    private bool resetBorderColor = true;

    public bool ResetBorderColor
    {
      set { this.resetBorderColor = value; }
    }

    public CustomValidator() : base()
    {
      this.Display = ValidatorDisplay.None;
      this.EnableClientScript = false;
      this.EnableViewState = false;
    }

    override protected void OnInit(EventArgs e)
    {
      log.Debug("CustomValidator.OnInit");

      InitializeComponent();
      base.OnInit(e);
    }

    /// <summary>
    /// Initialisierung.
    /// </summary>
    private void InitializeComponent()
    {
      log.Debug("CustomValidator.InitializeComponent");

      this.PreRender += new System.EventHandler(this.PreRenderHandler);
      this.Load += new EventHandler (Validator_Load);
    }

    /// <summary>
    /// Roten Border entfernen.
    /// </summary>
    private void Validator_Load (object sender, EventArgs e)
    {
      foreach (Object item in Parent.Controls) {
        if (item is WebControl) {
          WebControl toValidate = (WebControl)item;
          String id = toValidate.ClientID;

          if (id.EndsWith ("_" + this.ControlToValidate) && resetBorderColor) {
            toValidate.BorderColor = Color.Empty;
            break;
          }
        }
      }
    }

    /// <summary>
    /// Border des Eingabefeldes bei einem Fehler rot zeichnen.
    /// </summary>
    override protected bool EvaluateIsValid()
    {
      log.Debug("CustomValidator.EvaluateIsValid");

      bool result = base.EvaluateIsValid();
      log.Debug("validator id: " + ID + ", isValid: " + result);

      foreach (Object item in Parent.Controls) {
        if (item is WebControl) {
          WebControl toValidate = (WebControl)item;
          String id = toValidate.ClientID;
          if (id.EndsWith ("_" + this.ControlToValidate) && ! result) {
            toValidate.BorderColor = Color.Red;
            break;
          }
        }
      }

      return result;
    }

    /// <summary>
    /// Daten werden gebunden.
    /// </summary>
    private void PreRenderHandler(Object sender, EventArgs e)
    {
      log.Debug("CustomValidator.PreRenderHandler");

      this.DataBind();
    }
  }
}
