namespace de.fiok.type
{
  using System;
  using System.Collections;
  using de.fiok.core;
  using log4net;
  
  public enum DBBoolean
  {
    FALSE,
    TRUE
  }
  
  /// <summary>
  /// Aufzählungstyp für die Wahrweitswerte in der Datenbank.
  /// </summary>
  /// <remarks>
	/// created by - Steffen Förster
	/// </remarks>
  public class DBBooleanType
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(DBBooleanType));
  
    public static DBBoolean GetDBBoolean (bool value)
    {
      log.Debug ("DBBooleanType.GetDBBoolean");
    
      return value ? DBBoolean.TRUE : DBBoolean.FALSE;
    }
    
    public static bool GetBoolean (DBBoolean type)
    {
      log.Debug ("DBBooleanType.GetBoolean");
    
      return type == DBBoolean.TRUE;
    }
    
    public static bool GetBoolean (int type)
    {
      log.Debug ("DBBooleanType.GetBoolean");
    
      return ((DBBoolean)type) == DBBoolean.TRUE;
    }
  }
}
