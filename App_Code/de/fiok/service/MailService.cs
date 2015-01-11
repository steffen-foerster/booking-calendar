namespace de.fiok.service
{
  using System;
  using System.Configuration;
  using System.Net;
  using System.Net.Configuration;
  using System.Net.Mail;
  using System.Text;
  using de.fiok.core;
  using log4net;

  /// <summary>
  /// Über diesen Service können E-Mails versendet werden.
  /// </summary>
  /// <remarks>
	/// created by - Steffen Förster
	/// </remarks>
  public class MailService
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(MailService));

    private static MailService instance = new MailService ();

    private static NetworkCredential smtpCred = new NetworkCredential (
      ConfigurationManager.AppSettings["mail_user"],
      ConfigurationManager.AppSettings["mail_password"]
    );

    private MailService ()
    {
    }

    public static MailService GetInstance ()
    {
      return instance;
    }

    /// <summary>
    /// Sendet eine E-Mail an den übergebenen Empfänger.
    /// <summary>
    public void SendMail (String msgSubject, String msgBody, String recipient)
    {
      log.Debug ("MailService.SendMail");

      MailMessage msg = new MailMessage (
        ConfigurationManager.AppSettings["mail_address"],
        recipient,
        msgSubject,
        msgBody
      );

      msg.BodyEncoding = Encoding.GetEncoding("ISO-8859-1");
      
      PerformSend (msg);
    }
    
    private void PerformSend (MailMessage msg)
    {
      log.Debug ("MailService.performSend");
      
      try {
        SmtpClient client = new SmtpClient(ConfigurationManager.AppSettings["mail_server"]);
        client.Credentials = smtpCred;
        client.Send (msg);
      }
      catch (Exception e) {
        String errorMsg = "error occurred sending email, error-msg: " + e.Message;
        log.Error (errorMsg, e);
        throw e;
      }
    }
  }
}
