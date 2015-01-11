namespace de.fiok.core
{
  using System;
  using System.Resources;
  using System.Reflection;
  using de.fiok.resources;
  using System.Web;
  
	/// <summary>
	/// Über die Klasse Resources erfolgt der Zugriff auf die lokalisierten Texte der 
	/// Anwendung.
	/// </summary>
	/// <remarks>
	/// created by - Steffen Förster  
	/// </remarks>
	public sealed class AppResources
	{
		private static AppResources instance = new AppResources();
    private static ResXResourceSet manager;
		
		public static AppResources Instance 
		{
			get {
				return instance;
			}
		}

    private AppResources()
		{
          //manager = BookingResources.GetResourceManager();
          //manager = AppResources.ResourceManager;

          String path = HttpContext.Current.Server.MapPath("~/App_GlobalResources/AppResources.resx");
          manager = new ResXResourceSet(path);
		}
		
		public static String GetMessage (String key)
		{
		  return manager.GetString (key);
		}
		
		public static string GetMessage (String key, params Object[] args)
		{
		  String msg = GetMessage (key);
		  if (msg != null && args.Length > 0) {
		    return String.Format (msg, args);
		  }
		  else {
		    return msg;
		  }
		}
	}
}
