namespace de.fiok.service
{
  using System;
  using de.fiok.type;

  /// <summary>
  /// Das LandlordBean enthält spezifische Daten zu einem Vermieter.
  /// </summary>
  /// <remarks>
	///  created by - Steffen Förster
	/// </remarks>
  [Serializable]
  public class LandlordBean
  {
    private int id;
    private String name;
    private String firstname;
    private String email;
    private String domain;
    private LandlordRole role;
    private String newPassword;

    public static LandlordBean EMPTY_LANDLORD = new LandlordBean (-1, null, null, null);

    public LandlordBean ()
    {
    }

    public LandlordBean (int id, String name, String firstname, String email)
    {
      this.id = id;
      this.name = name;
      this.firstname = firstname;
      this.email = email;
    }

    public int ID
    {
      get { return id; }
      set { this.id = value; }
    }

    public String Name
    {
      get { return name; }
      set { this.name = value; }
    }
    
    public String Firstname
    {
      get { return firstname; }
      set { this.firstname = value; }
    }
    
    public String Email
    {
      get { return email; }
      set { this.email = value; }
    }

    public String Domain
    {
      get { return domain; }
      set { this.domain = value; }
    }

    public LandlordRole Role
    {
      get { return role; }
      set { this.role = value; }
    }

    public String NewPassword
    {
      get { return newPassword; }
      set { this.newPassword = value; }
    }
    
    public class Password 
    {
      private String phrase;
      private int salt;
      
      public Password (String phrase, int salt) 
      {
        this.phrase = phrase;
        this.salt = salt;
      }
      
      public String Phrase
      {
        get {return phrase;}
      }
      
      public int Salt
      {
        get {return salt;}
      }
    }
  } 
}
