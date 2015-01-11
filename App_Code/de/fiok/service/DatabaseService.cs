namespace de.fiok.service
{
  using System;
  using System.Data;
  using System.Configuration;
  using System.Data.OleDb;
  using log4net;

  public class DatabaseService
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(DatabaseService));

    private static String applicationPath;

    private DatabaseService()
    {
    }

    public static void InitService (String applicationPath)
    {
      log.Debug ("DatabaseService.InitService");

      DatabaseService.applicationPath = applicationPath;
    }

    public static IDbConnection GetConnection ()
    {
      log.Debug ("DatabaseService.GetConnection");

      String connStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + applicationPath + "\\App_Data\\booking.mdb;";
      connStr += "Jet OLEDB:Database Password=" + ConfigurationManager.AppSettings["database_password"] + ";";

      IDbConnection conn;
      try {
        conn =  new OleDbConnection (connStr);
        conn.Open ();
        return conn;
      }
      catch (Exception e) {
        log.Error ("cannot open the connection: ", e);
        throw e;
      }
    }
  }
}






