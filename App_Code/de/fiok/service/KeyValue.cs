namespace de.fiok.service
{
  using System;
  
  /// <summary>
  /// Objekte dieser Klasse enthalten einen Schl�ssel und einen Wert. Objekte dieser Klasse 
  /// k�nnen somit einfach in eine Auswahlliste geladen werden.
  /// </summary>
  /// <remarks>
	/// created by - Steffen F�rster
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

