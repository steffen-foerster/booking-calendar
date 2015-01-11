namespace de.fiok.service
{
  using System;
  using System.Data;
  using System.Collections;
  using de.fiok.core;
  using de.fiok.state;
  using de.fiok.type;
  using log4net;

  /// <summary>
  /// Service enthält Business-Logik zum Thema Berechnung des Mietpreises und Prüfung von
  /// Zeiträumen hinsichtlich Belegung. Weiterhin wird über diesen Service eine Reservierung 
  /// durchgeführt.
  /// </summary>
  /// <remarks>
	/// created by - Steffen Förster
	/// </remarks>
  public class BookingService : BaseService
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(BookingService));

    private static readonly int MAX_NOTES_LENGTH = 1000;

    private static readonly int MAX_AGE_CHILDREN_LENGTH = 255;

    private static MailService mailService = MailService.GetInstance ();
    private static HouseService houseService = HouseService.GetInstance ();
    private static BookingService instance = new BookingService ();

    public static readonly int BLOCK_ONLINE_BOOKING_PERIOD = 5;

    private BookingService ()
    {
    }

    public static BookingService GetInstance ()
    {
      return instance;
    }
    
    #region Zeitraum Validierung
    
    /// <summary>
    /// Prüfung, ob der Zeitraum gültig und noch frei ist.
    /// </summary>
    public ValidationResult ValidatePeriod (DateTime arrivalDate, DateTime departureDate, int houseId)
    {
      log.Debug ("BookingService.ValidatePeriod");
    
      bool result = true;
      String msg = null;
      DateTime now = new DateTime (DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

      // wenn noch kein Datum gewählt wurde -> keine Prüfung durchführen
      if (arrivalDate != DateTime.MinValue && departureDate != DateTime.MinValue) {
        // Anreise liegt vor aktuellem Datum + X Tage ?
        if (arrivalDate < now.AddDays (BLOCK_ONLINE_BOOKING_PERIOD)) {
          msg = AppResources.GetMessage("BookingService_msg.arrival.blocking.period", BLOCK_ONLINE_BOOKING_PERIOD);
          result = false;
        }
        // Abreise liegt vor Anreise ?
        else if (arrivalDate >= departureDate) {
          msg = AppResources.GetMessage("BookingService_msg.departure.before.arrival");
          result = false;
        }
        // Zeitraum schon belegt ?
        else if (! IsPeriodFree (arrivalDate, departureDate, houseId)) {
          msg = AppResources.GetMessage("BookingService_msg.period.occupied");
          result = false;
        }  
        // für den gesamten Zweitraum sind Preise festgelegt ?
        else if (! IsPricesDefined (arrivalDate, departureDate, houseId)) {
          msg = AppResources.GetMessage("BookingService_msg.no.price");
          result = false;
          log.Warn ("no price for period: " + arrivalDate.ToShortDateString () + "-" + departureDate.ToShortDateString ());
        }
        else {
          int totalDays = GetTotalDays (arrivalDate, departureDate);

          // Dauer des Aufenthalts >= minimaler Aufenthalt der Saison
          // -> bei Überschneidung einer Intervall-Grenze wird die minimale Aufenthaltsdauer zu Grunde gelegt
          String sql =
            " SELECT MIN(hp_min_booking_days) FROM house_price" +
            " WHERE hp_house_id = ?" +
            " AND hp_date_from <= ? AND hp_date_to >= ?";
          int minBookingDays = ProcessIntQuery (sql, houseId, departureDate, arrivalDate);

          if (totalDays < minBookingDays) {
            msg = AppResources.GetMessage("BookingService_msg.min.booking.days", minBookingDays);
            result = false;
          }
          // An- und Abreise-Tage prüfen
          else {
            sql =
              " SELECT hp_arrival_days FROM house_price" +
              " WHERE hp_house_id = ?" +
              " AND hp_date_from <= ? AND hp_date_to >= ?";
            BookingDays arrivalDays = BookingDays.Create (ProcessIntQuery (sql, houseId, arrivalDate, arrivalDate));
            sql =
              " SELECT hp_departure_days FROM house_price" +
              " WHERE hp_house_id = ?" +
              " AND hp_date_from <= ? AND hp_date_to >= ?";
            BookingDays departureDays = BookingDays.Create (ProcessIntQuery (sql, houseId, departureDate, departureDate));

            if (!arrivalDays.Contains (arrivalDate.DayOfWeek) || !departureDays.Contains (departureDate.DayOfWeek)) {
              msg = AppResources.GetMessage("BookingService_msg.arrival.departure.days",
                arrivalDays.BuildDayListString, departureDays.BuildDayListString);
              result = false;
            }
          }
        }
      }

      return new ValidationResult (result, msg);
    }

    /// <summary>
    /// Prüft, ob für alle Tage des übergebenen Zeitraums ein Preis hinterlegt ist.
    /// </summary>
    private bool IsPricesDefined (DateTime dateFrom, DateTime dateTo, int houseId) 
    {
      log.Debug ("BookingController.IsPricesDefined");

      IList prices = houseService.RetrieveHousePrices (houseId, dateFrom, dateTo);

      if (prices.Count == 0) {
        log.Debug ("---> count == 0");
        return false;
      }

      DateTime currentDate = new DateTime (dateFrom.Year, dateFrom.Month, dateFrom.Day);
      DateTime stopDate = new DateTime (dateTo.Year, dateTo.Month, dateTo.Day);
    
      while (currentDate <= dateTo) {
        bool contains = false;
        foreach (HousePriceInterval interval in prices) {
          if (interval.Contains (currentDate)) {
            contains = true;
          }
        }
        
        // für ein Datum wurde kein Preis gefunden
        if (! contains) {
          log.Debug ("no price for date: " + currentDate.ToShortDateString ());
          return false;
        }
      
        currentDate = currentDate.AddDays (1);
      }

      return true;
    }
    
    #endregion
    
    #region Mietpreis-Berechnung

    /// <summary>
    /// Mietpreis berechnen, als Grundlage dient hierfür der gewählte Zeitraum und das
    /// gewählte Haus.
    /// </summary>
    public MessageResult CalculatePrice (DateTime arrival, DateTime departure, int houseId)
    {
      log.Debug ("BookingService.CalculatePrice");
      
      // für den gesamten Zweitraum sind Preise festgelegt ?
      if (! IsPricesDefined (arrival, departure, houseId)) {
        String msg = AppResources.GetMessage("BookingService_msg.no.price");
        log.Warn ("no price for period: " + arrival.ToShortDateString () + "-" + departure.ToShortDateString ());
        return new MessageResult (false, msg);
      }

      PriceBean result = new PriceBean ();
      HouseBean house = houseService.RetrieveHouse (houseId);
      result.House = house;
      
      // Preis-Informationen für das Haus aus der Datenbank holen
      IList priceList = CreateHousePriceIntervals (houseId);

      // Mietdauer
      result.TotalDays = GetTotalDays (arrival, departure);

      // Miete
      result.Rent = CalculateRentByIntervals (arrival, departure, priceList, house);

      // Reinigungskosten
      result.CleaningCost = house.CleaningCost;

      // Reinigung inklusive
      // a) Mietdauer
      if (result.TotalDays >= house.MinDaysTotal) {
        result.CleaningDays = true;
        result.CleaningCost = 0;
      }
      // b) Anzahl Tage die in der Hauptsaison liegen -> Übernahme der Reinigung
      else if (CalculateCountPeakSeasonDays (arrival, departure, priceList) >= house.MinDaysSeason) {
        result.CleaningSeason = true;
        result.CleaningCost = 0;
      }

      // Gesamtpreis
      result.TotalCost = result.Rent + result.CleaningCost;
      
      return new MessageResult (true, null, result);
    }

    /// <summary>
    /// Mietdauer berechnen.
    /// </summary> 
    private int GetTotalDays (DateTime arrival, DateTime departure)
    {
      log.Debug ("BookingService.GetTotalDays");
    
      return (int)(departure - arrival).TotalDays;
    }

    /// <summary>
    /// Mietpreis berechnen entsprechend der Mietdauer, unter Berücksichtigung der
    /// verschiedenen Preise.
    /// </summary>  
    private int CalculateRentByIntervals (DateTime arrival, DateTime departure, IList priceList, HouseBean house)
    {
      log.Debug ("BookingService.CalculateRentByIntervals");
    
      int rent = 0;
      while (arrival < departure) {
        rent += GetPriceForDate (arrival, priceList, house);
        arrival = arrival.AddDays (1);
      }

      return rent;
    }

    /// <summary>
    /// Liefert die Anzahl der Tage, die in der Hochsaison liegen.
    /// </summary>   
    private int CalculateCountPeakSeasonDays (DateTime arrival, DateTime departure, IList priceList)
    {
      log.Debug ("BookingService.CalculateCountPeakSeasonDays");
    
      int days = 0;
      while (arrival < departure) {
        if (IsPeakSeason (arrival, priceList)) {
          days ++;
        }
        arrival = arrival.AddDays (1);
      }

      return days;
    }

    /// <summary>
    /// Liefert den Mietpreis für ein bestimmtes Datum.
    /// </summary>  
    private int GetPriceForDate (DateTime date, IList priceList, HouseBean house)
    {
      log.Debug ("BookingService.GetPriceForDate");
    
      IEnumerator it = priceList.GetEnumerator ();
      while (it.MoveNext ()) {
        HousePriceInterval interval = (HousePriceInterval)it.Current;
        if (interval.Contains (date)) {
          return interval.Price;
        }
      }

      // Mail an den Vermieter senden, dass für einen gewünschten Tag noch keine Preise hinterlegt wurden
      try {
        SendMissingPriceMail (house, date);
      }
      catch (Exception e) {
        log.Error (e, e);
      }

      throw new Exception ("no price for date: " + date.ToShortDateString ());
    }
    
    /// <summary>
    /// Sendet eine Information an den Vermieter, dass ein Preis noch nicht hinterlegt ist.
    /// </summary>
    private void SendMissingPriceMail (HouseBean house, DateTime date)
    {
      log.Debug ("BookingService.SendMissingPriceMail");

      String msgBody = AppResources.GetMessage("Booking_mail.missing.price.text", 
                                             house.ID, house.Location, date.ToShortDateString ());

      String subject = AppResources.GetMessage("Booking_mail.missing.price.subject", date.ToShortDateString());
      
      mailService.SendMail (subject, msgBody, house.Landlord.Email);
    }

    /// <summary>
    /// Liefert true, wenn das übergebene Datum in der Hochsaison liegt.
    /// </summary>
    private bool IsPeakSeason (DateTime date, IList priceList)
    {
      log.Debug ("BookingService.IsPeakSeason");
    
      IEnumerator it = priceList.GetEnumerator ();
      while (it.MoveNext ()) {
        HousePriceInterval interval = (HousePriceInterval)it.Current;
        if (interval.Contains (date)) {
          return interval.PeakSeason;
        }
      }

      throw new Exception ("no cleaning rule for date: " + date.ToShortDateString ());
    }
    
    /// <summary>
    /// Holt aus der Datenbank für ein Haus die Preisinformationen (Intervalle mit Preisen).
    /// </summary>
    public IList CreateHousePriceIntervals (int houseId)
    {
      log.Debug ("BookingService.CreateHousePriceIntervals");
      
      // Preis-Informationen aus dem Cache zurückliefern
      if (CacheHandler.Instance.ContainsHousePriceIntervalList (houseId)) {
        
        return CacheHandler.Instance.GetHousePriceIntervalList (houseId);
      }
      // noch keine Preis-Informationen zum Haus im Cache gespeichert
      else {
        IList priceList = (IList)SQLExecutorFactory.Create ().Execute (true, delegate(SQLExecutor executor)
        {
          IDbCommand command = executor.CreateCommand ();

          String sql = 
            " SELECT hp_date_from, hp_date_to, hp_price, hp_peak_season," +
            "        hp_arrival_days, hp_departure_days, hp_min_booking_days"+
            " FROM house_price" +
            " WHERE hp_house_id = ? AND hp_date_to >= ?" +
            " ORDER BY hp_date_from";
          executor.SetCommandText (sql, command);

          executor.AddInt (command, houseId);
          executor.AddDate (command, DateTime.Now);
          command.Prepare();
          IDataReader reader = executor.RegisterReader (command);

          IList priceIntervals = new ArrayList ();
          while (reader.Read()) {
            DateTime dateFrom = reader.GetDateTime (0);
            DateTime dateTo = reader.GetDateTime (1);
            
            HousePriceInterval price = new HousePriceInterval (
              new DateTime (dateFrom.Year, dateFrom.Month, dateFrom.Day), 
              new DateTime (dateTo.Year, dateTo.Month, dateTo.Day), 
              reader.GetInt32 (2), 
              DBBooleanType.GetBoolean (reader.GetInt32 (3)),
              BookingDays.Create (reader.GetInt32 (4)),
              BookingDays.Create (reader.GetInt32 (5)),
              reader.GetInt32 (6)
            );
            
            log.Debug ("adding price " + price);
            priceIntervals.Add (price);
          }
          reader.Close ();
          
          log.Debug (priceIntervals.Count + " price intervals found for house " + houseId);

          return priceIntervals;  
        });

        // Preis-Informationen im Cache speichern
        CacheHandler.Instance.AddHousePriceIntervalList (priceList, houseId);
        
        return priceList;
      }
    }

    #endregion

    #region Ermittlung des Belegungs-Status für ein Haus
    
    /// <summary>
    /// Holt aus der Datenbank für ein Haus zu einem Monat die Reservierungstage aus der
    /// Datenbank und erstellt ein BookingState Objekt. Vorher wird mittels des CacheHandlers versucht,
    /// den BookingState aus dem Cache zu laden.
    /// </summary>
    public MonthOccupancy CreateMonthOccupancy (int houseId, int year, int month)
    {
      log.Debug ("BookingService.CreateMonthOccupancy");
    
      if (CacheHandler.Instance.ContainsMonthOccupancy (year, month, houseId)) {
        // BookingState aus dem Cache zurückliefern
        return CacheHandler.Instance.GetMonthOccupancy (year, month, houseId);
      }
      else {
        // noch kein BookingState mit den aktuellen Parametern im Cache vorhanden
        DateTime startDate = new DateTime (year, month, 1);
        DateTime endDate = startDate.AddMonths (1);
        MonthOccupancy occupancy = new MonthOccupancy (year, month, houseId);
  
        SQLExecutorFactory.Create ().Execute (true, delegate (SQLExecutor executor)
        {
          IDbCommand command = executor.CreateCommand ();

          String sql =
            " SELECT bd_date, MAX(b_status), MAX(bd_arrival), MAX(bd_departure)" +
            " FROM booking_view" +
            " WHERE b_house_id = ?" +
            " AND bd_date >= ? AND bd_date < ?" +
            " GROUP BY bd_date";
          executor.SetCommandText (sql, command);

          executor.AddInt (command, occupancy.HouseId);
          executor.AddDate (command, startDate);
          executor.AddDate (command, endDate);
          command.Prepare();
          IDataReader reader = executor.RegisterReader (command);

          while (reader.Read()) {
            DateTime date = reader.GetDateTime (0);

            occupancy.AddBookingDay (
              new DateTime (date.Year, date.Month, date.Day),
              reader.GetInt32 (1) == 1,
              reader.GetInt32 (2) == 1,
              reader.GetInt32 (3) == 1
            );
          }
          reader.Close ();

          return null;
        });
        
        // state im Cache speichern
        CacheHandler.Instance.AddMonthOccupancy (occupancy);
        
        return occupancy;
      }
    }
    
    #endregion
    
    #region Ermitteln, ob der gewählte Zeitraum frei ist
    
    /// <summary>
    /// Prüft, ob ein Haus zum übergebenen Zeitpunkt noch frei ist. Ein Haus ist frei, wenn
    ///
    /// 1. kein Tag im gewünschten Zeitraum auf einen Tag fällt, der gebucht ist,
    ///    und an dem keine Ab- oder Anreise stattfindet
    ///
    /// und
    ///
    /// 2. kein Tag im gewünschten Zeitraum auf einen Tag fällt, der gebucht ist,
    ///    und an dem eine Ab- und eine Anreise stattfindet
    /// </summary>
    public bool IsPeriodFree (DateTime arrival, DateTime departure, int houseId)
    {
      log.Debug ("BookingService.IsPeriodFree");
 
      String query = " SELECT count(*) FROM booking_view" +
                     " WHERE b_house_id = ?" +
                     " AND (" +
                     "   bd_arrival = 0 AND bd_departure = 0 AND bd_date >= ? AND bd_date <= ?" +
                     "   OR bd_arrival = 1 AND bd_date = ?" +
                     "   OR bd_departure = 1 AND bd_date = ?" +
                     " )";
 
      return ! ExistsRecord (query, houseId, arrival, departure, arrival, departure);   
    }

    #endregion
    
    #region Ausführen einer Reservierung
    
    /// <summary>
    /// Reservierung ausführen
    /// 1. Reservierung prüfen. 
    /// 2. Reservierungsdaten in Datenbank speichern.
    /// 3. E-Mail an den zukünftigen Mieter senden.
    /// 4. E-Mail an den Vermieter senden, damit dieser die Reservierung prüft.
    /// </summary>
    public MessageResult PerformBooking (BookingData bookingData, TenantEntryData tenantData)
    {
      log.Debug ("BookingService.PerformBooking");
      
      MessageResult result = null;
      
      // Buchung kann nur von einem Thread ausgeführt werden
      lock (instance) {
        // Validierung des Zeitraums
        ValidationResult valid = ValidatePeriod (bookingData.ArrivalDate, bookingData.DepartureDate, 
                                                 bookingData.HouseID);
                          
        if (! valid.Valid) {
          result = new MessageResult (false, valid.Message);
        }
        else {
          try {
            // Buchung wird nur gespeichert, wenn auch E-Mail an den Vermieter abgesendet werden konnte
            SQLExecutorFactory.Create ().Execute (true, delegate (SQLExecutor executor)
            {
              // Reservierung in Datenbank speichern
              BookingItem item = new BookingItem ();
              item.Tenant = tenantData.Tenant;
              item.Notes = tenantData.Notes;
              item.CountAdults = tenantData.AdultCount;
              item.CountChildren = tenantData.ChildrenCount;
              item.BedClothes = tenantData.BedClothes;
              item.PromotionPartner = tenantData.PromotionPartner;
              item.AgeChildren = tenantData.AgeChildren;

              item.Arrival = bookingData.ArrivalDate;
              item.Departure = bookingData.DepartureDate;
              item.Price = bookingData.Price;
              item.HouseID = bookingData.HouseID;
              
              item.Status = BookingStatus.RESERVED;
              
              InsertBooking (item, executor);
        
              // E-Mail an den Vermieter senden
              SendLandlordNotificationMail (bookingData, tenantData);

              return null;
            });

            // Cache mit den Reservierungstagen löschen
            CacheHandler.Instance.RemoveAllMonthOccupancies ();
        
            // E-Mail an den Mieter senden, wenn eine Mail-Adresse angegeben wurde
            try {
              if (! String.IsNullOrEmpty (tenantData.Tenant.Email)) {
                SendTenantNotificationMail (bookingData, tenantData);
              }
              result = new MessageResult(true, AppResources.GetMessage("Confirmation_txt.success"));
            }
            // schlägt dies fehl, so wird die Reservierung trotzdem ausgeführt
            catch (Exception e) {
              log.Debug (e, e);
              result = new MessageResult(true, AppResources.GetMessage("Confirmation_txt.success.error.sending.email"));
            }  
          }
          // die Reservierung konnte nicht erfolgreich durchgeführt werden
          catch (Exception e) {
            log.Debug (e, e);
            result = new MessageResult(false, AppResources.GetMessage("Confirmation_txt.error.booking"));
          }
        }
      }
      
      return result;
    }
    
    /// <summary>
    /// Sendet eine Reservierungsbestätigung an den Mieter.
    /// </summary>
    private void SendTenantNotificationMail (BookingData bookingData, TenantEntryData tenantData)
    {
      log.Debug ("BookingService.SendTenantNotificationMail");
      
      HouseBean house = houseService.RetrieveHouse (bookingData.HouseID);

      String msgBody;
      
      if (tenantData.Tenant.Salutation == Salutation.MR) {
        msgBody = AppResources.GetMessage("Confirmation_mail.tenant.salutation.mr", tenantData.Tenant.Name);
      }
      else {
        msgBody = AppResources.GetMessage("Confirmation_mail.tenant.salutation.mrs", tenantData.Tenant.Name);
      }

      msgBody += AppResources.GetMessage("Confirmation_mail.tenant.text", 
                                       bookingData.ArrivalDate, bookingData.DepartureDate,
                                       bookingData.Price.TotalCost);

      String subject = AppResources.GetMessage("Confirmation_mail.tenant.subject", house.Location);
      
      mailService.SendMail (subject, msgBody, tenantData.Tenant.Email);
    }
    
    /// <summary>
    /// Sendet eine Information an den Vermieter.
    /// </summary>
    private void SendLandlordNotificationMail (BookingData bookingData, TenantEntryData tenantData)
    {
      log.Debug ("BookingService.SendLandlordNotificationMail");
      
      HouseBean house = houseService.RetrieveHouse (bookingData.HouseID);

      String msgBody = AppResources.GetMessage("Confirmation_mail.landlord.text", 
                                             house.Location, house.ID, DateTime.Now,
                                             bookingData.ArrivalDate, bookingData.DepartureDate,
                                             bookingData.Price.TotalCost, 
                                             SalutationType.GetName (tenantData.Tenant.Salutation),
                                             tenantData.Tenant.Firstname, tenantData.Tenant.Name);

      String subject = AppResources.GetMessage("Confirmation_mail.landlord.subject", DateTime.Now);
      
      mailService.SendMail (subject, msgBody, house.Landlord.Email);
    }
    
    /// <summary>
    /// Speichert eine Reservierung in der Datenbank.
    /// Achtung: Damit die Änderungen gespeichert werden, muss 
    /// </summary>
    public void InsertBooking (BookingItem item, SQLExecutor executor)
    {
      log.Debug ("BookingService.InsertBooking");
    
      // ------ 1. Einfügen eines neuen Mieters 
      IDbCommand command;
      int rowCount;
      
      log.Debug ("insert tenant ...");
      command = executor.CreateCommand ();
      String sql = 
        " INSERT INTO tenant (t_name, t_firstname, t_street, t_zipcode, t_location," +
        "                     t_email, t_telephone, t_fax, t_title, t_salutation)" +
        " VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
      executor.SetCommandText (sql, command);

      executor.AddString (command, item.Tenant.Name);
      executor.AddString (command, item.Tenant.Firstname);
      executor.AddString (command, item.Tenant.Street);
      executor.AddString (command, item.Tenant.Zipcode);
      executor.AddString (command, item.Tenant.Location);
      executor.AddString (command, item.Tenant.Email);
      executor.AddString (command, item.Tenant.Telephone);
      executor.AddString (command, item.Tenant.Fax);
      executor.AddString (command, item.Tenant.Title);
      executor.AddInt (command, (Int32)item.Tenant.Salutation);
              
      command.Prepare();
      rowCount = command.ExecuteNonQuery ();
      log.Debug ("tenant inserted = " + (rowCount == 1));
      
      // Autowert ermitteln
      item.Tenant.ID = executor.RetrieveIdentity ();

      // ------ 2. Einfügen einer neuen Reservierung
      log.Debug ("insert booking ...");
      command = executor.CreateCommand ();
      sql = 
        " INSERT INTO booking (b_tenant_id, b_house_id, b_status, b_date," +
        "                      b_price, b_cleaning_cost, b_notes, b_count_adults, b_count_children," +
        "                      b_bedclothes, b_promotion_partner, b_age_children)" +
        " VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
      executor.SetCommandText (sql, command);

      executor.AddInt (command, item.Tenant.ID);
      executor.AddInt (command, item.HouseID);
      executor.AddInt (command, (int)item.Status);
      executor.AddDate (command, DateTime.Now);
      executor.AddInt (command, item.Price.Rent);
      executor.AddInt (command, item.Price.CleaningCost);
      executor.AddString (command, item.Notes, MAX_NOTES_LENGTH);
      executor.AddInt (command, item.CountAdults);
      executor.AddInt (command, item.CountChildren);
      executor.AddBoolean (command, item.BedClothes);
      executor.AddInt (command, item.PromotionPartner);
      executor.AddString(command, item.AgeChildren, MAX_AGE_CHILDREN_LENGTH);
              
      command.Prepare();
      rowCount = command.ExecuteNonQuery ();
      log.Debug ("booking inserted = " + (rowCount == 1));
      
      // Autowert ermitteln
      item.ID = executor.RetrieveIdentity ();
     
      // ------ 3. Einfügen der reservierten Tage
      log.Debug ("insert booking days...");
      
      // Insert Command für mehrmaligen Aufruf vorbereiten
      IDbCommand insertCmd = executor.CreateCommand ();
      sql = 
        " INSERT INTO booking_days (bd_id, bd_date, bd_occupied, bd_arrival, bd_departure)" +
        " VALUES (?, ?, ?, ?, ?)";
      executor.SetCommandText (sql, insertCmd);
          
      executor.AddParams (insertCmd, item.ID, DateTime.Now, (int)DBBoolean.TRUE, (int)DBBoolean.FALSE, (int)DBBoolean.FALSE);
      command.Prepare();
      
      // Buchungsätze für jeden Buchungstag in die Datenbank einfügen
      DateTime bookingDate = item.Arrival;
      while (bookingDate <= item.Departure) {

        int arrivalFlag = (int)DBBooleanType.GetDBBoolean (bookingDate.CompareTo (item.Arrival) == 0);
        int departureFlag = (int)DBBooleanType.GetDBBoolean (bookingDate.CompareTo (item.Departure) == 0);
        
        // neuen Datensatz einfügen
        executor.SetParameterValue (insertCmd, 1, bookingDate);
        executor.SetParameterValue (insertCmd, 3, (int)arrivalFlag);
        executor.SetParameterValue (insertCmd, 4, (int)departureFlag);
          
        rowCount = insertCmd.ExecuteNonQuery ();
        log.Debug ("booking day inserted = " + (rowCount == 1));
        
        // Buchungsdatum einen Tag hochzählen
        bookingDate = bookingDate.AddDays (1);
      }
    }

    #endregion
    
    #region Laden und bearbeiten von Buchungen/Reservierungen
    
    /// <summary>
    /// Liefert alle Buchungen, die den Suchkriterien entsprechen, als eine Liste.
    /// <summary>
    public IList RetrieveBookings (int houseId, DateTime dateFrom, DateTime dateTo, int[] status)
    {
      log.Debug ("BookingService.RetrieveBookings");

      return (IList)SQLExecutorFactory.Create ().Execute (true, delegate (SQLExecutor executor)
      {
        // Suchkriterien auswerten
        String search = "";
        ArrayList searchParams = new ArrayList ();
        searchParams.Add (houseId);
        
        if (dateFrom > DateTime.MinValue) {
          search += " AND (SELECT MIN(bd_date) FROM booking_days WHERE bd_id = b_id) >= ?";
          searchParams.Add (dateFrom);
        }
        if (dateTo > DateTime.MinValue) {
          search += " AND (SELECT MAX(bd_date) FROM booking_days WHERE bd_id = b_id) <= ?";
          searchParams.Add (dateTo);
        }
        if (status.Length > 0) {
          search += " AND b_status IN (" + CreateParamWildcats (status.Length) + ")";
          for (int i = 0; i < status.Length; i++) {
            searchParams.Add (status[i]);
          }
        }

        IDbCommand command = executor.CreateCommand ();
        
        String sql =
          " SELECT * FROM (" +
          "   SELECT b_id, b_date, b_status, b_notes, b_count_adults, b_count_children, b_age_children," +
          "          b_bedclothes, b_promotion_partner, b_price, b_cleaning_cost, t_name, t_firstname, t_street, t_zipcode, t_location," +
          "          t_email, t_telephone, t_fax, t_title, t_salutation, t_id," +
          "          (SELECT MAX(bd_date) FROM booking_days WHERE bd_id = b_id) AS departure," +
          "          (SELECT MIN(bd_date) FROM booking_days WHERE bd_id = b_id) AS arrival" + 
          "   FROM booking, tenant" +
          "   WHERE b_house_id = ? " +
          "   AND b_tenant_id = t_id" +
          search + 
          " )" +
          " ORDER BY arrival";
        executor.SetCommandText (sql, command);      

        executor.AddParams (command, (Object[])searchParams.ToArray (typeof (Object)));
        command.Prepare();
        IDataReader reader = executor.RegisterReader (command);
        
        IList bookings = new ArrayList ();
        while (reader.Read()) {
          int index = 0;
          BookingItem item = new BookingItem ();
          item.HouseID = houseId;
          
          item.ID = reader.GetInt32 (index++);
          item.BookingDate = reader.GetDateTime (index++);
          item.Status = (BookingStatus)reader.GetInt32 (index++);
          item.Notes = executor.GetString (reader, index++);
          item.CountAdults = reader.GetInt32 (index++);
          item.CountChildren = reader.GetInt32 (index++);
          item.AgeChildren = executor.GetString(reader, index++);
          item.BedClothes = executor.GetBoolean (reader, index++);
          item.PromotionPartner = executor.GetInt32(reader, index++);
          
          // Preis
          PriceBean price = new PriceBean ();
          price.Rent = reader.GetInt32 (index++);
          price.CleaningCost = reader.GetInt32 (index++);
          item.Price = price;
                   
          // Mieter 
          TenantBean tenant = new TenantBean ();
          tenant.Name = executor.GetString (reader, index++);
          tenant.Firstname = executor.GetString (reader, index++);
          tenant.Street = executor.GetString (reader, index++);
          tenant.Zipcode = executor.GetString (reader, index++);
          tenant.Location = executor.GetString (reader, index++);
          tenant.Email = executor.GetString (reader, index++);
          tenant.Telephone = executor.GetString (reader, index++);
          tenant.Fax = executor.GetString (reader, index++);
          tenant.Title = executor.GetString (reader, index++);
          int salutation = executor.GetInt32 (reader, index++);
          tenant.Salutation = (Salutation)salutation;
          tenant.ID = executor.GetInt32 (reader, index++);
          item.Tenant = tenant;
          
          // Zeitraum
          item.Departure = reader.GetDateTime (index++);
          item.Arrival = reader.GetDateTime (index++);
          
          bookings.Add (item);
        }
        
        if (bookings.Count == 0) {
          log.Info ("bookings could not be found by id: " + houseId);
        }
        reader.Close ();

        return bookings;
      });
    }

    /// <summary>
    /// Liefert die Daten zu einer Buchung mit der übergebenen Buchungsnummer.
    /// <summary>
    public BookingItem RetrieveBooking (int bookingId)
    {
      log.Debug ("BookingService.RetrieveBooking");

      return (BookingItem)SQLExecutorFactory.Create ().Execute (true, delegate (SQLExecutor executor)
      {
        IDbCommand command = executor.CreateCommand ();

        String sql =
          " SELECT b_id, b_date, b_status, b_notes, b_count_adults, b_count_children, b_age_children," +
          "        b_bedclothes, b_promotion_partner, b_price, b_cleaning_cost, t_name, t_firstname, t_street, t_zipcode, t_location," +
          "        t_email, t_telephone, t_fax, t_title, t_salutation, t_id," +
          "        (SELECT MAX(bd_date) FROM booking_days WHERE bd_id = b_id) AS departure," +
          "        (SELECT MIN(bd_date) FROM booking_days WHERE bd_id = b_id) as arrival" +
          " FROM booking, tenant" +
          " WHERE b_id = ? " +
          " AND b_tenant_id = t_id";
        executor.SetCommandText (sql, command);      

        executor.AddParams (command, bookingId);
        command.Prepare();
        IDataReader reader = executor.RegisterReader (command);

        BookingItem item = null;

        if (reader.Read()) {
          int index = 0;
          item = new BookingItem ();
          
          item.ID = reader.GetInt32 (index++);
          item.BookingDate = reader.GetDateTime (index++);
          item.Status = (BookingStatus)reader.GetInt32 (index++);
          item.Notes = executor.GetString (reader, index++);
          item.CountAdults = reader.GetInt32 (index++);
          item.CountChildren = reader.GetInt32 (index++);
          item.AgeChildren = executor.GetString(reader, index++);
          item.BedClothes = executor.GetBoolean (reader, index++);
          item.PromotionPartner = executor.GetInt32 (reader, index++);
          
          // Preis
          PriceBean price = new PriceBean ();
          price.Rent = reader.GetInt32 (index++);
          price.CleaningCost = reader.GetInt32 (index++);
          item.Price = price;
                   
          // Mieter 
          TenantBean tenant = new TenantBean ();
          tenant.Name = executor.GetString (reader, index++);
          tenant.Firstname = executor.GetString (reader, index++);
          tenant.Street = executor.GetString (reader, index++);
          tenant.Zipcode = executor.GetString (reader, index++);
          tenant.Location = executor.GetString (reader, index++);
          tenant.Email = executor.GetString (reader, index++);
          tenant.Telephone = executor.GetString (reader, index++);
          tenant.Fax = executor.GetString (reader, index++);
          tenant.Title = executor.GetString (reader, index++);
          int salutation = executor.GetInt32 (reader, index++);
          log.Debug ("salutation == " + salutation);
          tenant.Salutation = (Salutation)salutation;
          tenant.ID = executor.GetInt32 (reader, index++);
          item.Tenant = tenant;
          
          // Zeitraum
          item.Departure = reader.GetDateTime (index++);
          item.Arrival = reader.GetDateTime (index++);
        }
        
        if (item == null) {
          log.Error ("bookings could not be found by id: " + bookingId);
        }
        reader.Close ();

        return item;
      });
    }

    /// <summary>
    /// Ändert eine Buchung in der Datenbank oder fügt eine neue Buchung hinzu.
    /// </summary>
    public MessageResult PerformUpdateBooking (BookingItem item)
    {
      // Ändern einer Buchung und das Erstellen einer neuen Buchung kann nur von 
      // einem Thread ausgeführt werden
      lock (instance) {
        // Prüfen des Buchungszeitraums
        ValidationResult valid = ValidatePeriodForUpdate (item);
        if (valid.Valid) {
          UpdateBooking (item);
          return new MessageResult (true);
        }
        else {
          return new MessageResult (false, valid.Message);
        }
      }
    }
    
    /// <summary>
    /// Ändert eine Reservierungen in der Datenbank.
    /// </summary>
    private void UpdateBooking (BookingItem item)
    {
      log.Debug ("BookingService.UpdateBooking");

      SQLExecutorFactory.Create ().Execute (true, delegate (SQLExecutor executor)
      {
        // ------ 1. Buchung updaten
        IDbCommand command = executor.CreateCommand ();

        String sql = 
          " UPDATE booking SET b_status = ?, b_notes = ?, b_count_adults = ?, b_count_children = ?, b_bedclothes = ?," +
          "                    b_price = ?, b_cleaning_cost = ?, b_promotion_partner = ?, b_age_children = ?" +
          " WHERE b_id = ?";
        executor.SetCommandText (sql, command);

        executor.AddInt (command, (int)item.Status);
        executor.AddString (command, item.Notes, MAX_NOTES_LENGTH);
        executor.AddInt (command, item.CountAdults);
        executor.AddInt (command, item.CountChildren);
        executor.AddBoolean (command, item.BedClothes);
        executor.AddInt (command, item.Price.Rent);
        executor.AddInt (command, item.Price.CleaningCost);
        executor.AddInt (command, item.PromotionPartner);
        executor.AddString(command, item.AgeChildren, MAX_AGE_CHILDREN_LENGTH);
        executor.AddInt (command, item.ID);
        command.Prepare();
        int rowCount = command.ExecuteNonQuery ();
        
        log.Debug ("booking updated = " + (rowCount == 1));
      
        // ------ 2. Mieterdaten ändern
        log.Debug ("modify tenant id: " + item.Tenant.ID);

        command = executor.CreateCommand ();

        sql = 
          " UPDATE tenant SET t_name = ?, t_firstname = ?, t_street = ?, t_zipcode = ?, t_location = ?," + 
          "                   t_email = ?, t_telephone = ?, t_fax = ?, t_title = ?, t_salutation = ?" +
          " WHERE t_id = ?";

        TenantBean tenant = item.Tenant;
        executor.SetCommandText (sql, command);

        executor.AddParams (command, tenant.Name, tenant.Firstname, tenant.Street, tenant.Zipcode, tenant.Location,
                                     tenant.Email, tenant.Telephone, tenant.Fax, tenant.Title, (Int32)tenant.Salutation,
                                     tenant.ID);

        command.Prepare();
        rowCount = command.ExecuteNonQuery ();
        
        log.Debug ("tenant updated = " + (rowCount == 1));

        // ------ 3. Sofern der Zeitraum verändert wurde 
        // -> Löschen der gebuchten Tage und hinzufügen neuer Tage
        if (item.PeriodModified) {

          // Tage löschen
          int deletedDays = ProcessUpdate ("DELETE FROM booking_days WHERE bd_id = ?", item.ID);
          log.Debug ("deleted days = " + deletedDays);
          
          // Insert Command für mehrmaligen Aufruf vorbereiten
          IDbCommand insertCmd = executor.CreateCommand ();
          sql = 
            " INSERT INTO booking_days (bd_id, bd_date, bd_occupied, bd_arrival, bd_departure)" +
            " VALUES (?, ?, ?, ?, ?)";
          executor.SetCommandText (sql, insertCmd);
              
          executor.AddParams (insertCmd, item.ID, DateTime.Now, (int)DBBoolean.TRUE, (int)DBBoolean.FALSE, (int)DBBoolean.FALSE);
          command.Prepare();
          
          // Buchungsätze für jeden Buchungstag in die Datenbank einfügen
          DateTime bookingDate = item.Arrival;
          while (bookingDate <= item.Departure) {
    
            int arrivalFlag = (int)DBBooleanType.GetDBBoolean (bookingDate.CompareTo (item.Arrival) == 0);
            int departureFlag = (int)DBBooleanType.GetDBBoolean (bookingDate.CompareTo (item.Departure) == 0);
            
            // neuen Datensatz einfügen
            executor.SetParameterValue (insertCmd, 1, bookingDate);
            executor.SetParameterValue (insertCmd, 3, (int)arrivalFlag);
            executor.SetParameterValue (insertCmd, 4, (int)departureFlag);
              
            rowCount = insertCmd.ExecuteNonQuery ();
            log.Debug ("booking day inserted = " + (rowCount == 1));
            
            // Buchungsdatum einen Tag hochzählen
            bookingDate = bookingDate.AddDays (1);
          }
        }   
     
        return null;
      });
      
      // Cache mit den Reservierungstagen löschen
      CacheHandler.Instance.RemoveAllMonthOccupancies ();
    }

    /// <summary>
    /// Hinzufügen einer Reservierung durch den Vermieter -> Keine Versendung von E-Mails.
    /// 1. Reservierung prüfen. 
    /// 2. Reservierungsdaten in Datenbank speichern.
    /// </summary>
    public MessageResult PerformInsertBooking (BookingItem bookingItem)
    {
      log.Debug ("BookingService.PerformInsertBooking");
      
      MessageResult result = null;
      
      // Buchung kann nur von einem Thread ausgeführt werden
      lock (instance) {
        // Validierung des Zeitraums
        ValidationResult valid = ValidatePeriodForInsert (bookingItem);
                          
        if (! valid.Valid) {
          result = new MessageResult (false, valid.Message);
        }
        else {          
          try {
            // Reservierung in Datenbank speichern
            SQLExecutorFactory.Create ().Execute (true, delegate (SQLExecutor executor)
            {
              InsertBooking (bookingItem, executor);

              return null;
            });

            // item ist nun nicht mehr neu
            bookingItem.IsNew = false;

            // Cache mit den Reservierungstagen löschen
            CacheHandler.Instance.RemoveAllMonthOccupancies ();

            result = new MessageResult (true);
          }
          // die Reservierung konnte nicht erfolgreich durchgeführt werden
          catch (Exception e) {
            log.Debug (e, e);
            result = new MessageResult(false, AppResources.GetMessage("BookingService_error.insert.booking"));
          }
        }
      }
      
      return result;
    }

    /// <summary>
    /// Prüfung, ob der Zeitraum gültig und noch frei ist.
    /// </summary>
    private ValidationResult ValidatePeriodForUpdate (BookingItem item)
    {
      log.Debug ("BookingService.ValidatePeriodForUpdate");
    
      bool result = true;
      String msg = null;
      DateTime now = new DateTime (DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            
      // Abreise liegt vor Anreise ?
      if (item.Arrival >= item.Departure) {
        msg = AppResources.GetMessage("BookingService_msg.departure.before.arrival");
        result = false;
      }
      else if (item.IsNew) {
        // Zeitraum schon belegt ?
        if (!IsPeriodFree (item.Arrival, item.Departure, item.HouseID)) {
          msg = AppResources.GetMessage("BookingService_msg.period.occupied");
          result = false;
        }
      }
      else if (! item.IsNew) {
        // Zeitraum schon belegt - Zeites des BookingItems nicht berücksichtigen?
        if (!IsPeriodFree (item)) {
          msg = AppResources.GetMessage("BookingService_msg.period.occupied");
          result = false;
        }
      }
      return new ValidationResult (result, msg);
    }

    /// <summary>
    /// Prüfung, ob der Zeitraum gültig und noch frei ist.
    /// </summary>
    private ValidationResult ValidatePeriodForInsert (BookingItem item)
    {
      log.Debug ("BookingService.ValidatePeriodForInsert");

      return ValidatePeriodForUpdate (item);
    }

    /// <summary>
    /// Prüft, ob ein Haus zum übergebenen Zeitpunkt noch frei ist. Ein Haus ist frei, wenn
    ///
    /// 1. kein Tag im gewünschten Zeitraum auf einen Tag fällt, der gebucht ist,
    ///    und an dem keine Ab- oder Anreise stattfindet
    ///
    /// und
    ///
    /// 2. kein Tag im gewünschten Zeitraum auf einen Tag fällt, der gebucht ist,
    ///    und an dem eine Ab- und eine Anreise stattfindet
    /// 
    /// Die gebuchten Tage des übergebene BookingItems werden hierbei ignoriert.
    /// </summary>
    private bool IsPeriodFree (BookingItem item)
    {
      log.Debug ("BookingService.IsPeriodFree");
 
      String query = " SELECT count(*) FROM booking_view" +
                     " WHERE b_house_id = ?" +
                     " AND b_id <> ? " +
                     " AND (" +
                     "   bd_arrival = 0 AND bd_departure = 0 AND bd_date >= ? AND bd_date <= ?" +
                     "   OR bd_arrival = 1 AND bd_date = ?" +
                     "   OR bd_departure = 1 AND bd_date = ?" +
                     " )";
 
      return ! ExistsRecord (query, item.HouseID, item.ID, item.Arrival, item.Departure, item.Arrival, item.Departure);   
    }

    
    
    #endregion
  }
}
