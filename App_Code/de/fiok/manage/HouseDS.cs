using System;
using System.Collections.Generic;
using System.Text;
using de.fiok.service;
using log4net;


namespace de.fiok.manage
{
  public class HouseDS : BaseService
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(HouseDS));

    private static readonly HouseService houseService = HouseService.GetInstance ();

    public HouseDS ()
    {
    }

    public static IEnumerable<HouseBean> GetHouses (int id)
    {
      log.Debug ("HouseDS.GetHouses");

      return houseService.RetrieveHouseBeansByLandlord (id);
    }

    public static HouseBean GetHouse (int id)
    {
      log.Debug ("HouseDS.GetHouse");

      return houseService.RetrieveHouse (id);
    }

    public static void UpdateHouse (HouseBean house)
    {
      log.Debug ("HouseDS.UpdateHouse");

      houseService.UpdateHouseStandardAttr (house); 
    }

    public static void InsertHouse (HouseBean house)
    {
      log.Debug ("HouseDS.InsertHouse");

      houseService.InsertHouse (house); 
    }

    public static void DeleteHouse (HouseBean house)
    {
      log.Debug ("HouseDS.DeleteHouse");

      houseService.DeleteHouse (house); 
    }
  }
}
