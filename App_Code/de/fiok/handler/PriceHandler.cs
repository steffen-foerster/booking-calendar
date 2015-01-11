using System;
using System.Collections;
using System.Text;
using System.Web;
using de.fiok.core;
using de.fiok.service;

namespace de.fiok.handler
{
  public class PriceHandler : IHttpHandler
  {
    private static readonly String CSS_ITEM_TABLE = 
      "color:#333333;font-family:Verdana;font-size: 9pt;empty-cells:show;border:0;border-collapse:collapse";
    private static readonly String CSS_HCELL_STYLE = 
      "background-color:#DDDDDD;color:#022585;border:1px solid #a7a7a7;font-weight: bold;padding:5px;text-align:center";
    private static readonly String CSS_CELL_STYLE = 
      "border:1px solid #a7a7a7;padding:5px;text-align:center;";

    public PriceHandler ()
    {
    }

    public void ProcessRequest (HttpContext context)
    {
      HttpRequest request = context.Request;
      HttpResponse response = context.Response;
      
      response.Write ("<table cellpadding='0' cellspacing='0' style='" + CSS_ITEM_TABLE + "'>");

      WriteHeader (response);
      WritePrices (response, request);

      response.Write ("</table>");
    }

    
    private void WriteHeader (HttpResponse response)
    {
      String header = 
        "<tr>" +
        "  <td style='" + CSS_HCELL_STYLE + ";width: 75px;'>" +
        AppResources.GetMessage("AdminHouse_lb.head.date.from") +   
        "  </td>" +
        "  <td style='" + CSS_HCELL_STYLE + ";width: 75px;'>" +
        AppResources.GetMessage("AdminHouse_lb.head.date.to") +   
        "  </td>" +
        "  <td style='" + CSS_HCELL_STYLE + ";width: 60px;'>" +
        AppResources.GetMessage("AdminHouse_lb.head.price") +   
        "  </td>" +
        "  <td style='" + CSS_HCELL_STYLE + ";width: 110px;'>" +
        AppResources.GetMessage("AdminHouse_lb.head.arrival.days") +   
        "  </td>" +
        "  <td style='" + CSS_HCELL_STYLE + ";width: 110px;'>" +
        AppResources.GetMessage("AdminHouse_lb.head.departure.days") + 
        "  </td>" +
        "  <td style='" + CSS_HCELL_STYLE + ";width: 90px;'>" +
        AppResources.GetMessage("AdminHouse_lb.head.min.booking.days") +   
        "  </td>" +   
        "</tr>";

      response.Write (header);
    }

    private void WritePrices (HttpResponse response, HttpRequest request)
    {
      String houseIdStr = request.Params["houseId"];
      int houseId = Int32.Parse (houseIdStr);
      IList prices = LoadHousePrices (houseId);

      foreach (HousePriceInterval price in prices) {
        String priceRow =
          "<tr>" +
          "  <td style='" + CSS_CELL_STYLE + ";width: 75px;'>" +
          price.Start.ToShortDateString () +
          "  </td>" + 
          "  <td style='" + CSS_CELL_STYLE + ";width: 75px;'>" +
          price.End.ToShortDateString () +
          "  </td>" + 
          "  <td style='" + CSS_CELL_STYLE + ";width: 60px;'>" +
          price.Price + " EUR" +
          "  </td>" +
          "  <td style='" + CSS_CELL_STYLE + ";width: 110px;'>" +
          price.ArrivalDays.BuildDayListString +
          "  </td>" +
          "  <td style='" + CSS_CELL_STYLE + ";width: 110px;'>" +
          price.DepartureDays.BuildDayListString +
          "  </td>" +
          "  <td style='" + CSS_CELL_STYLE + ";width: 90px;'>" +
          AppResources.GetMessage("AdminHouse_lb.min.booking.days", price.MinBookingDays) +
          "  </td>" +
          "</tr>";

        response.Write (priceRow);
      }
    }

    private IList LoadHousePrices (int houseId)
    {
      HouseService hService = HouseService.GetInstance ();

      // Preise für die nächsten zwei Jahre abfragen
      DateTime dateFrom = new DateTime (DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
      DateTime dateTo= new DateTime (DateTime.Now.Year + 2, DateTime.Now.Month, DateTime.Now.Day);
      
      return hService.RetrieveHousePrices (houseId, dateFrom, dateTo);
    }

    /// <summary>
    /// Dieser Handler kann wiederverwendet werden.
    /// </summary>
    public bool IsReusable
    {
      get { return true; }
    }
  }
}
