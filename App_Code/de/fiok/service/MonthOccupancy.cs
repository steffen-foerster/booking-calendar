namespace de.fiok.service
{
  using System;
  using System.Collections;
  using log4net;

  /// <summary>
  /// Klasse enthält den Belegungsstatus für einen Monat und für ein Haus.
  /// </summary>
  /// <remarks>
	/// created by - Steffen Förster
	/// </remarks>
  public class MonthOccupancy
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(MonthOccupancy));

    private int month;
    private int year;
    private int houseId;
    private Hashtable days;

    public MonthOccupancy (int year, int month, int houseId)
    {
      this.year = year;
      this.month = month;
      this.houseId = houseId;

      days = new Hashtable ();
    }

    /// <summary>
    /// Neuen Buchungstag hinzufügen.
    /// </summary>
    public void AddBookingDay (DateTime date, bool reserved, bool arrival, bool departure)
    {
      date = new DateTime (date.Year, date.Month, date.Day);
      BookingDay bookingDay = new BookingDay (reserved, arrival, departure);

      days.Add (date, bookingDay);
    }

    /// <summary>
    /// Liefert Buchungstag zu einem Datum.
    /// </summary>
    public BookingDay GetBookingDay (DateTime date)
    {
      return (BookingDay)days[date];
    }

    public int Year
    {
      get {return year;}
    }

    public int Month
    {
      get {return month;}
    }

    public int HouseId
    {
      get {return houseId;}
    }
  }

  public class BookingDay
  {
    private bool reserved;
    private bool arrival;
    private bool departure;

    public BookingDay (bool reserved, bool arrival, bool departure)
    {
      this.reserved = reserved;
      this.arrival = arrival;
      this.departure = departure;
    }

    public bool Reserved
    {
      get {
        return reserved;
      }
    }

    public bool Arrival
    {
      get {
        return arrival;
      }
    }

    public bool Departure
    {
      get {
        return departure;
      }
    }
  }
}
