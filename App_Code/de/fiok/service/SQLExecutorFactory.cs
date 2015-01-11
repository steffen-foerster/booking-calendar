namespace de.fiok.service
{
  using System;
  using System.Collections;
  using System.Data;

  /// <summary>
  /// Die Factory erstellt einen neuen ISQLExecutor. Durch Anpassung der Factory kann im gesamten 
  /// Code eine neue Implementierung verwendet werden, wenn z.B. eine andere Datenbank eingesetzt 
  /// werden soll.
  /// </summary>
  public class SQLExecutorFactory
  {
    private SQLExecutorFactory ()
    {
    }
    
    public static SQLExecutor Create ()
    {
      return new AccessSQLExecutor ();
    }
  }
}

