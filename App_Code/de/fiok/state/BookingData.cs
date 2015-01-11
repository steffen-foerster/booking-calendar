namespace de.fiok.state
{
  using System;
  using de.fiok.service;
 
  ///<summary>
  /// Die Klasse BookingData enthält den ausgewählten Zeitraum und den berechneten Preis.
  ///</summary>
  ///<remarks>
	/// created by - Steffen Förster
	///</remarks>
  public class BookingData
  {
    private DateTime arrivalDate;
    private DateTime departureDate;
    private PriceBean price;

    public BookingData ()
    {
      this.arrivalDate = DateTime.MinValue;
      this.departureDate = DateTime.MinValue;
    }
    
    public BookingData (DateTime arrivalDate, DateTime departureDate, PriceBean price)
    {
      this.arrivalDate = arrivalDate;
      this.departureDate = departureDate;
      this.price = price;
    }

    public DateTime ArrivalDate
    {
      get {return arrivalDate;}
      set {arrivalDate = value;}
    }

    public DateTime DepartureDate
    {
      get {return departureDate;}
      set {departureDate = value;}
    }

    public PriceBean Price
    {
      get {return price;}
      set {price = value;}
    }
    
    public int HouseID
    {
      get {return price.House.ID;}
    }
  }
}
