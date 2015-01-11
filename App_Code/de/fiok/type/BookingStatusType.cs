namespace de.fiok.type
{
  using System;
  using System.Collections;
  using de.fiok.core;
  
  public enum BookingStatus
  {
    RESERVED = 0,
    BOOKED = 1,
    CANCELED = -1
  }
  
  /// <summary>
  /// Aufzählungstyp für den Buchungsstatus.
  /// </summary>
  /// <remarks>
	/// created by - Steffen Förster
	/// </remarks>
  public class BookingStatusType
  {
    public static String GetName (BookingStatus type) 
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
    
    public static String GetValueAsString (BookingStatus type)
    {
      return ((Int32)type).ToString ();
    }
    
    private static String ResourceKey
    {
      get {return "BookingStatusType.";}
    }
  }
}
