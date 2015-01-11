using System;
using System.Collections;
using System.Text;
using System.Web;
using de.fiok.core;
using de.fiok.service;

namespace de.fiok.handler
{
  public class DomainCheckerHandler : IHttpHandler
  {
    private static readonly String NOT_REGISTERED = "not registered"; 

    public DomainCheckerHandler ()
    {
    }

    public void ProcessRequest (HttpContext context)
    {
      String registeredDomain = NOT_REGISTERED;

      HttpRequest request = context.Request;
      HttpResponse response = context.Response;

      try {
        String domain = request.Params["domain"];
        registeredDomain = LandlordService.GetInstance ().GetDomainOfDemoAccount (domain);
        if (registeredDomain.Length == 0) {
          registeredDomain = NOT_REGISTERED;
        }
      }
      catch (Exception ex) {
        // ignore
      }

      response.Write (registeredDomain);
    }

    public bool IsReusable
    {
      get { return true; }
    }
  }
}
