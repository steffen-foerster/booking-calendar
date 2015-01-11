namespace de.fiok.state
{
  using System;
  using de.fiok.service;
 
  /// <summary>
  ///   Die Klasse TenantEntryData enthält die persönlichen Daten des Mieters.
  /// </summary>
  /// <remarks>
	/// 	created by - Steffen Förster
	/// </remarks>
  public class TenantEntryData
  {
    private TenantBean tenant;
    private int adultCount;
    private int childrenCount;
    private String notes;
    private bool bedClothes;
    private int promotionPartner;
    private String ageChildren;

    public TenantEntryData ()
    {
      tenant = new TenantBean ();
    }
    
    public TenantBean Tenant
    {
      get {return tenant;}
      set {tenant = value;}
    }
    
    public int AdultCount
    {
      get {return adultCount;}
      set {adultCount = value;}
    }
    
    public int ChildrenCount
    {
      get {return childrenCount;}
      set {childrenCount = value;}
    }
    
    public String Notes
    {
      get {return notes;}
      set {notes = value;}
    }

    public bool BedClothes
    {
      get {return bedClothes;}
      set {bedClothes = value;}
    }

    public int PromotionPartner
    {
        get { return promotionPartner; }
        set { promotionPartner = value; }
    }

    public String AgeChildren
    {
      get { return ageChildren; }
      set { ageChildren = value; }
    }
	
  }
}
