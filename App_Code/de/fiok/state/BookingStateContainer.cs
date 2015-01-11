namespace de.fiok.state
{
  using System;
  using de.fiok.service;
 
  ///<summary>
  /// Die Klasse BookingStateContainer dient als Container für alle Daten, die während
  /// der Navigation durch die Dialoge der Reservierung gespeichert werden müssen.
  ///</summary>
  ///<remarks>
	/// created by - Steffen Förster
	///</remarks>
  public class BookingStateContainer
  {
    private HouseBean house;
    private BookingData bookingData;
    private TenantEntryData tenantData;

    public BookingStateContainer ()
    {
    }

    public HouseBean HouseBean
    {
      get { return house; }
      set { house = value; }
    }
    
    public BookingData BookingData
    {
      get {return bookingData;}
      set {bookingData = value;}
    }

    public TenantEntryData TenantData
    {
      get {return tenantData;}
      set {tenantData = value;}
    }
  }
}
