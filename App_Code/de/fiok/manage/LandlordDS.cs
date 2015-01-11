using System;
using System.Collections.Generic;
using System.Text;
using de.fiok.service;
using log4net;

namespace de.fiok.manage
{
  public class LandlordDS : BaseService
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(LandlordDS));

    private static readonly LandlordService landlordService = LandlordService.GetInstance ();

    public LandlordDS ()
    {
    }

    public static IEnumerable<LandlordBean> GetLandlords ()
    {
      log.Debug ("LandlordDS.GetLandlords");

      return landlordService.RetrieveLandlords ();
    }

    public static LandlordBean GetLandlord (int id)
    {
      log.Debug ("LandlordDS.GetLandlord");

      return landlordService.RetrieveLandlord (id);
    }

    public static void UpdateLandlord (LandlordBean landlord)
    {
      log.Debug ("LandlordDS.UpdateLandlord");

      landlordService.UpdateLandlord (landlord); 
    }

    public static void InsertLandlord (LandlordBean landlord)
    {
      log.Debug ("LandlordDS.InsertLandlord");

      landlordService.InsertLandlord (landlord);
    }

    public static void DeleteLandlord (LandlordBean landlord)
    {
      log.Debug ("LandlordDS.DeleteLandlord");

      landlordService.DeleteLandlord (landlord);
    }
  }
}
