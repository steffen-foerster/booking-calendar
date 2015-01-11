namespace de.fiok.state
{
  using System;
  using System.Collections;
  using de.fiok.service;
 
  ///<summary>
  /// Die Klasse AdminHouseData enthält die Daten für die Administrationsseiten, zur Pflege von
  /// Ferienhausdaten.
  ///</summary>
  ///<remarks>
	/// created by - Steffen Förster
	///</remarks>
  public class AdminHouseData
  {
    private HouseBean currentHouse;
    private IList housePriceList; 
    private IList removedPriceList;
  
    public AdminHouseData ()
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
    
    public IList HousePriceList
    {
      get {return housePriceList;}
      set {housePriceList = value;}
    }
    
    public IList RemovedPriceList
    {
      get {return removedPriceList;}
      set {removedPriceList = value;}
    }
  }
}
