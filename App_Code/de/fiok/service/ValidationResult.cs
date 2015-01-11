namespace de.fiok.service 
{
	using System;	
	
	/// <summary>
	/// Ergebnis einer Validierung.
	/// </summary>
	/// <remarks>
	/// created by - Steffen Förster
	/// </remarks>
	public class ValidationResult
	{
	  private bool valid;
	  private String message;
	
		/// <summary>
		/// Konstruktor.
		/// </summary>
		public ValidationResult (bool valid, String message) 
		{
		  this.valid = valid;
		  this.message = message;
		}
		
		public bool Valid
		{
		  get {return valid;}
		}
		
		public String Message
		{
		  get {return message;}
		}
	}
}
