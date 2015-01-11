namespace de.fiok.service
{
  using System;
  
  /// <summary>
  /// Objekte dieser Klasse enthalten einen Schlüssel und einen Wert. Objekte dieser Klasse 
  /// können somit einfach in eine Auswahlliste geladen werden.
  /// </summary>
  /// <remarks>
	/// created by - Steffen Förster
	/// </remarks>
	public class KeyValue
	{
	  private String key;
	  private String value;
	  
	  public KeyValue (String key, String value)
	  {
	    this.key = key;
	    this.value = value;
	  }
	  
	  public String Key 
	  {
	    get {return key;}
	  }
	  
	  public String Value 
	  {
	    get {return value;}
	  }
	}
}

