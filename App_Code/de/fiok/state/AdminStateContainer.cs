namespace de.fiok.state
{
  using System;
  using de.fiok.service;
 
  ///<summary>
  /// Die Klasse AdminStateContainer dient als Container f�r alle Daten, die w�hrend
  /// der Navigation durch die Administration eines Ferienhauses gespeichert werden 
  /// m�ssen.
  ///</summary>
  ///<remarks>
	/// created by - Steffen F�rster
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
