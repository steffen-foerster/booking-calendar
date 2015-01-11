namespace de.fiok.service
{
  using System;
  using System.Data;
  using System.Data.OleDb;
  using System.Collections;
  using System.Text;
  using log4net;
  
  /// <summary>
  /// Basisklasse für alle Services.
  /// </summary>
  /// <remarks>
	/// created by - Steffen Förster
	/// </remarks>
  public class BaseService
  {
    private static readonly ILog log = LogManager.GetLogger (typeof (BaseService));

    /// <summary>
    /// Liefert true zurück, wenn mindests ein Datensatz mit der übergebenen Query 
    /// selektiert werden kann.
    /// </summary>
    protected static bool ExistsRecord (String query, params Object[] dbParams)
    {
      log.Debug ("BaseService.ExistsRecord");

      return ExistsRecord (SQLExecutorFactory.Create (), true, query, dbParams);
    }

    /// <summary>
    /// Liefert true zurück, wenn mindests ein Datensatz mit der übergebenen Query 
    /// selektiert werden kann.
    /// </summary>
    protected static bool ExistsRecord (SQLExecutor exe, String query, params Object[] dbParams)
    {
      log.Debug ("BaseService.ExistsRecord");

      return ExistsRecord (exe, false, query, dbParams);
    }

    /// <summary>
    /// Liefert true zurück, wenn mindests ein Datensatz mit der übergebenen Query 
    /// selektiert werden kann.
    /// </summary>
    private static bool ExistsRecord (SQLExecutor exe, bool commit, String query, params Object[] dbParams)
    {
      log.Debug ("BaseService.ExistsRecord");

      return (bool)exe.Execute (commit, delegate (SQLExecutor executor)
      {
        IDbCommand command = executor.CreateCommand ();
        executor.SetCommandText (query, command);

        executor.AddParams (command, dbParams);
        command.Prepare ();
        IDataReader reader = executor.RegisterReader (command);

        reader.Read ();
        int result = reader.GetInt32 (0);
        reader.Close ();

        return result >= 1;
      });
    }

    /// <summary>
    /// Führt eine Datenbankanfrage aus und liefert als Ergebnis ein KeyValue-Array. 
    /// Das SQL-Statement muss zwei Spalten enthalten, an Position 1 steht der 'key'-Wert
    /// und an Position 2 der 'value'-Wert.
    /// </summary>
    protected static KeyValue[] ProcessKeyValue (String query, params Object[] dbParams)
    {
      log.Debug ("BaseService.ProcessKeyValue");

      return (KeyValue[])SQLExecutorFactory.Create ().Execute (true, delegate (SQLExecutor executor)
      {
        IDbCommand command = executor.CreateCommand ();
        executor.SetCommandText (query, command);

        executor.AddParams (command, dbParams);
        command.Prepare ();
        IDataReader reader = executor.RegisterReader (command);

        ArrayList items = new ArrayList ();
        while (reader.Read ()) {
          // TODO Key muss zur Zeit immer ein Integer-Wert sein -> Typ über Meta-Daten abfragen
          KeyValue item = new KeyValue (reader.GetInt32 (0).ToString (), reader.GetString (1));
          items.Add (item);
        }
        reader.Close ();

        return items.ToArray (typeof (KeyValue));
      });
    }

    /// <summary>
    /// Erstellt einen String mit kommaseparierten '?'.
    /// </summary>
    protected static String CreateParamWildcats (int count)
    {
      log.Debug ("BaseService.CreateParamWildcats");
      StringBuilder wildcats = new StringBuilder ();
      for (int i = 0; i < count; i++) {
        if (wildcats.Length > 0) {
          wildcats.Append (",");
        }
        wildcats.Append ("?");
      }

      return wildcats.ToString ();
    }

    /// <summary>
    /// Führt eine Datenbankanfrage aus und liefert den Wert der ersten Zeile und der ersten Spalte
    /// des Results als int-Wert.
    /// </summary>
    /// <returns>-1 if value is null</returns>
    protected static int ProcessIntQuery (String query, params Object[] dbParams)
    {
      log.Debug ("BaseService.ProcessIntQuery");

      return (Int32)SQLExecutorFactory.Create ().Execute (true, delegate (SQLExecutor executor)
      {
        IDbCommand command = executor.CreateCommand ();
        executor.SetCommandText (query, command);
        executor.AddParams (command, dbParams);
        command.Prepare ();
        IDataReader reader = executor.RegisterReader (command);

        int result = -1;
        if (reader.Read ()) {
          if (reader.IsDBNull (0)) {
            result = -1;
          }
          else {
            result = reader.GetInt32 (0);
          }
        }
        else {
          throw new DataException ("no result row returned");
        }
        reader.Close ();

        return result;
      });
    }

    /// <summary>
    /// Führt eine Datenbankanfrage aus und liefert den Wert der ersten Zeile und der ersten Spalte
    /// des Results als String-Wert.
    /// </summary>
    protected static String ProcessStringQuery (String query, params Object[] dbParams)
    {
      log.Debug ("BaseService.ProcessStringQuery");

      return (String)SQLExecutorFactory.Create ().Execute (true, delegate (SQLExecutor executor)
      {
        IDbCommand command = executor.CreateCommand ();
        executor.SetCommandText (query, command);
        executor.AddParams (command, dbParams);
        command.Prepare ();
        IDataReader reader = executor.RegisterReader (command);

        String result = null;
        if (reader.Read ()) {
          if (reader.IsDBNull (0)) {
            result = null;
          }
          else {
            result = reader.GetString (0);
          }
        }
        else {
          throw new DataException ("no result row returned");
        }
        reader.Close ();

        return result;
      });
    }

    /// <summary>
    /// Führt ein Insert oder Update aus.
    /// </summary>
    /// <returns>Anzahl der geänderten Zeilen</returns>
    protected static int ProcessUpdate (String query, params Object[] dbParams)
    {
      log.Debug ("BaseService.ProcessUpdate");

      return (Int32)SQLExecutorFactory.Create ().Execute (true, delegate (SQLExecutor executor)
      {
        IDbCommand command = executor.CreateCommand ();
        executor.SetCommandText (query, command);
        executor.AddParams (command, dbParams);
        command.Prepare ();
        return command.ExecuteNonQuery ();
      });
    }
  }
}
