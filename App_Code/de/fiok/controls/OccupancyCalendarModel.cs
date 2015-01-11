namespace de.fiok.controls
{
  using System;
  using System.Collections;
  using de.fiok.service;
  using log4net;

  /// <summary>
  /// Klasse enthält die notwendigen Daten zum Rendern eines Monats, der die Belegung 
  /// eines Hauses anzeigt.
  /// </summary>
  /// <remarks>
	/// created by - Steffen Förster
	/// </remarks>
  public class OccupancyCalendarModel
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(OccupancyCalendarModel));

    private int month;
    private int year;
    private MonthOccupancy occupancy;
    private ArrayList daysOfWeek;
    private ArrayList weeks;

    public OccupancyCalendarModel (int year, int month, MonthOccupancy occupancy)
    {
      this.year = year;
      this.month = month;
      this.occupancy = occupancy;

      daysOfWeek = createDayList ();
      weeks = createWeeks ();
    }

    public int Year
    {
      get {return year;}
    }

    public int Month
    {
      get {return month;}
    }

    public MonthOccupancy Occupancy
    {
      get {return occupancy;}
    }

    public String AsString
    {
      get {return new DateTime (year, month, 1).ToString ("MMMM yyyy");}
    }

    public ArrayList DaysOfWeek
    {
      get {return daysOfWeek;}
    }

    public ArrayList Weeks
    {
      get {return weeks;}
    }

    /// <summary>
    /// Erstellt eine Liste mit den Wochentagen, angefangen mit dem Wochentag des 01. des Monats.
    /// </summary>
    private ArrayList createDayList ()
    {
      //Hashtable days = new Hashtable ();
      //days.Add (DayOfWeek.Monday, "Mo");
      //days.Add (DayOfWeek.Tuesday, "Di");
      //days.Add (DayOfWeek.Wednesday, "Mi");
      //days.Add (DayOfWeek.Thursday, "Do");
      //days.Add (DayOfWeek.Friday, "Fr");
      //days.Add (DayOfWeek.Saturday, "Sa");
      //days.Add (DayOfWeek.Sunday, "So");

      ArrayList result = new ArrayList ();

      //DateTime day = new DateTime (year, month, 1);

      //for (int i = 0; i < 7; i++) {
      //  result.Add (new DayName ((String)days[day.DayOfWeek], day.DayOfWeek));
      //  day = day.AddDays (1);
      //}

      result.Add (new DayName ("Mo", DayOfWeek.Monday));
      result.Add (new DayName ("Di", DayOfWeek.Tuesday));
      result.Add (new DayName ("Mi", DayOfWeek.Wednesday));
      result.Add (new DayName ("Do", DayOfWeek.Thursday));
      result.Add (new DayName ("Fr", DayOfWeek.Friday));
      result.Add (new DayName ("Sa", DayOfWeek.Saturday));
      result.Add (new DayName ("So", DayOfWeek.Sunday));

      return result;
    }

    /// <summary>
    /// Erstellt eine Liste mit Wochen, die die Tage des Monats enthalten.
    /// </summary>
    private ArrayList createWeeks ()
    {
      DateTime startDate = new DateTime (year, month, 1);
      DateTime endDate = startDate.AddMonths (1);

      ArrayList weeks = new ArrayList ();

      Week week = new Week (); 
      weeks.Add (week);

      // leere Tage hinzufügen als Offset
      if (startDate.DayOfWeek == DayOfWeek.Sunday) {
        for (int i = 0; i < 6; i++) {
          week.AddDay (-1);
        }
      }
      else {
        for (int i = (int)DayOfWeek.Monday; i < (int)startDate.DayOfWeek; i++) {
          week.AddDay (-1);
        }
      }

      while (startDate < endDate) {
        week.AddDay (startDate.Day);

        if (startDate.DayOfWeek == DayOfWeek.Sunday) {
          week = new Week ();
          weeks.Add (week);
        }

        startDate = startDate.AddDays (1);
      }

      // angefangene Woche mit leeren Tagen hinzufügen
      for (int i = week.Days.Count; i < 7; i++) {
        week.AddDay (-1);
      }

      if (weeks.Count == 4) {
        addEmptyWeek (weeks);
      }
      if (weeks.Count == 5) {
        addEmptyWeek (weeks);
      }

      return weeks;
    }

    private void addEmptyWeek (ArrayList weeks)
    {
      Week week = new Week ();
      weeks.Add (week);

      // Woche mit leeren Tagen hinzufügen
      for (int i = 0; i < 7; i++) {
        week.AddDay (-1);
      }
    }
  }

  public class DayName
  {
    private String name;
    private DayOfWeek dayWeek;

    public DayName (String name, DayOfWeek dayWeek)
    {
      this.name = name;
      this.dayWeek = dayWeek;
    }

    public String Name
    {
      get {
        return name;
      }
    }

    public DayOfWeek DayWeek
    {
      get {
        return dayWeek;
      }
    }
  }

  public class Week
  {
    private ArrayList days;

    public Week ()
    {
      days = new ArrayList ();
    }

    public void AddDay (int day)
    {
      days.Add (day);
    }

    public ArrayList Days
    {
      get {
        return days;
      }
    }
  }
}
