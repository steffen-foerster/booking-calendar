namespace de.fiok.web
{
  using System;
  using System.Drawing;
  using System.Collections;
  using System.Text;
  using System.Web.UI;
  using System.Web.UI.WebControls;
  using System.Web.UI.HtmlControls;
  using de.fiok.service;
  using de.fiok.controls;
  using log4net;
  
  /// <summary>
  /// Diese Klasse enthält Utility-Methoden, die von der Web-Schicht verwendet werden.
  /// </summary>
  /// <remarks>
	/// created by - Steffen Förster
	/// </remarks>
	public class WebUtils
	{
    private static readonly ILog log = LogManager.GetLogger(typeof(WebUtils));
	  	  
	  private WebUtils ()
	  {
	  }
	  
	  /// <summary>
    /// Es wird eine Konvertierung des KeyValue-Array in ListItems durchgeführt, die danach
    /// in die Collection eingefügt werden.
    /// </summary>
	  public static void ConvertToListItems (ListItemCollection listItems, KeyValue[] items)
	  {
	    log.Debug ("WebUtils.ConvertToListItems");
	  
	    // alle Items zuerst entfernen 
	    listItems.Clear ();
	    
	    foreach (KeyValue item in items) {
	      listItems.Add (new ListItem (item.Value, item.Key));
	    }
	  }
	  
	  /// <summary>
    /// Aktiviert alle Validator-Controls mit einem bestimmten Prefix in der ID.
    /// Die Validator-Controls müssen ein Kind des Form-Controls sein.
    /// </summary>
	  public static void EnableValidators (Page page, String idPrefix)
	  {
	    log.Debug ("WebUtils.EnableValidators");
	    
	    foreach (Control outerControl in page.Controls) {
	      if (outerControl is HtmlForm) {
	        foreach (Control innerControl in outerControl.Controls) {
	          if (innerControl is BaseValidator) {
    	        BaseValidator validator = (BaseValidator)innerControl;
    	        if (validator.ID.StartsWith (idPrefix)) {
    	          validator.Enabled = true;
    	        }
    	      }
  	      }
	      }
	    }
	  }
	  
	  /// <summary>
    /// Aktiviert alle Validator-Controls eines DataListItems.
    /// </summary>
	  public static void EnableValidators (DataListItem item)
	  {
	    log.Debug ("WebUtils.EnableValidators");
	    
	    foreach (Control control in item.Controls) {
	      //log.Debug ("control.GetType (): " + control.GetType ());
	    
	      if (control is BaseValidator) {
    	    BaseValidator validator = (BaseValidator)control;
  	      validator.Enabled = true;
	      }
	    }
	  }
	  
	  /// <summary>
    /// Erstellt eine Liste mit allen Fehlermeldungen aller Validators des übergebenen Items,
    /// und übergibt diese an die übergebene DataListValidationSummary. 
    /// </summary>
	  public static void CollectErrorMessages (DataListItem item, DataListValidationSummary summary)
	  {
	    log.Debug ("WebUtils.CollectErrorMessages");
	    
	    IList errors = new ArrayList ();
	    IList errorControls = new ArrayList ();  
	    foreach (Control control in item.Controls) {
	      log.Debug ("control.GetType (): " + control.GetType ());
	    
	      if (control is BaseValidator) {
    	    BaseValidator validator = (BaseValidator)control;
    	    if (! validator.IsValid) {
    	      // DataBind notwendig, um die ErrorMessages an das Control zu binden ==> <%# Resources.GetMessage ()%>
    	      validator.DataBind ();
    	      errors.Add (validator.ErrorMessage);
    	      errorControls.Add (validator.ControlToValidate);   	      
  	      }
	      }
	    }
	    
	    log.Debug ("set errors: " + errors.Count);
	    summary.ErrorMessages = errors;
	    summary.ErrorControls = errorControls;
	  }
	  
	  /// <summary>
    /// Erstellt einen Style-String aus einer CssStyleCollection.
    /// </summary>
	  public static String BuildStyleString (CssStyleCollection style)
	  { 
	    StringBuilder result = new StringBuilder ();
	    foreach (String key in style.Keys) {
	      result.Append (key + ":" + style[key] + ";");
	    }
	    return result.ToString ();
	  }
	}
}
