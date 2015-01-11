using System.Web.Security;
using System.Configuration.Provider;
using System.Collections.Specialized;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.Odbc;
using System.Configuration;
using System.Diagnostics;
using System.Web;
using System.Globalization;
using de.fiok.type;
using de.fiok.core;
using de.fiok.service;

namespace de.fiok.web
{
  public class LandlordRoleProvider
  {
    private static readonly ThreadLocal allRoles = new ThreadLocal ();

    public static void AddRoles (LandlordBean landlord)
    {
      List<LandlordRole> userRoles = new List<LandlordRole> ();
      userRoles.Add (landlord.Role);
      allRoles.Add (userRoles);
    }

    public static void RemoveRoles ()
    {
      allRoles.Remove ();
    }

    public static bool IsInRole (LandlordRole role)
    {
      List<LandlordRole> userRoles = (List<LandlordRole>)allRoles.Get ();
      return userRoles != null && userRoles.Contains (role);
    }
  }
}
