namespace de.fiok.service
{
  using System;
  using de.fiok.type;

  /// <summary>
  ///  Diese Klasse repräsentiert eine Buchung mit dazugehörigen Daten.
  /// </summary>
  /// <remarks>
	///  created by - Steffen Förster
	/// </remarks>
  public class BookingItem
  {
    private Int32 id;
    private Int32 houseId;
    private TenantBean tenant;
    private PriceBean price;
    private DateTime arrival;
    private DateTime departure;
    private DateTime bookingDate;
    private String notes;
    private BookingStatus status;
    private int countChildren;
    private int countAdults;
    private int promotionPartner;
    private String ageChildren;
    private bool bedClothes;
    private bool periodModified;
    private bool isNew;
    
    public BookingItem ()
    {
    }
    
    public Int32 ID
    {
      get { return id; }
      set { id = value; }
    }

    public Int32 HouseID
    {
      get { return houseId; }
      set { houseId = value; }
    }
    
    public TenantBean Tenant
    {
      get { return tenant; }
      set { tenant = value; }
    }
    
    public PriceBean Price
    {
      get { return price; }
      set { price = value; }
    }
    
    public DateTime Arrival
    {
      get { return arrival; }
      set { arrival = value; }
    }
    
    public DateTime Departure
    {
      get { return departure; }
      set { departure = value; }
    }
    
    public DateTime BookingDate
    {
      get { return bookingDate; }
      set { bookingDate = value; }
    }
    
    public String Notes
    {
      get { return notes; }
      set { notes = value; }
    }
    
    public BookingStatus Status
    {
      get { return status; }
      set { status = value; }
    }
    
    public int CountAdults
    {
      get { return countAdults; }
      set { countAdults = value; }
    }
    
    public int CountChildren
    {
      get { return countChildren; }
      set { countChildren = value; }
    }

    public String AgeChildren
    {
      get { return ageChildren; }
      set { ageChildren = value; }
    }

    public int PromotionPartner
    {
      get { return promotionPartner; }
      set { promotionPartner = value; }
    }

    public bool BedClothes
    {
      get { return bedClothes; }
      set { bedClothes = value; }
    }

    public bool PeriodModified
    {
      get { return periodModified; }
      set { periodModified = value; }
    }

    public bool IsNew
    {
      get { return isNew; }
      set { isNew = value; }
    }
  }
}
