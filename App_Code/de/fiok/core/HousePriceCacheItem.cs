namespace de.fiok.core 
{
	using System;
	using System.Collections;
	using de.fiok.service;
	
	/// <summary>
	/// Die Preisinformationen f�r ein Haus werden im ASP.NET-Cache gespeichert. Eine Aktualisierung 
	/// ist nach dem Speichern neuer Preise notwendig.
	/// </summary>
	/// <remarks>
	/// created by - Steffen F�rster
	/// </remarks>
	public class HousePriceCacheItem
	{
	  private IDictionary houses = new Hashtable ();
	  
		/// <summary>
		/// Default constructor.
		/// </summary>
		public HousePriceCacheItem () 
		{
		}
		
		/// <summary>
		/// F�gt eine Liste mit Objekten vom Typ HousePriceInterval dem Cache hinzu.
		/// </summary>
		public void AddHousePriceIntervalList (IList priceIntervals, int houseId)
		{
		  houses[houseId] = priceIntervals;
		}
		
		/// <summary>
		/// Liefert eine Liste mit Objekten vom Typ HousePriceInterval, die im Cache gespeichert wurde, 
		/// oder 'null' wenn eine solche Liste noch nicht existiert.
		/// </summary>
		public IList GetHousePriceIntervalList (int houseId)
		{
		  return (IList)houses[houseId];
		}
	}
}
