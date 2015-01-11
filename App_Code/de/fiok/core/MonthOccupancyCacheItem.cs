namespace de.fiok.core 
{
	using System;
	using System.Collections;
	using de.fiok.service;
	
	/// <summary>
	/// Die einzelnen Statusinformationen für jeden Buchungsmonat werden im ASP.NET-Cache
	/// gespeichert, bis eine Änderung der Buchungsinformationen durchgeführt wurde. Eine Instanz
	/// dieser Klasse hält die Referenzen auf alle schon erzeugten Informationen.
	/// </summary>
	/// <remarks>
	/// created by - Steffen Förster
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
		/// Fügt einen MonthOccupancy dem Cache hinzu.
		/// </summary>
		public void AddMonthOccupancy (MonthOccupancy occupancy)
		{
		  calenders[occupancy.HouseId.ToString () + "_" + occupancy.Year.ToString () + "_" + 
		            occupancy.Month.ToString ()] = occupancy;
		}
		
		/// <summary>
		/// Liefert den BookingState für einen bestimmten Monat, der im Cache gespeichert wurde, 
		/// oder 'null' wenn ein solcher BookingState noch nicht existiert.
		/// </summary>
		public MonthOccupancy GetMonthOccupancy (int year, int month, int houseId)
		{
		  return (MonthOccupancy)calenders[houseId.ToString () + "_" + year.ToString () + "_" + 
		                                   month.ToString ()];
		}
	}
}
