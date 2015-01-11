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

namespace de.fiok.web
{
  public class StaticRoleProvider : RoleProvider
  {
    private static readonly Dictionary<string, List<string>> roleMap = new Dictionary<string, List<string>> ();

    static StaticRoleProvider ()
    {
      roleMap.Add (LandlordRole.ADMIN.ToString (), new List<string> ());
      roleMap.Add (LandlordRole.CALENDAR.ToString (), new List<string> ());
      roleMap.Add (LandlordRole.DEFAULT.ToString (), new List<string> ());
      roleMap.Add (LandlordRole.STANDARD.ToString (), new List<string> ());
    }

    //
    // System.Web.Security.RoleProvider methods.
    //

    public override string ApplicationName
    {
      get { return "booking"; }
      set { }
    } 

    //
    // RoleProvider.AddUsersToRoles
    //
    public override void AddUsersToRoles (string[]  usernames, string[] rolenames)
    {
      foreach (string role in rolenames) {
        List<string> users = roleMap[role];
        if (users == null) {
          throw new ProviderException ("unknown role: " + role);
        }
        else {
          foreach (string user in users) {
            if (users.Contains (user)) {
              throw new ProviderException ("user " + user + " already in role " + role);
            }
            else {
              users.Add (user);
            }
		    	}
        }
      }
    }

    //
    // RoleProvider.CreateRole
    //
    public override void CreateRole (string rolename)
    { 
      throw new ProviderException ("unsupported operation");
    }

    //
    // RoleProvider.DeleteRole
    //
    public override bool DeleteRole (string rolename, bool throwOnPopulatedRole)
    {
      throw new ProviderException ("unsupported operation");
    }

    //
    // RoleProvider.GetAllRoles
    //
    public override string[] GetAllRoles ()
    {
      throw new ProviderException ("unsupported operation");
    }

    //
    // RoleProvider.GetRolesForUser
    //
    public override string[] GetRolesForUser(string username)
    {
      throw new ProviderException ("unsupported operation");
    }


    //
    // RoleProvider.GetUsersInRole
    //
    public override string[] GetUsersInRole(string rolename)
    {
      throw new ProviderException ("unsupported operation");
    }


    //
    // RoleProvider.IsUserInRole
    //
    public override bool IsUserInRole (string username, string rolename)
    {
      List<string> users = roleMap[rolename];
      if (users == null) {
        throw new ProviderException ("unknown role " + rolename);
      }
      else {
        return users.Contains (username);
      }
    }


    //
    // RoleProvider.RemoveUsersFromRoles
    //
    public override void RemoveUsersFromRoles(string[] usernames, string[] rolenames)
    {
      throw new ProviderException ("unsupported operation");
    }


    //
    // RoleProvider.RoleExists
    //
    public override bool RoleExists(string rolename)
    {
      throw new ProviderException ("unsupported operation");
    }

    //
    // RoleProvider.FindUsersInRole
    //
    public override string[] FindUsersInRole(string rolename, string usernameToMatch)
    {
      throw new ProviderException ("unsupported operation");
    }
  }
}
