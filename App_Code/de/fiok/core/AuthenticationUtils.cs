namespace de.fiok.core
{
  using System;
  using System.Text;
  using System.Security.Cryptography;
  using log4net;

	/// <summary>
	/// Klasse zur Generierung von Passwörtern und Hashwerten von Passwörtern.
	/// Jedem Passwort wird vor dem Hashen ein zufälliger Wert (Salt) anhängt, 
	/// und erst dann wird der Hash über das Passwort und den Salt berechnet. 
	/// Der Hashwert und der Salt werden in der Datenbank gespeichert.
	/// </summary>
	/// <remarks>
	/// created by - Christoph Wille, http://www.aspheute.com/artikel/20040105.htm
	/// </remarks>
	public class AuthenticationUtils
	{
    private static readonly ILog log = LogManager.GetLogger(typeof(AuthenticationUtils));

    /// <summary>
	  /// Klasse zur Generierung von Passwörtern und Hashwerten von Passwörtern.
   	/// Jedem Passwort wird vor dem Hashen ein zufälliger Wert (Salt) anhängt, 
  	/// und erst dann wird der Hash über das Passwort und den Salt berechnet. 
  	/// Der Hashwert und der Salt werden in der Datenbank gespeichert.
  	/// </summary>
		public static String CreateRandomPassword (int passwordLength)
		{
		  log.Debug ("AuthenticationUtils.CreateRandomPassword");
		
			String allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ23456789";
			byte[] randomBytes = new byte[passwordLength];
			
			RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
			rng.GetBytes(randomBytes);
			
			char[] chars = new char[passwordLength];
			int allowedCharCount = allowedChars.Length;

			for (int i = 0; i < passwordLength; i++) {
				chars[i] = allowedChars[(int)randomBytes[i] % allowedCharCount];
			}
			
			String randomPassword = new String (chars);
      log.Debug ("randomPassword: " + randomPassword);
      
			return randomPassword;
		}

    /// <summary>
	  /// Generierung des Zufallwertes -> Salt.
  	/// </summary>
		public static int CreateRandomSalt ()
		{
			byte[] saltBytes = new byte[4];
			RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider ();
			rng.GetBytes (saltBytes);

			return ((((int)saltBytes[0]) << 24) + (((int)saltBytes[1]) << 16) + 
				      (((int)saltBytes[2]) << 8) + ((int)saltBytes[3]));
		}

    /// <summary>
	  /// Erzeugung des Hashwertes aus einem Passwort und einem Salt.
  	/// </summary>
		public static String ComputeSaltedHash (String password, int salt)
		{
			// aus dem Passwort ein Byte-Array in ASCII erstellen
			ASCIIEncoding encoder = new ASCIIEncoding ();
			byte[] secretBytes = encoder.GetBytes (password);
			
			// aus dem Salt ein Byte-Array erstellen
			byte[] saltBytes = new byte[4];
			saltBytes[0] = (byte)(salt >> 24);
			saltBytes[1] = (byte)(salt >> 16);
			saltBytes[2] = (byte)(salt >> 8);
			saltBytes[3] = (byte)(salt);

			// beide Array in ein Array kopieren
			byte[] toHash = new byte[secretBytes.Length + saltBytes.Length];
			Array.Copy (secretBytes, 0, toHash, 0, secretBytes.Length);
			Array.Copy (saltBytes, 0, toHash, secretBytes.Length, saltBytes.Length);
      
      // SHA1-Hashwert berechnen
			SHA1 sha1 = SHA1.Create ();
			byte[] computedHash = sha1.ComputeHash (toHash);

      // Hash-Wert als Base64-String zurückliefern 
			String strHash = Convert.ToBase64String (computedHash);
		  log.Debug ("hash: " + strHash);
		  
		  return strHash;
		}
	}
}
