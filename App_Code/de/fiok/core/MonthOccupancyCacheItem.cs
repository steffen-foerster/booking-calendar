namespace de.fiok.core 
{
	using System;
	using System.Collections;
	using de.fiok.service;
	
	/// <summary>
	/// Die einzelnen Statusinformationen f�r jeden Buchungsmonat werden im ASP.NET-Cache
	/// gespeichert, bis eine �nderung der Buchungsinformationen durchgef�hrt wurde. Eine Instanz
	/// dieser Klasse h�lt die Referenzen auf alle schon erzeugten Informationen.
	/// </summary>
	/// <remarks>
	/// created by - Steffen F�rster
	/// </remarks>
	public class MonthOccupancyCacheItem
	{
	  private IDictionary calenders = new Hashtable ();
	  
		/// <summary>
		/// Default constructor.
		/// </summary>
		public MonthOccupancyCacheItem() 
		{
		}
		
		/// <summary>
		/// F�gt einen MonthOccupancy dem Cache hinzu.
		/// </summary>
		public void AddMonthOccupancy (MonthOccupancy occupancy)
		{
		  calenders[occupancy.HouseId.ToString () + "_" + occupancy.Year.ToString () + "_" + 
		            occupancy.Month.ToString ()] = occupancy;
		}
		
		/// <summary>
		/// Liefert den BookingState f�r einen bestimmten Monat, der im Cache gespeichert wurde, 
		/// oder 'null' wenn ein solcher BookingState noch nicht existiert.
		/// </summary>
		public MonthOccupancy GetMonthOccupancy (int year, int month, int houseId)
		{
		  return (MonthOccupancy)calenders[houseId.ToString () + "_" + year.ToString () + "_" + 
		                                   month.ToString ()];
		}
	}
}
