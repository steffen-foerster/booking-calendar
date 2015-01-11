namespace de.fiok.service 
{
	using System;
	using System.Collections;
	
  public class HousePriceInterval
  {
    private int id;
    private DateTime start;
    private DateTime end;
    // Miete pro Tag
    private int price;
    private bool peakSeason; 
    private bool valid;
    private BookingDays arrivalDays;
    private BookingDays departureDays;
    private int minBookingDays;

    public HousePriceInterval (DateTime start, DateTime end, int price, bool peakSeason, 
                               BookingDays arrivalDays, BookingDays departureDays, int minBookingDays)
    {
      this.start = start;
      this.end = end;
      this.price = price;
      this.peakSeason = peakSeason;
      this.id = -1;
      this.valid = true;
      this.arrivalDays = arrivalDays;
      this.departureDays = departureDays;
      this.minBookingDays = minBookingDays;
    }
    
    public HousePriceInterval (int id, DateTime start, DateTime end, int price, bool peakSeason,
                               BookingDays arrivalDays, BookingDays departureDays, int minBookingDays) 
           : this (start, end, price, peakSeason, arrivalDays, departureDays, minBookingDays)
    {
      this.id = id;
    }

    public DateTime Start
    {
      get {return start;}
      set {start = value;}
    }

    public DateTime End
    {
      get {return end;}
      set {end = value;}
    }

    public int Price
    {
      get {return price;}
      set {price = value;}
    }

    public bool PeakSeason
    {
      get {return peakSeason;}
      set {peakSeason = value;}
    }

    public BookingDays ArrivalDays
    {
      get {return arrivalDays;}
      set {arrivalDays = value;}
    }

    public BookingDays DepartureDays
    {
      get {return departureDays;}
      set {departureDays = value;}
    }

    public int MinBookingDays
    {
      get {return minBookingDays;}
      set {minBookingDays = value;}
    }
    
    public int ID
    {
      get {return id;}
    }
    
    public bool Valid
    {
      get {return valid;}
      set {valid = value;}
    }
    
    public bool IsNew ()
    {
      return id == -1;
    }

    public bool Contains (DateTime date)
    {
      // Zeit auf 0 setzen
      date = new DateTime (date.Year, date.Month, date.Day);

      return start <= date && end >= date;
    }
    
    public override String ToString ()
    {
      return "start = " + start.ToShortDateString () + ", end = " + end.ToShortDateString () +
             ", price = " + price;
    }
  }
}
