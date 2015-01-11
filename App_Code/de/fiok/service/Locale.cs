namespace de.fiok.service
{
  using System;
  using System.Globalization;
  using System.Security.Permissions;
  using System.Threading;
  using log4net;
  
  [SecurityPermissionAttribute(SecurityAction.PermitOnly, ControlThread = true)]  
  
  public class Locale
  {
    private static readonly CultureInfo DEFAULT_LOCALE = new CultureInfo ("de-DE");
    private static readonly ILog log = LogManager.GetLogger(typeof(Locale));

    private Locale()
    {
    }
    
    public static void SetDefaultLocale ()
    {
      //log.Debug ("CultureInfo: " + Thread.CurrentThread.CurrentCulture.Name);
      
      Thread.CurrentThread.CurrentCulture = DEFAULT_LOCALE;
      Thread.CurrentThread.CurrentUICulture = DEFAULT_LOCALE;
      
      //log.Debug ("CultureInfo: " + Thread.CurrentThread.CurrentCulture.Name);
    }
  }
}
