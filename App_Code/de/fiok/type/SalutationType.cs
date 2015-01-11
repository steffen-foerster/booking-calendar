namespace de.fiok.type
{
  using System;
  using System.Collections;
  using de.fiok.core;
  
  public enum Salutation
  {
    MR,
    MRS
  }
  
  /// <summary>
  /// Aufzählungstyp für die Anrede.
  /// </summary>
  /// <remarks>
	/// created by - Steffen Förster
	/// </remarks>
  public class SalutationType
  {
    public static Salutation RetrieveType (String typeValue)
    {
      return (Salutation)Int32.Parse (typeValue);
    }
    
    public static String GetName (Salutation type) 
    {
      return AppResources.GetMessage(ResourceKey + ((int)type));
    }
    
    public static String GetName (int typeValue)
    {
      return AppResources.GetMessage(ResourceKey + typeValue);      
    }
    
    public static String GetName (String typeValue)
    {
      return AppResources.GetMessage(ResourceKey + typeValue);      
    }
    
    public static String GetValueAsString (Salutation type)
    {
      return ((Int32)type).ToString ();
    }
    
    private static String ResourceKey
    {
      get {return "SalutationType.";}
    }
  }
}
