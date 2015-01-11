namespace de.fiok.core 
{
	using System;
	using System.Collections;
	using System.Web.Caching;
	using de.fiok.service;
  using log4net;
	
	/// <summary>
	/// Über diesen Handler werden alle Objekte verwaltet, die sich im ASP.NET Cache befinden.
	/// </summary>
	/// <remarks>
	/// created by - Steffen Förster
	/// </remarks>
	public class CacheHandler 
	{
    private static readonly ILog log = LogManager.GetLogger (typeof (CacheHandler));

    private static String MONTH_OCCUPANCY_KEY = "month.occupancy";
    private static String HOUSE_PRICE_KEY = "house.price";
    private static CacheHandler instance = new CacheHandler ();
    private static Cache appCache;
	  
		private CacheHandler () {}
		
		public static CacheHandler Instance
		{
		  get {return instance;} 
		}
		
		/// <summary>
		/// Vor der ersten Verwendung wird der CacheHandler initialisiert -> beim Applikationsstart.
		/// </summary>
		public static void Init (Cache cache)
		{
      log.Debug ("CacheHandler.init");

      appCache = cache;
		}
		
		#region MonthOccupancy
		
		/// <summary>
		/// Einen Buchungsstatus dem Cache hinzufügen.
		/// </summary>
		public void AddMonthOccupancy (MonthOccupancy occupancy)
		{
		  MonthOccupancyCacheItem item = (MonthOccupancyCacheItem)appCache.Get (MONTH_OCCUPANCY_KEY);
      if (item == null) {
        item = new MonthOccupancyCacheItem ();
        appCache.Insert (MONTH_OCCUPANCY_KEY, item);
      }
		  item.AddMonthOccupancy (occupancy);
		}
		
		/// <summary>
		/// Einen Buchungsstatus aus dem Cache holen und zurückliefern.
		/// </summary>
		public MonthOccupancy GetMonthOccupancy (int year, int month, int houseId)
		{
		  MonthOccupancyCacheItem item = (MonthOccupancyCacheItem)appCache.Get (MONTH_OCCUPANCY_KEY);
      if (item == null) {
        return null;
      }
      else {
        return item.GetMonthOccupancy (year, month, houseId);
      }
		}
		
		/// <summary>
		/// Liefert true, wenn ein Buchungsstatus für das Tupel (Jahr, Monat) 
		//  schon im Cache gespeichert ist.
		/// </summary>
		public bool ContainsMonthOccupancy (int year, int month, int houseId)
		{
		  return GetMonthOccupancy (year, month, houseId) != null;
		}
		
		/// <summary>
		/// Alle Buchungsstatus aus dem Cache entfernen.
		/// </summary>
		public void RemoveAllMonthOccupancies ()
		{
		  appCache.Insert (MONTH_OCCUPANCY_KEY, new MonthOccupancyCacheItem ());
		}
		
		#endregion
		
		#region IList<HousePriceInverval>
		
		/// <summary>
		/// Eine Liste mit Objekten vom Typ HousePriceInverval dem Cache hinzufügen.
		/// </summary>
		public void AddHousePriceIntervalList (IList priceList, int houseId)
		{
		  HousePriceCacheItem item = (HousePriceCacheItem)appCache.Get (HOUSE_PRICE_KEY);
      if (item == null) {
        item = new HousePriceCacheItem ();
        appCache.Insert (HOUSE_PRICE_KEY, item);
      }
      item.AddHousePriceIntervalList (priceList, houseId);
		}
		
		/// <summary>
		/// Eine Liste mit Objekten vom Typ HousePriceInverval aus dem Cache holen und zurückliefern.
		/// </summary>
		public IList GetHousePriceIntervalList (int houseId)
		{
		  HousePriceCacheItem item = (HousePriceCacheItem)appCache.Get (HOUSE_PRICE_KEY);
      if (item == null) {
        return null;
      }
      else {
        return item.GetHousePriceIntervalList (houseId);
      }
		}
		
		/// <summary>
		/// Liefert true, wenn eine Liste mit Objekten vom Typ HousePriceInverval für das übergebene
		/// Haus schon im Cache gespeichert ist.
		/// </summary>
		public bool ContainsHousePriceIntervalList (int houseId)
		{
		  return GetHousePriceIntervalList (houseId) != null;
		}
		
		/// <summary>
		/// Alle Preise aus dem Cache entfernen.
		/// </summary>
		public void RemoveAllHousePriceIntervalLists ()
		{
		  appCache.Insert (HOUSE_PRICE_KEY, new HousePriceCacheItem ());
		}
		
		#endregion
	}
}
