namespace de.fiok.service
{
  using System;

  /// <summary>
  ///  Das HouseBean enthält spezifische Daten zu einem Ferienhaus.
  /// </summary>
  /// <remarks>
	///  created by - Steffen Förster
	/// </remarks>
  [Serializable]
  public class HouseBean
  {
    private int id;
    private int cleaningCost;
    private int minDaysSeason;
    private int minDaysTotal;
    private bool bedClothesHirable;
    private String location;
    private String name;
    private LandlordBean landlord;

    public static HouseBean EMPTY_HOUSE = new HouseBean (-1, -1, -1, -1, false, null, null, null);

    public HouseBean ()
    {
    }

    public HouseBean (int id, String location, String name)
    {
      this.id = id;
      this.location = location;
      this.name = name;
    }

    public HouseBean (int id, int cleaningCost, int minDaysSeason, int minDaysTotal,
                      bool bedClothesHirable, String location, String name, LandlordBean landlord)
    {
      this.id = id;
      this.cleaningCost = cleaningCost;
      this.minDaysSeason = minDaysSeason;
      this.minDaysTotal = minDaysTotal;
      this.bedClothesHirable = bedClothesHirable;
      this.location = location;
      this.landlord = landlord;
      this.name = name;
    }

    public int ID
    {
      get { return id; }
      set { id = value; }
    }

    public int CleaningCost
    {
      get {return cleaningCost;}
      set {cleaningCost = value;}
    }

    public int MinDaysSeason
    {
      get {return minDaysSeason;}
      set {minDaysSeason = value;}
    }

    public int MinDaysTotal
    {
      get {return minDaysTotal;}
      set {minDaysTotal = value;}
    }
    
    public String Location
    {
      get {return location;}
      set {location = value;}
    }

    public String Name
    {
      get {return name;}
      set {name = value;}
    }

    public bool BedClothesHirable
    {
      get {return bedClothesHirable;}
      set {bedClothesHirable = value;}
    }

    public bool ExtraHirable
    {
      get {return bedClothesHirable;}
    }
    
    public LandlordBean Landlord
    {
      get {return landlord;}
    }

    public int LandlordId
    {
      get { return landlord.ID; }
      set {
        this.landlord = new LandlordBean ();
        this.landlord.ID = value;
      }
    }
  }
}
