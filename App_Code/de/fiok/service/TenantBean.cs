namespace de.fiok.service
{
  using System;
  using de.fiok.type;
 
  /// <summary>
  ///   Diese Klasse repräsentiert einen Mieter.
  /// </summary>
  /// <remarks>
	/// 	created by - Steffen Förster
	/// </remarks>
  public class TenantBean
  {
    private Salutation salutation;
    private String name;
    private String firstname;
    private String title;
    private String street;
    private String zipcode;
    private String location;
    private String email;
    private String telephone;
    private String fax;
    private Int32 id;

    public TenantBean ()
    {
    }

    public Int32 ID
    {
      get {return id;}
      set {id = value;}
    }
    
    public Salutation Salutation
    {
      get {return salutation;}
      set {salutation = value;}
    }
    
    public String Name
    {
      get {return name;}
      set {name = value;}
    }
    
    public String Firstname
    {
      get {return firstname;}
      set {firstname = value;}
    }
    
    public String Title
    {
      get {return title;}
      set {title = value;}
    }
    
    public String Street
    {
      get {return street;}
      set {street = value;}
    }
    
    public String Zipcode
    {
      get {return zipcode;}
      set {zipcode = value;}
    }
    
    public String Location
    {
      get {return location;}
      set {location = value;}
    }
    
    public String Email
    {
      get {return email;}
      set {email = value;}
    }
    
    public String Telephone
    {
      get {return telephone;}
      set {telephone = value;}
    }
    
    public String Fax
    {
      get {return fax;}
      set {fax = value;}
    }
  }
}


