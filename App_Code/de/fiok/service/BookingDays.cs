using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using log4net;
using de.fiok.core;

namespace de.fiok.service
{
  /// <summary>
  /// Repräsentiert die Wochentage, an denen eine Anreise oder Abreise möglich ist.
  /// </summary>
  [Serializable]
  public class BookingDays
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(BookingDays));

    public static readonly BookingDays ALL_DAYS = BookingDays.Create (127);
   
    private IList<DayOfWeek> days = new List<DayOfWeek> ();
    
    public BookingDays ()
    {
    }

    public BookingDays (IList<DayOfWeek> days)
    {
      this.days = days; 
    }

    /// <summary>
    /// Erstellt aus einer Zahl, die die Wochentage repräsentiert, eine Liste
    /// mit den Wochentagen.
    /// </summary>
    /// <param name="dayList">Zulässige Wochentage als Nummer.</param>
    public static BookingDays Create (int dayList)
    {
      log.Debug ("BookingDays.Create");

      log.Debug ("dayList = " + dayList);

      IList<DayOfWeek> days = new List<DayOfWeek> ();
      if ((dayList & GetDatabaseIntValue (DayOfWeek.Monday)) > 0) {
        days.Add (DayOfWeek.Monday);
      }
      if ((dayList & GetDatabaseIntValue (DayOfWeek.Tuesday)) > 0) {
        days.Add (DayOfWeek.Tuesday);
      }
      if ((dayList & GetDatabaseIntValue (DayOfWeek.Wednesday)) > 0) {
        days.Add (DayOfWeek.Wednesday);
      }
      if ((dayList & GetDatabaseIntValue (DayOfWeek.Thursday)) > 0) {
        days.Add (DayOfWeek.Thursday);
      }
      if ((dayList & GetDatabaseIntValue (DayOfWeek.Friday)) > 0) {
        days.Add (DayOfWeek.Friday);
      }
      if ((dayList & GetDatabaseIntValue (DayOfWeek.Saturday)) > 0) {
        days.Add (DayOfWeek.Saturday);
      }
      if ((dayList & GetDatabaseIntValue (DayOfWeek.Sunday)) > 0) {
        days.Add (DayOfWeek.Sunday);
      }

      BookingDays instance = new BookingDays (days);
      log.Debug ("----> BookingDays.Create number: " + dayList + ", days: " + instance.BuildDayListString);

      return instance;
    }

    /// <summary>
    /// Fügt den übergebenen Wochentag der Liste hinzu.
    /// </summary>
    public void Add (DayOfWeek dayOfWeek)
    {
      days.Add (dayOfWeek);
    }

    /// <summary>
    /// Liefert true, wenn der übergebene Wochentag in der Liste der Wochentage des BookingDays-Objektes
    /// enthalten ist.
    /// </summary>
    public bool Contains (DayOfWeek dayOfWeek)
    {
      return days.Contains (dayOfWeek);
    }

    /// <summary>
    /// Erstellt eine Zahl, die die zulässigen Wochentage repräsentieren.
    /// Bit 1 -> Sonntag, Bit 2 -> Montag, Bit 3 -> Dienstag usw.
    /// </summary>
    public int BuildDayListNumber ()
    {
      log.Debug ("BookingDays.BuildDayListNumber");

      int number = 0;

      foreach (DayOfWeek day in days) {
        number = number | (1 << (int)day);
      }

      log.Debug ("BuildDayListNumber number: " + number);
      return number;
    }

    /// <summary>
    /// Erstellt einen String mit den Kurz-Namen aller enthaltenen Wochentage.
    /// </summary>
    public String BuildDayListString
    {
      get
      {
        if (BuildDayListNumber () == 127) {
          return AppResources.GetMessage("BookingDays.all.days");
        }

        IDictionary<DayOfWeek, String> dayNames = CreateDayNameMap ();

        StringBuilder str = new StringBuilder ();
        foreach (DayOfWeek day in days) {
          if (str.Length > 0) {
            str.Append (", ");
          }
          str.Append (dayNames[day]);
        }
        
        return str.ToString ();
      }
    }

    /// <summary>
    /// Liefert eine Liste, die zu jedem Wochentag ein BookingDayItem enthält. Dieses 
    /// kann z.B. als Objekt für eine Checkbox-Liste dienen.
    /// </summary>
    public IList<BookingDayItem> GetItems ()
    {
      log.Debug ("BookingDays.GetItems");

      IList<BookingDayItem> items = new List<BookingDayItem> ();
      IDictionary<DayOfWeek, String> dayNames = CreateDayNameMap ();
      foreach (KeyValuePair<DayOfWeek, String> dayName in dayNames) {
        items.Add (new BookingDayItem (days.Contains (dayName.Key), dayName.Value, dayName.Key));
      }

      return items;
    }
    
    /// <summary>
    /// Liefert true, wenn keine Wochentage enthalten sind.
    /// </summary>
    public bool IsEmpty
    {
      get { return days.Count == 0; }
    }

    private static IDictionary<DayOfWeek, String> CreateDayNameMap ()
    {
      log.Debug ("BookingDays.CreateDayNameMap");

      IDictionary<DayOfWeek, String> dayNames = new Dictionary<DayOfWeek, String> ();
      dayNames.Add(DayOfWeek.Monday, AppResources.GetMessage("DateTime_monday.short"));
      dayNames.Add(DayOfWeek.Tuesday, AppResources.GetMessage("DateTime_tuesday.short"));
      dayNames.Add(DayOfWeek.Wednesday, AppResources.GetMessage("DateTime_wednesday.short"));
      dayNames.Add(DayOfWeek.Thursday, AppResources.GetMessage("DateTime_thursday.short"));
      dayNames.Add(DayOfWeek.Friday, AppResources.GetMessage("DateTime_friday.short"));
      dayNames.Add(DayOfWeek.Saturday, AppResources.GetMessage("DateTime_saturday.short"));
      dayNames.Add(DayOfWeek.Sunday, AppResources.GetMessage("DateTime_sunday.short"));

      return dayNames;
    }

    private static int GetDatabaseIntValue (DayOfWeek dayOfWeek)
    {
      log.Debug ("BookingDays.GetDatabaseIntValue");

      int value = 1 << ((int)dayOfWeek);

      log.Debug ("dayOfWeek: " + dayOfWeek + ", databaseValue: " + value);

      return value;
    }
  }

  public class BookingDayItem
  {
    private bool enabled;
    private String name;
    private DayOfWeek dayOfWeek;

    public BookingDayItem (bool enabled, String name, DayOfWeek dayOfWeek)
    {
      this.enabled = enabled;
      this.name = name;
      this.dayOfWeek = dayOfWeek;
    }

    public String Value
    {
      get { return enabled + "_" + ((int)dayOfWeek); }
    }

    public String Name
    {
      get { return name; }
    }
  }
}
