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
  /// Mit diesem Service kann auf die Häuser- und Vermieterdaten zugegriffen werden. 
  /// </summary>
  public class HouseService : BaseService
  {
    private static readonly ILog log = LogManager.GetLogger (typeof (HouseService));
    private static readonly LandlordService landlordService = LandlordService.GetInstance ();

    private static HouseService instance = new HouseService ();
    

    private HouseService ()
    {
    }

    public static HouseService GetInstance ()
    {
      return instance;
    }

    #region House

    /// <summary>
    /// Liefert das zur ID gehörende HouseBean.
    /// <summary>
    public HouseBean RetrieveHouse (int houseId)
    {
      log.Debug ("HouseService.RetrieveHouse");

      return (HouseBean)SQLExecutorFactory.Create ().Execute (true, delegate (SQLExecutor executor)
      {
        IDbCommand command = executor.CreateCommand ();

        String sql =
          " SELECT h_id, h_cleaning_cost, h_clean_min_days_season, h_clean_min_days_total," +
          "        h_bedclothes, h_location, h_name, h_landlord_id" +
          " FROM house WHERE h_id = ?";
        executor.SetCommandText (sql, command);

        executor.AddInt (command, houseId);
        command.Prepare ();
        IDataReader reader = executor.RegisterReader (command);

        HouseBean result = null;
        if (reader.Read ()) {
          LandlordBean landlord = landlordService.RetrieveLandlord (reader.GetInt32 (7));

          result = new HouseBean (
            executor.GetInt32 (reader, 0),
            executor.GetInt32 (reader, 1),
            executor.GetInt32 (reader, 2),
            executor.GetInt32 (reader, 3),
            executor.GetBoolean (reader, 4),
            executor.GetString (reader, 5),
            executor.GetString (reader, 6),
            landlord
          );
        }
        else {
          log.Warn ("house could not be found by id: " + houseId);
        }
        reader.Close ();

        return result;
      });
    }

    /// <summary>
    /// Liefert alle Häuser, die dem übergebenen Vermieter zugeordnet sind.
    /// </summary>
    public KeyValue[] RetrieveHousesByLandlord (int landlordId)
    {
      log.Debug ("HouseService.RetrieveHousesByLandlord");

      log.Debug ("landlordId = " + landlordId);
      SQLExecutor exe = SQLExecutorFactory.Create ();

      String query = " SELECT h_id, " + exe.SQLConcat ("h_name", "' - '", "h_location", "' (ID='", "h_id", "')'") +
                     " FROM house WHERE h_landlord_id = ? ORDER BY h_id";

      return ProcessKeyValue (query, landlordId);
    }

    /// <summary>
    /// Liefert alle Häuser, die dem übergebenen Vermieter zugeordnet sind.
    /// </summary>
    public List<HouseBean> RetrieveHouseBeansByLandlord (int landlordId)
    {
      log.Debug ("HouseService.RetrieveHousesByLandlord");

      return (List<HouseBean>)SQLExecutorFactory.Create ().Execute (true, delegate (SQLExecutor executor)
      {
        LandlordBean landlord = landlordService.RetrieveLandlord (landlordId);

        IDbCommand command = executor.CreateCommand ();

        String sql =
          " SELECT h_id, h_cleaning_cost, h_clean_min_days_season, h_clean_min_days_total," +
          "        h_bedclothes, h_location, h_name" +
          " FROM house" +
          " WHERE h_landlord_id = ?" +
          " ORDER BY h_id";

        executor.SetCommandText (sql, command);
        executor.AddInt (command, landlordId);
        command.Prepare ();
        IDataReader reader = executor.RegisterReader (command);

        List<HouseBean> houses = new List<HouseBean> ();
        while (reader.Read ()) {
          HouseBean house = new HouseBean (
            executor.GetInt32 (reader, 0),
            executor.GetInt32 (reader, 1),
            executor.GetInt32 (reader, 2),
            executor.GetInt32 (reader, 3),
            executor.GetBoolean (reader, 4),
            executor.GetString (reader, 5),
            executor.GetString (reader, 6),
            landlord
          );
          houses.Add (house);
        }
        reader.Close ();

        return houses;
      });
    }

    /// <summary>
    /// Liefert das erste Haus entsprechend der Domain eines Vermieters.
    /// </summary>
    public int RetrieveHouseIdByDomain (String domain)
    {
      log.Debug ("HouseService.RetrieveHouseByDomain");

      return ProcessIntQuery (
        " SELECT MIN(h_id) AS id FROM landlord, house" +
        " WHERE l_domain = ? AND l_id = h_landlord_id", 
        domain);
    }

    /// <summary>
    /// Liefert das erste Haus entsprechend der Domain eines Vermieters und der House-ID (Validierung der Haus-ID).
    /// </summary>
    public int RetrieveHouseIdByDomain (String domain, int houseId)
    {
      log.Debug ("HouseService.RetrieveHouseByDomain");

      return ProcessIntQuery (
        " SELECT MIN(h_id) AS id FROM landlord, house" +
        " WHERE l_domain = ? AND l_id = h_landlord_id AND h_id = ?", 
        domain, houseId);
    }

    /// <summary>
    /// Speichert Änderungen zu einem Ferienhaus.
    /// <summary>
    public void UpdateHouse (HouseBean house)
    {
      log.Debug ("HouseService.UpdateHouse");

      SQLExecutorFactory.Create ().Execute (true, delegate (SQLExecutor executor)
      {
        IDbCommand command = executor.CreateCommand ();

        String sql =
          " UPDATE house SET h_cleaning_cost = ?, h_clean_min_days_season = ?, h_clean_min_days_total = ?," +
          "                  h_bedclothes = ?, h_location = ?" +
          " WHERE h_id = ?";
        executor.SetCommandText (sql, command);

        executor.AddParams (command, house.CleaningCost, house.MinDaysSeason, house.MinDaysTotal,
                                     house.BedClothesHirable, house.Location, house.ID);
        command.Prepare ();
        int rowCount = command.ExecuteNonQuery ();
        log.Debug ("house " + house.ID + " updated = " + (rowCount == 1));

        return null;
      });
    }

    /// <summary>
    /// Speichert Änderungen zu einem Ferienhaus, nur Name und Ort werden verändert.
    /// <summary>
    public void UpdateHouseStandardAttr (HouseBean house)
    {
      log.Debug ("HouseService.UpdateHouse");

      SQLExecutorFactory.Create ().Execute (true, delegate (SQLExecutor executor)
      {
        IDbCommand command = executor.CreateCommand ();

        String sql =
          " UPDATE house SET h_location = ?, h_name = ?" +
          " WHERE h_id = ?";
        executor.SetCommandText (sql, command);

        executor.AddParams (command, house.Location, house.Name, house.ID);
        command.Prepare ();
        int rowCount = command.ExecuteNonQuery ();
        log.Debug ("house " + house.ID + " updated = " + (rowCount == 1));

        return null;
      });
    }

    /// <summary>
    /// Liefert alle Preise, die zum übergebenen Haus und Jahr gespeichert sind, als eine Liste.
    /// <summary>
    public IList RetrieveHousePrices (int houseId, int year)
    {
      log.Debug ("HouseService.RetrieveHousePrices");

      return RetrieveHousePrices (houseId, new DateTime (year, 1, 1), new DateTime (year, 12, 31));
    }

    /// <summary>
    /// Liefert alle Preise, die zum übergebenen Haus und Zeitraum gespeichert sind, als eine Liste.
    /// <summary>
    public IList RetrieveHousePrices (int houseId, DateTime dateFrom, DateTime dateTo)
    {
      log.Debug ("HouseService.RetrieveHousePrices");

      return (IList)SQLExecutorFactory.Create ().Execute (true, delegate (SQLExecutor executor)
      {
        IDbCommand command = executor.CreateCommand ();

        String sql =
          " SELECT hp_id, hp_date_from, hp_date_to, hp_price, hp_peak_season," +
          "        hp_arrival_days, hp_departure_days, hp_min_booking_days" +
          " FROM house_price" +
          " WHERE hp_date_from <= ? AND hp_date_to >= ?" +
          " AND hp_house_id = ?" +
          " ORDER BY hp_date_from";

        executor.SetCommandText (sql, command);

        executor.AddDate (command, dateTo);
        executor.AddDate (command, dateFrom);
        executor.AddInt (command, houseId);
        command.Prepare ();
        IDataReader reader = executor.RegisterReader (command);

        IList prices = new ArrayList ();
        while (reader.Read ()) {
          HousePriceInterval price = new HousePriceInterval (
            reader.GetInt32 (0),
            reader.GetDateTime (1),
            reader.GetDateTime (2),
            reader.GetInt32 (3),
            DBBooleanType.GetBoolean (reader.GetInt32 (4)),
            BookingDays.Create (reader.GetInt32 (5)),
            BookingDays.Create (reader.GetInt32 (6)),
            reader.GetInt32 (7)
          );

          prices.Add (price);
        }

        if (prices.Count == 0) {
          log.Warn ("house prices could not be found by id: " + houseId +
            " and dateFrom: " + dateFrom.ToShortDateString () + " and dateTo: " + dateTo.ToShortDateString ());
        }
        reader.Close ();

        return prices;
      });
    }

    /// <summary>
    /// Liefert alle Preise, die zum übergebenen Haus und Zeitraum gespeichert sind, als eine Liste.
    /// Die Zeiträume werden gruppiert.
    /// <summary>
    public IList RetrieveHousePricesCollapse (int houseId, DateTime dateFrom, DateTime dateTo)
    {
      log.Debug ("HouseService.RetrieveHousePricesCollapse");

      return (IList)SQLExecutorFactory.Create ().Execute (true, delegate (SQLExecutor executor)
      {
        IDbCommand command = executor.CreateCommand ();

        String sql =
          " SELECT date_from, date_to, hp_price, hp_peak_season, hp_arrival_days, hp_departure_days, hp_min_booking_days" +
          " FROM (" +
          "   SELECT MIN(hp_date_from) AS date_from, MAX(hp_date_to) AS date_to," +
          "          hp_price, hp_peak_season, hp_arrival_days, hp_departure_days, hp_min_booking_days" +
          "   FROM house_price" +
          "   WHERE hp_date_from <= ? AND hp_date_to >= ?" +
          "   AND hp_house_id = ?" +
          "   GROUP BY hp_price, hp_peak_season, hp_arrival_days, hp_departure_days, hp_min_booking_days" +
          " )" +
          " ORDER BY date_from";

        executor.SetCommandText (sql, command);

        executor.AddDate (command, dateTo);
        executor.AddDate (command, dateFrom);
        executor.AddInt (command, houseId);
        command.Prepare ();
        IDataReader reader = executor.RegisterReader (command);

        IList prices = new ArrayList ();
        while (reader.Read ()) {
          HousePriceInterval price = new HousePriceInterval (
            0,
            reader.GetDateTime (0),
            reader.GetDateTime (1),
            reader.GetInt32 (2),
            DBBooleanType.GetBoolean (reader.GetInt32 (3)),
            BookingDays.Create (reader.GetInt32 (4)),
            BookingDays.Create (reader.GetInt32 (5)),
            reader.GetInt32 (6)
          );

          prices.Add (price);
        }

        if (prices.Count == 0) {
          log.Warn ("house prices could not be found by id: " + houseId +
            " and dateFrom: " + dateFrom.ToShortDateString () + " and dateTo: " + dateTo.ToShortDateString ());
        }
        reader.Close ();

        return prices;
      });
    }

    /// <summary>
    /// Speichert alle Preis-Änderungen zu einem Ferienhaus.
    /// <summary>
    public void UpdateHousePrices (HouseBean house, IList prices, IList removedPrices)
    {
      log.Debug ("HouseService.UpdateHousePrices");

      SQLExecutorFactory.Create ().Execute (true, delegate (SQLExecutor executor)
      {
        // Update Command für mehrmaligen Aufruf vorbereiten
        IDbCommand updateCmd = executor.CreateCommand ();
        String sql =
          " UPDATE house_price SET hp_date_from = ?, hp_date_to = ?, hp_price = ?, hp_peak_season = ?," +
          "   hp_arrival_days = ?, hp_departure_days = ?, hp_min_booking_days = ? " +
          " WHERE hp_id = ?";
        executor.SetCommandText (sql, updateCmd);

        executor.AddParams (updateCmd, DateTime.Now, DateTime.Now, -1, (int)DBBoolean.FALSE, -1, 0, 0, -1);
        updateCmd.Prepare ();

        foreach (HousePriceInterval interval in prices) {
          if (!interval.IsNew ()) {
            // Parameter setzen
            executor.SetParameterValue (updateCmd, 0, interval.Start);
            executor.SetParameterValue (updateCmd, 1, interval.End);
            executor.SetParameterValue (updateCmd, 2, interval.Price);
            executor.SetParameterValue (updateCmd, 3, (int)DBBooleanType.GetDBBoolean (interval.PeakSeason));
            executor.SetParameterValue (updateCmd, 4, interval.ArrivalDays.BuildDayListNumber ());
            executor.SetParameterValue (updateCmd, 5, interval.DepartureDays.BuildDayListNumber ());
            executor.SetParameterValue (updateCmd, 6, interval.MinBookingDays);
            executor.SetParameterValue (updateCmd, 7, interval.ID);

            int rowCount = updateCmd.ExecuteNonQuery ();
            log.Debug ("price interval updated = " + (rowCount == 1));
          }
        }

        // Insert Command für mehrmaligen Aufruf vorbereiten
        IDbCommand insertCmd = executor.CreateCommand ();
        sql =
          " INSERT INTO house_price (hp_date_from, hp_date_to, hp_price, hp_peak_season," +
          "   hp_arrival_days, hp_departure_days, hp_min_booking_days, hp_house_id)" +
          " VALUES (?, ?, ?, ?, ?, ?, ?, ?)";
        executor.SetCommandText (sql, insertCmd);

        executor.AddParams (
          insertCmd, DateTime.Now, DateTime.Now, -1, (int)DBBoolean.FALSE, 0, 0, -1, house.ID);
        insertCmd.Prepare ();

        foreach (HousePriceInterval interval in prices) {
          if (interval.IsNew ()) {
            // Parameter setzen
            executor.SetParameterValue (insertCmd, 0, interval.Start);
            executor.SetParameterValue (insertCmd, 1, interval.End);
            executor.SetParameterValue (insertCmd, 2, interval.Price);
            executor.SetParameterValue (insertCmd, 3, (int)DBBooleanType.GetDBBoolean (interval.PeakSeason));
            executor.SetParameterValue (insertCmd, 4, interval.ArrivalDays.BuildDayListNumber ());
            executor.SetParameterValue (insertCmd, 5, interval.DepartureDays.BuildDayListNumber ());
            executor.SetParameterValue (insertCmd, 6, interval.MinBookingDays);

            int rowCount = insertCmd.ExecuteNonQuery ();
            log.Debug ("price interval inserted = " + (rowCount == 1));
          }
        }

        // Löschen der entfernten und nicht neuen Preise
        if (removedPrices.Count > 0) {
          // Delete Command für mehrmaligen Aufruf vorbereiten
          IDbCommand deleteCmd = executor.CreateCommand ();
          sql = " DELETE FROM house_price WHERE hp_id = ?";
          executor.SetCommandText (sql, deleteCmd);

          executor.AddParams (deleteCmd, -1);
          updateCmd.Prepare ();

          foreach (HousePriceInterval interval in removedPrices) {
            if (!interval.IsNew ()) {
              // Parameter setzen
              executor.SetParameterValue (deleteCmd, 0, interval.ID);

              int rowCount = deleteCmd.ExecuteNonQuery ();
              log.Debug ("price interval deleted = " + (rowCount == 1));
            }
          }
        }

        return null;
      });

      // Cache mit Preisen löschen
      CacheHandler.Instance.RemoveAllHousePriceIntervalLists ();
    }

    /// <summary>
    /// Erstellt ein neues Vermietungsobjekt.
    /// <summary>
    public void InsertHouse (HouseBean house)
    {
      log.Debug ("HouseService.InsertHouse");

      SQLExecutorFactory.Create ().Execute (true, delegate (SQLExecutor executor)
      {
        IDbCommand command = executor.CreateCommand ();

        String sql =
          " INSERT INTO house (h_cleaning_cost, h_clean_min_days_season, h_clean_min_days_total," +
          " h_bedclothes, h_location, h_name, h_landlord_id)" +
          " VALUES (?,?,?,?,?,?,?)";

        executor.SetCommandText (sql, command);
        executor.AddParams (command, house.CleaningCost, house.MinDaysSeason, house.MinDaysTotal, 
                            house.BedClothesHirable, house.Location, house.Name, house.Landlord.ID);
        command.Prepare();
        int rowCount = command.ExecuteNonQuery ();
        log.Debug ("house inserted = " + (rowCount == 1));

        int id = executor.RetrieveIdentity ();
        house.ID = id;
        
        return null;
      });
    }

    /// <summary>
    /// Löscht ein Vermietungsobjekt inklusive Buchungen und Preise.
    /// </summary>
    public void DeleteHouse (HouseBean house)
    {
      log.Debug ("HouseService.DeleteHouse");

      int count = ProcessUpdate ("DELETE FROM house WHERE h_id = ?", house.ID);

      log.Debug ("house " + house.ID + " deleted: " + (count == 1));
    }

    #endregion
  }
}
