namespace de.fiok.state
{
  using System;
  using System.Collections;
  using de.fiok.service;
 
  ///<summary>
  /// Die Klasse AdminBookingData enthält die Daten für die Administration der Buchungen.
  ///</summary>
  ///<remarks>
	/// created by - Steffen Förster
	///</remarks>
  public class AdminBookingData
  {
    private HouseBean currentHouse;
    private IList bookingList; 
  
    public AdminBookingData ()
    {
    }
    
    public HouseBean House
    {
      get {return currentHouse;}
      set {currentHouse = value;}
    }
    
    public int LandlordID
    {
      get {
        return currentHouse == null ? -1 : currentHouse.Landlord.ID;
      }
    }
    
    public IList BookingList
    {
      get {return bookingList;}
      set {bookingList = value;}
    }
  }
}
