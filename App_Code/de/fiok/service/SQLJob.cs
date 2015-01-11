namespace de.fiok.service
{
  using System;
  using System.Collections;

  public abstract class SQLJob
  {
    private String sql;
    
    public String SQL 
    {
      get {return sql;}
      set {sql = value;}
    }
  
    public abstract Object Execute (SQLExecutor executor, Hashtable properties);
  }
}
