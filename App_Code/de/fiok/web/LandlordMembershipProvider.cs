using System.Web.Security;
using System.Configuration.Provider;
using System.Collections.Specialized;
using System;
using System.Data;
using System.Data.Odbc;
using System.Configuration;
using System.Diagnostics;
using System.Web;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Web.Configuration;

namespace de.fiok.web
{

  public class LandlordMembershipProvider: MembershipProvider
  {
    public override void Initialize(string name, NameValueCollection config)
    {
    }

    public override string ApplicationName
    {
      get { return "booking"; }
      set { }
    } 

    public override bool EnablePasswordReset
    {
      get { return false; }
    }


    public override bool EnablePasswordRetrieval
    {
      get { return false; }
    }


    public override bool RequiresQuestionAndAnswer
    {
      get { return false; }
    }


    public override bool RequiresUniqueEmail
    {
      get { return false; }
    }


    public override int MaxInvalidPasswordAttempts
    {
      get { return 3; }
    }


    public override int PasswordAttemptWindow
    {
      get { return 3; }
    }


    public override MembershipPasswordFormat PasswordFormat
    {
      get { return MembershipPasswordFormat.Hashed; }
    }

    public override int MinRequiredNonAlphanumericCharacters
    {
      get { return 1; }
    }

    public override int MinRequiredPasswordLength
    {
      get { return 8; }
    }

    public override string PasswordStrengthRegularExpression
    {
      get { return ""; }
    }

    //
    // MembershipProvider.ChangePassword
    //
    public override bool ChangePassword(string username, string oldPwd, string newPwd)
    {
      throw new ProviderException ("unsupported operation");
    }

    //
    // MembershipProvider.ChangePasswordQuestionAndAnswer
    //
    public override bool ChangePasswordQuestionAndAnswer(string username,
                  string password,
                  string newPwdQuestion,
                  string newPwdAnswer)
    {
      throw new ProviderException ("unsupported operation");
    }

    //
    // MembershipProvider.CreateUser
    //
    public override MembershipUser CreateUser(string username,
             string password,
             string email,
             string passwordQuestion,
             string passwordAnswer,
             bool isApproved,
             object providerUserKey,
             out MembershipCreateStatus status)
    {
      throw new ProviderException ("unsupported operation");
    }

    //
    // MembershipProvider.DeleteUser
    //
    public override bool DeleteUser(string username, bool deleteAllRelatedData)
    {
      throw new ProviderException ("unsupported operation");
    }

    //
    // MembershipProvider.GetAllUsers
    //
    public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
    {
      throw new ProviderException ("unsupported operation");
    }

    //
    // MembershipProvider.GetNumberOfUsersOnline
    //
    public override int GetNumberOfUsersOnline()
    {
      throw new ProviderException ("unsupported operation");
    }

    //
    // MembershipProvider.GetPassword
    //
    public override string GetPassword(string username, string answer)
    {
      throw new ProviderException ("unsupported operation");
    }

    //
    // MembershipProvider.GetUser(string, bool)
    //
    public override MembershipUser GetUser(string username, bool userIsOnline)
    {
      throw new ProviderException ("unsupported operation");
    }


    //
    // MembershipProvider.GetUser(object, bool)
    //
    public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
    {
      throw new ProviderException ("unsupported operation");
    }

    //
    // MembershipProvider.UnlockUser
    //
    public override bool UnlockUser(string username)
    {
      throw new ProviderException ("unsupported operation");
    }

    //
    // MembershipProvider.GetUserNameByEmail
    //
    public override string GetUserNameByEmail(string email)
    {
      throw new ProviderException ("unsupported operation");
    }

    //
    // MembershipProvider.ResetPassword
    //
    public override string ResetPassword(string username, string answer)
    {
      throw new ProviderException ("unsupported operation");
    }

    //
    // MembershipProvider.UpdateUser
    //
    public override void UpdateUser(MembershipUser user)
    {
      throw new ProviderException ("unsupported operation");
    }

    //
    // MembershipProvider.ValidateUser
    //
    public override bool ValidateUser(string username, string password)
    {
      throw new ProviderException ("unsupported operation");
    }

    //
    // MembershipProvider.FindUsersByName
    //
    public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
    {
      throw new ProviderException ("unsupported operation");
    }

    //
    // MembershipProvider.FindUsersByEmail
    //
    public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
    {
      throw new ProviderException ("unsupported operation");
    }
  }
}


