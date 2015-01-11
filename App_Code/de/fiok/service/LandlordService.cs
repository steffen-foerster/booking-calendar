namespace de.fiok.service
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Text;
  using System.Data;
  using de.fiok.core;
  using de.fiok.type;
  using log4net;

  /// <summary>
  /// Mit diesem Service kann auf die Vermieterdaten zugegriffen werden. 
  /// </summary>
  public class LandlordService : BaseService
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(LandlordService));

    private static LandlordService instance = new LandlordService ();

    private LandlordService ()
    {
    }

    public static LandlordService GetInstance ()
    {
      return instance;
    }
    
    #region Landlord
    
    /// <summary>
    /// Liefert den zur ID gehörenden Vermieter.
    /// <summary>
    public LandlordBean RetrieveLandlord (int landlordId)
    {
      log.Debug ("HouseService.RetrieveLandlord");
         
      return (LandlordBean)SQLExecutorFactory.Create ().Execute (true, delegate (SQLExecutor executor)
      {
        IDbCommand command = executor.CreateCommand ();
         
        String sql = 
          " SELECT l_id, l_name, l_firstname, l_email, l_domain, l_role" +
          " FROM landlord WHERE l_id = ?";
        executor.SetCommandText (sql, command);

        executor.AddInt (command, landlordId);
        command.Prepare();
        IDataReader reader = executor.RegisterReader (command);

        LandlordBean result = null;
        if (reader.Read()) {
          result = new LandlordBean ();
          result.ID = reader.GetInt32 (0);
          result.Name = reader.GetString (1);
          result.Firstname = reader.GetString (2);
          result.Email = reader.GetString (3);
          result.Domain = reader.GetString (4);
          result.Role = LandlordRoleType.RetrieveType (reader.GetString (5));
        }
        else {
          log.Warn ("landlord could not be found by id: " + landlordId);
        }
        reader.Close ();

        return result;
      });
    }

    /// <summary>
    /// Liefert alle Vermieter.
    /// <summary>
    public List<LandlordBean> RetrieveLandlords ()
    {
      log.Debug ("HouseService.RetrieveLandlords");
         
      return (List<LandlordBean>)SQLExecutorFactory.Create ().Execute (true, delegate (SQLExecutor executor)
      {
        IDbCommand command = executor.CreateCommand ();
         
        String sql = 
          " SELECT l_id, l_name, l_firstname, l_email, l_domain, l_role FROM landlord";
        executor.SetCommandText (sql, command);
        command.Prepare();
        IDataReader reader = executor.RegisterReader (command);

        List<LandlordBean> result = new List<LandlordBean> ();
        while (reader.Read()) {
          LandlordBean bean = new LandlordBean ();
          bean.ID = reader.GetInt32 (0);
          bean.Name = reader.GetString (1);
          bean.Firstname = reader.GetString (2);
          bean.Email = reader.GetString (3);
          bean.Domain = reader.GetString (4);
          bean.Role = LandlordRoleType.RetrieveType (reader.GetString (5));

          result.Add (bean);
        }
        reader.Close ();

        return result;
      });
    }
    
    /// <summary>
    /// Liefert den E-Mail-Adresse und zum Passwort gehörenden Vermieter.
    /// <summary>
    public LandlordBean RetrieveLandlordByCredentials (String email, String password)
    {
      log.Debug ("HouseService.RetrieveLandlordByCredentialsJob");
         
      return (LandlordBean)SQLExecutorFactory.Create ().Execute (true, delegate (SQLExecutor executor)
      {
        IDbCommand command = executor.CreateCommand ();
         
        String sql = 
          " SELECT l_id, l_name, l_firstname, l_email, l_salt, l_password" +
          " FROM landlord WHERE l_email = ?";
        executor.SetCommandText (sql, command);  
    
        executor.AddString (command, email);
        command.Prepare();
        IDataReader reader = executor.RegisterReader (command);

        LandlordBean result = null;
        if (reader.Read()) {

          int salt = reader.GetInt32 (4);
          String savedPwd = reader.GetString (5);
          
          // das Bean wird nur erstellt, wenn das Passwort mit dem gespeicherten Passwort übereinstimmt
          if (AuthenticationUtils.ComputeSaltedHash (password, salt).Equals (reader.GetString (5))) {
            result = new LandlordBean (
              reader.GetInt32 (0),
              reader.GetString (1),
              reader.GetString (2),
              reader.GetString (3)
            );
          }
        }
        else {
          log.Debug ("landlord could not be found by id: " + email);
        }
        reader.Close ();

        return result;
      });

    }
    
    /// <summary>
    /// Speichert Änderungen zu einem Vermieter einschließlich eines neuen Passworts.
    /// <summary>
    public void UpdateLandlord (LandlordBean landlord, LandlordBean.Password password)
    {
      log.Debug ("HouseService.UpdateLandlord");

      SQLExecutorFactory.Create ().Execute (true, delegate (SQLExecutor executor)
      {
        IDbCommand command = executor.CreateCommand ();
        
        String sql = 
          " UPDATE landlord SET l_name = ?, l_firstname = ?, l_email = ?," + 
          "                     l_password = ?, l_salt = ?" +
          " WHERE l_id = ?";
        executor.SetCommandText (sql, command);

        executor.AddParams (command, landlord.Name, landlord.Firstname, landlord.Email,
                                     password.Phrase, password.Salt, landlord.ID);

        command.Prepare();
        int rowCount = command.ExecuteNonQuery ();
        log.Debug ("landlord " + landlord.ID + " updated = " + (rowCount == 1));
        
        return null;
      });
    }

    /// <summary>
    /// Speichert Änderungen zu einem Vermieter.
    /// <summary>
    public void UpdateLandlord (LandlordBean landlord)
    {
      log.Debug ("HouseService.UpdateLandlord");

      SQLExecutorFactory.Create ().Execute (true, delegate (SQLExecutor executor)
      {
        bool createPwd = !String.IsNullOrEmpty (landlord.NewPassword);

        IDbCommand command = null;

        if (createPwd) {
          int salt = AuthenticationUtils.CreateRandomSalt ();
          String hash = AuthenticationUtils.ComputeSaltedHash (landlord.NewPassword, salt);

          command = executor.CreateCommand ();

          String sql = 
            " UPDATE landlord SET l_name = ?, l_firstname = ?, l_email = ?, l_domain = ?, l_role = ?," +
            " l_salt = ?, l_password = ? WHERE l_id = ?";
  
          executor.SetCommandText (sql, command);

          executor.AddParams (command, landlord.Name, landlord.Firstname, landlord.Email, 
                              landlord.Domain, landlord.Role.ToString (), salt, hash, landlord.ID);
        }
        else {
          command = executor.CreateCommand ();

          String sql = 
            " UPDATE landlord SET l_name = ?, l_firstname = ?, l_email = ?, l_domain = ?, l_role = ?" +
            " WHERE l_id = ?";
  
          executor.SetCommandText (sql, command);

          executor.AddParams (command, landlord.Name, landlord.Firstname, landlord.Email, 
                              landlord.Domain, landlord.Role.ToString (), landlord.ID);
        }
        
        command.Prepare();
        
        int rowCount = command.ExecuteNonQuery ();
        log.Debug ("landlord " + landlord.ID + " updated = " + (rowCount == 1));
        
        return null;
      });
    }

    /// <summary>
    /// Erstellt einen neuen Vermieter.
    /// <summary>
    public void InsertLandlord (LandlordBean landlord)
    {
      log.Debug ("LandlordService.InsertLandlord");

      SQLExecutorFactory.Create ().Execute (true, delegate (SQLExecutor executor)
      {
        // Passwort erstellen
        int salt = AuthenticationUtils.CreateRandomSalt ();
        String hash = AuthenticationUtils.ComputeSaltedHash (landlord.NewPassword, salt);
          
        IDbCommand command = executor.CreateCommand ();

        String sql =
          " INSERT INTO landlord  (l_name, l_firstname, l_email, l_domain, l_role, l_password, l_salt)" +
          " VALUES (?,?,?,?,?,?,?)";

        executor.SetCommandText (sql, command);
        executor.AddParams (command, landlord.Name, landlord.Firstname, landlord.Email, 
                            landlord.Domain, landlord.Role.ToString (), hash, salt);
        command.Prepare();
        int rowCount = command.ExecuteNonQuery ();
        log.Debug ("landlord inserted = " + (rowCount == 1));

        int id = executor.RetrieveIdentity ();
        landlord.ID = id;
        
        return null;
      });

      // neues Haus erstellen
      int count = ProcessUpdate (
        " INSERT INTO house (h_cleaning_cost, h_bedclothes, h_location, h_name, h_landlord_id) VALUES (0,0,'Ort ?','Objekt 1',?)",
        landlord.ID
      );

      log.Debug ("house inserted = " + (count == 1));
    }

    /// <summary>
    /// Löscht einen Vermieter und die zugeordneten Daten (Häuser, Hauspreise, Buchungen etc.)
    /// </summary>
    public void DeleteLandlord (LandlordBean landlord)
    {
      log.Debug ("LandlordService.DeleteLandlord");

      int count = ProcessUpdate ("DELETE FROM landlord WHERE l_id = ?", landlord.ID);

      log.Debug ("landlord " + landlord.ID + " deleted: " + (count == 1));
    }


    public String GetDomainOfDemoAccount (String referrer)
    {
      log.Debug ("LandlordService.GetDomainOfDemoAccount");

      return (String)SQLExecutorFactory.Create ().Execute (true, delegate (SQLExecutor executor)
      {
        IDbCommand command = executor.CreateCommand ();
         
        String sql = "SELECT l_domain FROM landlord WHERE l_role = 'DEMO'";
        executor.SetCommandText (sql, command);  
        command.Prepare();
        IDataReader reader = executor.RegisterReader (command);

        while (reader.Read()) {
          String domain = executor.GetString (reader, 0);
          if (domain != null) {
            if (referrer.Contains (domain)) {
              return domain;
            }
          }
        }
        reader.Close ();

        return "";
      });
    }
  }
  
  #endregion
}
