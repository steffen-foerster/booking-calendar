namespace de.fiok.log
{
  using System;
  using log4net;
  using log4net.Config;
  using log4net.Util;
  using log4net.Appender;
  using de.fiok.service;
  using System.IO;
  using System.Diagnostics;

  /// <summary>
  /// Klasse zur Configuration von log4net.
  /// </summary>
  /// <remarks>
	/// created by - Steffen Förster
	/// </remarks>
  public class LogConfig
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(LogConfig));

    public LogConfig()
    {
    }

    /// <summary>
    /// Hier wird der Pfad zur Log-Datei gesetzt. So muss der absolute Pfad nicht in der
    /// Konfigurations-Datei stehen, sondern wird beim Start der Applikation dynamisch ermittelt.
    /// </summary>
    public static void ConfigLog4net (String applicationPath)
    {
      log4net.Config.XmlConfigurator.Configure();

      log.Info ("setting new path ... ");

      FileAppender appender = (FileAppender)LogManager.GetAllRepositories()[0].GetAppenders()[0];
      appender.File = applicationPath + "\\log\\booking-system.log";
      appender.ActivateOptions ();

      log.Info ("end log4net configuration");
    }
  }
}
