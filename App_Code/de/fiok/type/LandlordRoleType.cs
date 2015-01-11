namespace de.fiok.type
{
  using System;
  using System.Collections;
  using de.fiok.core;
  
  [Serializable]
  public enum LandlordRole
  {
    ADMIN,
    STANDARD,
    CALENDAR,
    DEMO,
    DEFAULT
  }
  
  /// <summary>
  /// Aufzählungstyp für die Rollen eines Vermieters.
  /// </summary>
  /// <remarks>
	/// created by - Steffen Förster
	/// </remarks>
  [Serializable]
  public class LandlordRoleType
  {
    public static LandlordRole RetrieveType (String typeValue)
    {
      if ("ADMIN".Equals (typeValue)) {
        return LandlordRole.ADMIN;
      }
      else if ("STANDARD".Equals (typeValue)) {
        return LandlordRole.STANDARD;
      }
      else if ("CALENDAR".Equals (typeValue)) {
        return LandlordRole.CALENDAR;
      }
      else if ("DEMO".Equals (typeValue)) {
        return LandlordRole.DEMO;
      }
      else {
        return LandlordRole.DEFAULT;
      }
    }
    
    public static String GetValueAsString (LandlordRole type)
    {
      return type.ToString ();
    }
  }
}
