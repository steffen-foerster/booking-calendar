namespace de.fiok.service 
{
	using System;	
	
	/// <summary>
	/// Ergebnis eines Methoden-Aufrufs, das einen boolschen-Wert und eine Meldung enthält.
	/// </summary>
	/// <remarks>
	/// created by - Steffen Förster
	/// </remarks>
	public class MessageResult
	{
	  private bool result;
	  private String message;
	  private Object value;
	
	  /// <summary>
		/// Konstruktor.
		/// </summary>
		public MessageResult (bool result) 
		{
		  this.result = result;
		}
	 
		/// <summary>
		/// Konstruktor.
		/// </summary>
		public MessageResult (bool result, String message) : this (result)
		{
		  this.message = message;
		}
		
		/// <summary>
		/// Konstruktor.
		/// </summary>
		public MessageResult (bool result, String message, Object value) : this (result, message)
		{
		  this.value = value;
		}
		
		public bool Result
		{
		  get {return result;}
		}
		
		public String Message
		{
		  get {return message;}
		}
		
		public Object Value
		{
		  get {return value;}
		}
	}
}
