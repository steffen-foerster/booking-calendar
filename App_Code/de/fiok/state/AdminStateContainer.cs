namespace de.fiok.state
{
  using System;
  using de.fiok.service;
 
  ///<summary>
  /// Die Klasse AdminStateContainer dient als Container für alle Daten, die während
  /// der Navigation durch die Administration eines Ferienhauses gespeichert werden 
  /// müssen.
  ///</summary>
  ///<remarks>
	/// created by - Steffen Förster
	///</remarks>
  public class AdminStateContainer
  {
    private AdminHouseData adminHouseData;
    private AdminBookingData adminBookingData;
    
    public AdminStateContainer ()
    {
      adminHouseData = new AdminHouseData ();
      adminBookingData = new AdminBookingData ();
    }
    
    public AdminHouseData AdminHouseData
    {
      get {return adminHouseData;}
      set {adminHouseData = value;}
    }
    
    public AdminBookingData AdminBookingData
    {
      get {return adminBookingData;}
      set {adminBookingData = value;}
    }
  }
}
