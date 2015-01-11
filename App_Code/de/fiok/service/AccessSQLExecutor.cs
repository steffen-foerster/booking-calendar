namespace de.fiok.service
{
  using System;
  using System.Collections;
  using System.Data;
  using System.Data.OleDb;
  using System.Text;
  using log4net;
  using de.fiok.type;

  /// <summary>
  /// Durch den Einsatz des SQLExecutors wird der Code für den Zugriff auf die 
  /// Datenbank vereinfacht und sich wiederholender Code vermieden.
  /// </summary>
  public class AccessSQLExecutor : SQLExecutor
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(AccessSQLExecutor));

    private IDbConnection conn;
    private IDbTransaction trans;
    //private int paramCounter = 0;
    private IList readers = new ArrayList ();
    private String lastSQL;

    public AccessSQLExecutor ()
    {
    }
    
    #region Implementation SQLExecutor

    /// <summary>
    /// Ausführung einer oder mehrerer DB-Anfragen oder Updates wobei das notwendige
    /// Exception-Handling vom SQLExecutor übernommen wird.
    /// </summary>
    public override object Execute (bool commit, SQLJob job)
    {
      try {
        if (conn == null) {
          conn = DatabaseService.GetConnection ();
          trans = conn.BeginTransaction (IsolationLevel.ReadCommitted);
        }
      
        Object result = job (this);
        if (commit) {
          trans.Commit ();
        }
        return result;
      }
      catch (Exception e) {
        log.Error ("Last SQL-Statement: " + lastSQL);
        log.Error (e, e);
        try {
          trans.Rollback ();
        }
        catch (InvalidOperationException ie) {
          log.Warn ("cannot rollback", ie);
        }
        throw e;
      }
      finally {
        if (commit) {
          conn.Close ();
          CloseReaders ();
        }
      }
    }
    
    /// <summary>
    /// Erstellt ein neues Command.
    /// </summary>
    public override IDbCommand CreateCommand ()
    {
      IDbCommand command = new OleDbCommand ();
      command.CommandTimeout = 0;
      command.Connection = conn;
      command.Transaction = trans;

      return command;
    }

    /// <summary>
    /// Setzt das SQL Statement und speichert es für den Fehlerfall, um dann eine Fehlermeldung mit 
    /// dem letzten Statement zu erzeugen.
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="command"></param>
    public override void SetCommandText (String sql, IDbCommand command)
    {
      this.lastSQL = sql;
      command.CommandText = sql;
    }

    /// <summary>
    /// Fügt einen Parameter zum übergebenen Command dazu.
    /// </summary>
    public override void AddInt (IDbCommand command, Int32 param)
    {
      OleDbCommand oleCommand = (OleDbCommand)command;
      OleDbParameter oleParam = new OleDbParameter ();
      oleParam.Value = param;
      oleParam.OleDbType = OleDbType.Integer;
      oleCommand.Parameters.Add (oleParam);
    }

    /// <summary>
    /// Fügt einen Parameter zum übergebenen Command dazu.
    /// </summary>
    public override void AddBoolean (IDbCommand command, Boolean param)
    {
      AddInt (command, (int)DBBooleanType.GetDBBoolean (param));
    }
    
    /// <summary>
    /// Fügt einen Parameter zum übergebenen Command dazu. Der übergebene String wird ggf.
    /// auf die angegebene Länge gestutzt. 
    /// </summary>
    public override void AddString (IDbCommand command, String param, int maxLength)
    {
      if (param != null && param.Length > maxLength) {
        param = param.Substring (0, maxLength);
      }
      AddString (command, param);
    }
    
    /// <summary>
    /// Fügt einen Parameter zum übergebenen Command dazu.
    /// </summary>
    public override void AddString (IDbCommand command, String param)
    {
      if (param == null || param.Length == 0) {
        AddNull (command, CreateNullValue (typeof (String)));
        return;
      }
    
      OleDbCommand oleCommand = (OleDbCommand)command;
      OleDbParameter oleParam = new OleDbParameter ();
      oleParam.IsNullable = true;
      oleParam.Value = param;
      oleParam.Size = param.Length;
      oleParam.OleDbType = OleDbType.VarChar;
      
      oleCommand.Parameters.Add (oleParam);
    }
    
    /// <summary>
    /// Fügt einen Parameter zum übergebenen Command dazu.
    /// </summary>
    public override void AddNull (IDbCommand command, INullValue param)
    {
      OleDbCommand oleCommand = (OleDbCommand)command;
      OleDbParameter oleParam = new OleDbParameter ();
      oleParam.IsNullable = true;
      oleParam.Value = DBNull.Value;
      OleDbType type = (OleDbType)param.GetDBType ();
      
      oleParam.OleDbType = type;
      if (type == OleDbType.VarChar) {
        oleParam.Size = 1;
      }
      
      oleCommand.Parameters.Add (oleParam);
    }

    /// <summary>
    /// Fügt einen Parameter zum übergebenen Command dazu.
    /// </summary>
    public override void AddDate (IDbCommand command, DateTime param)
    {
      OleDbCommand oleCommand = (OleDbCommand)command;
      OleDbParameter oleParam = new OleDbParameter ();
      oleParam.Value = param;
      oleParam.OleDbType = OleDbType.Date;
      oleCommand.Parameters.Add (oleParam);
    }
    
    /// <summary>
    /// Fügt mehrere Parameter zum übergebenen Command dazu.
    /// </summary>
    public override void AddParams (IDbCommand command, params Object[] dbParams)
    {
      if (dbParams == null) {
        return;      
      }
      
      for (int i = 0; i < dbParams.Length; i++) {
        if (dbParams[i] is String) {
          AddString (command, (String)dbParams[i]);
        }
        else if (dbParams[i] is Int32) {
          AddInt (command, (Int32)dbParams[i]);
        }
        else if (dbParams[i] is DateTime) {
          AddDate (command, (DateTime)dbParams[i]);
        }
        else if (dbParams[i] is Boolean) {
          AddBoolean (command, (Boolean)dbParams[i]);
        }
        else if (dbParams[i] is INullValue) {
          AddNull (command, (INullValue)dbParams[i]);
        }
        else {
          throw new Exception ("unknown parameter type: " + dbParams[i].GetType ());
        }
      }
    }
    
    /// <summary>
    /// Setzt für einen Parameter einen neuen Wert.
    /// </summary>
    public override void SetParameterValue (IDbCommand command, int index, Object value)
    {
      OleDbCommand oleCommand = (OleDbCommand)command;
      oleCommand.Parameters[index].Value = value;
    }
    
    /// <summary>
    /// Setzt für einen Parameter einen neuen Wert.
    /// </summary>
    public override void SetParameterValue (IDbCommand command, int index, String value, int maxLength)
    {
      OleDbCommand oleCommand = (OleDbCommand)command;
      
      if (value != null && value.Length > maxLength) {
        value = value.Substring (0, maxLength);
      }
      
      if (value != null && value != "") {
        oleCommand.Parameters[index].Value = value;
        oleCommand.Parameters[index].Size = value.Length;
      }
      else {
        oleCommand.Parameters[index].IsNullable = true;
        oleCommand.Parameters[index].Value = DBNull.Value;
        oleCommand.Parameters[index].Size = 1;
      }
    }

       
    /// <summary>
    /// Liefert einen Reader und speichert die Referenz, um ein Schließen des Readers nach 
    /// Beendigung eines Jobs durchführen zu können.
    /// </summary>
    public override IDataReader RegisterReader (IDbCommand command)
    {
      IDataReader reader = command.ExecuteReader();
      readers.Add (reader);
      return reader;
    }
    
    /// <summary>
    /// Liefert den Wert einer Spalte als String. Es werden DBNull-Werte als 'null' zurückgeliefert.
    /// </summary>
    public override String GetString (IDataReader reader, int index)
    {
      if (reader.GetValue (index) is DBNull) {
        return null;
      }
      else {
        return reader.GetString (index);
      }
    }
    
    /// <summary>
    /// Liefert den Wert einer Spalte als Int32. Es werden DBNull-Werte als '0' zurückgeliefert.
    /// </summary>
    public override Int32 GetInt32 (IDataReader reader, int index)
    {
      if (reader.GetValue (index) is DBNull) {
        return 0;
      }
      else {
        return reader.GetInt32 (index);
      }
    }

    /// <summary>
    /// Liefert den Wert einer Spalte als Boolean.
    /// </summary>
    public override Boolean GetBoolean (IDataReader reader, int index)
    {
      if (reader.GetValue (index) is DBNull) {
        return false;
      }
      else {
        return DBBooleanType.GetBoolean (reader.GetInt32 (index));
      }
    }
    
    /// <summary>
    /// Liefert den Autowert, der beim letzten Insert erzeugt wurde.
    /// </summary>
    public override int RetrieveIdentity ()
    {
      IDbCommand command = CreateCommand ();
      command.CommandText = "SELECT @@Identity";
      IDataReader reader = RegisterReader (command);
      
      int result = -1;
      if (reader.Read ()) {
        result = reader.GetInt32 (0);
      }
      reader.Close ();
      
      return result;      
    }
    
    /// <summary>
    /// Liefert einen SQL-Ausdruck, der die beiden übergebenen Ausdrücke 
    /// als eine Zeichenkette zusammenfügt.
    /// </summary>
    public override String SQLConcat (params String[] expressions)
    {
      StringBuilder result = new StringBuilder ();
      foreach (String expression in expressions) {
        if (result.Length > 0) {
          result.Append (" & ");
        }
        result.Append (expression);
      }  
      return result.ToString ();
    }
    
    public override INullValue CreateNullValue (Type type) 
    {
      OleDbNullValue nullValue; 
    
      if (type == typeof (DateTime)) {
        nullValue = new OleDbNullValue (OleDbType.Date);
      }
      else if (type == typeof (Int32)) {
        nullValue = new OleDbNullValue (OleDbType.Integer);
      }
      else if (type == typeof (String)) {
        nullValue = new OleDbNullValue (OleDbType.VarChar);
      }
      else {
        throw new Exception ("unknown parameter type " + type);
      }
      
      return nullValue;
    }  

    #endregion

    /// <summary>
    /// Schließt noch offene Reader.
    /// </summary>
    private void CloseReaders ()
    {
      foreach (IDataReader reader in readers) {
        try {
          reader.Close ();
        }
        catch (Exception e) {
          log.Error ("Cannot close a reader: " + e.Message);
        }
      }
    }
  }
  
  #region Implementation INullValue
  
  /// <summary>
  /// Null-Wert für einen Parameter, der den zugehörigen Datenbank-Typ enthält.
  /// </summary>
  public class OleDbNullValue : INullValue
  {
    private OleDbType type;
    
    public OleDbNullValue (OleDbType type) 
    {
      this.type = type;
    }
       
    public DbType GetDBType ()
    {
      return (DbType)type;
    }
  }
  
  #endregion
}
