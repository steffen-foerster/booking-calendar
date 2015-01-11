namespace de.fiok.service
{
  using System;
  using log4net;

  /// <summary>
  ///  Das PriceBean enthält den berechneten Mietpreis und Informationen zur Preisberechnung.
  /// </summary>
  /// <remarks>
  ///  created by - Steffen Förster
  /// </remarks>
  public class PriceBean
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(PriceBean));

    private int totalDays;
    private int totalCost;
    private int rent;
    private int cleaningCost;
    private bool cleaningDays;
    private bool cleaningSeason;
    private HouseBean house;

    public static PriceBean EMPTY_PRICE = new PriceBean (HouseBean.EMPTY_HOUSE);

    public PriceBean()
    {
    }
    
    public PriceBean (HouseBean house)
    {
      this.house = house; 
    }

    public int TotalDays
    {
      get {return totalDays;}
      set {totalDays = value;}
    }

    public int TotalCost
    {
      get {return totalCost;}
      set {totalCost = value;}
    }

    public int Rent
    {
      get {return rent;}
      set {rent = value;}
    }

    public int CleaningCost
    {
      get {return cleaningCost;}
      set {cleaningCost = value;}
    }

    public bool CleaningDays
    {
      get {return cleaningDays;}
      set {cleaningDays = value;}
    }

    public bool CleaningSeason
    {
      get {return cleaningSeason;}
      set {cleaningSeason = value;}
    }

    public HouseBean House
    {
      get {return house;}
      set {house = value;}
    }
  }
}
