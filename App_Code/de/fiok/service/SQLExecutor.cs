namespace de.fiok.service
{
  using System;
  using System.Collections;
  using System.Data;

  /// <summary>
  /// Durch den Einsatz des SQLExecutors wird der Code f�r den Zugriff auf die 
  /// Datenbank vereinfacht und sich wiederholender Code vermieden. Au�erdem k�nnen durch
  /// verschiedene Implementierungen unterschiedliche Datenbanken verwendet werden. Sofern
  /// nur Standard-SQL benutzt wird, ist dies auch ohne Anpassung der SQL-Statements m�glich.
  /// </summary>
  public abstract class SQLExecutor
  {
    public delegate object SQLJob (SQLExecutor executor);

    /// <summary>
    /// Ausf�hrung einer oder mehrerer DB-Anfragen oder Updates wobei das notwendige
    /// Exception-Handling vom SQLExecutor �bernommen wird.
    /// </summary>
    public abstract object Execute (bool commit, SQLJob job);
    
    /// <summary>
    /// Erstellt ein neues Command.
    /// </summary>
    public abstract IDbCommand CreateCommand ();
    
    /// <summary>
    /// Setzt das SQL Statement und speichert es f�r den Fehlerfall, um dann eine Fehlermeldung mit 
    /// dem letzten Statement zu erzeugen.
    /// </summary>
    public abstract void SetCommandText (String sql, IDbCommand command);

    /// <summary>
    /// F�gt einen Parameter zum �bergebenen Command dazu.
    /// </summary>
    public abstract void AddInt (IDbCommand command, Int32 param);
        
    /// <summary>
    /// F�gt einen Parameter zum �bergebenen Command dazu.
    /// </summary>
    public abstract void AddBoolean (IDbCommand command, Boolean param);

    /// <summary>
    /// F�gt einen Parameter zum �bergebenen Command dazu. Der �bergebene String wird ggf.
    /// auf die angegebene L�nge gestutzt. 
    /// </summary>
    public abstract void AddString (IDbCommand command, String param, int maxLength);
        
    /// <summary>
    /// F�gt einen Parameter zum �bergebenen Command dazu.
    /// </summary>
    public abstract void AddString (IDbCommand command, String param);
        
    /// <summary>
    /// F�gt einen Parameter zum �bergebenen Command dazu.
    /// </summary>
    public abstract void AddNull (IDbCommand command, INullValue param);
    
    /// <summary>
    /// F�gt einen Parameter zum �bergebenen Command dazu.
    /// </summary>
    public abstract void AddDate (IDbCommand command, DateTime param);
        
    /// <summary>
    /// F�gt mehrere Parameter zum �bergebenen Command dazu.
    /// </summary>
    public abstract void AddParams (IDbCommand command, params Object[] dbParams);
        
    /// <summary>
    /// Setzt f�r einen Parameter einen neuen Wert.
    /// </summary>
    public abstract void SetParameterValue (IDbCommand command, int index, Object value);
           
    /// <summary>
    /// Setzt f�r einen Parameter einen neuen Wert.
    /// </summary>
    public abstract void SetParameterValue (IDbCommand command, int index, String value, int maxLength);       
           
    /// <summary>
    /// Liefert einen Reader und speichert die Referenz, um ein Schlie�en des Readers nach 
    /// Beendigung eines Jobs durchf�hren zu k�nnen.
    /// </summary>
    public abstract IDataReader RegisterReader (IDbCommand command);
        
    /// <summary>
    /// Liefert den Wert einer Spalte als String. Es werden DBNull-Werte als 'null' zur�ckgeliefert.
    /// </summary>
    public abstract String GetString (IDataReader reader, int index);
    
    /// <summary>
    /// Liefert den Wert einer Spalte als Int32. Es werden DBNull-Werte als '0' zur�ckgeliefert.
    /// </summary>
    public abstract Int32 GetInt32 (IDataReader reader, int index);

    /// <summary>
    /// Liefert den Wert einer Spalte als Boolean.
    /// </summary>
    public abstract Boolean GetBoolean (IDataReader reader, int index);
        
    /// <summary>
    /// Liefert den Autowert, der beim letzten Insert erzeugt wurde.
    /// </summary>
    public abstract int RetrieveIdentity ();
    
    /// <summary>
    /// Liefert einen SQL-Ausdruck, der die �bergebenen Ausdr�cke 
    /// als eine Zeichenkette zusammenf�gt.
    /// </summary>
    public abstract String SQLConcat (params String[] expressions);
    
    /// <summary>
    /// Erstellt einen neuen INullValue.
    /// </summary>
    public abstract INullValue CreateNullValue (Type type);
  }
  
  /// <summary>
  /// Null-Wert f�r einen Parameter, der den zugeh�rigen Datenbank-Typ enth�lt.
  /// </summary>
  public interface INullValue 
  {
    DbType GetDBType ();
  }
}
