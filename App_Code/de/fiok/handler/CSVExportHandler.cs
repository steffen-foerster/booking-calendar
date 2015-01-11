using System;
using System.Collections;
using System.Text;
using System.Web;
using de.fiok.core;
using de.fiok.type;
using de.fiok.service;

namespace de.fiok.handler
{
  /// <summary>
  /// Handler, zur Erzeugung von CSV-Dateien, die zur Erstellung von Serienbriefen
  /// in Word eingesetzt werden.
  /// </summary>
  public class CSVExportHandler : IHttpHandler
  {
    private static readonly double PREPAYMENT_PERCENT = 20.0;
    private static readonly double TAX_PERCENT = 7.0;

    public CSVExportHandler ()
    {
    }

    public void ProcessRequest (HttpContext context)
    {
      HttpRequest request = context.Request;
      HttpResponse response = context.Response;

      // Buchung laden
      String bookingIdStr = request.Params["bookingId"];
      int bookingId = Int32.Parse (bookingIdStr);
      
      BookingService bService = BookingService.GetInstance ();
      BookingItem bookingItem = bService.RetrieveBooking (bookingId);

      StringBuilder csvFile = new StringBuilder ();
      csvFile.Append ("#Name;Vorname;Anrede;Titel;Straﬂe;PLZ;Ort;Telefon;E-Mail;Fax;Preis;Endreinigung;Gesamtkosten;Anzahlung;Restzahlung;Netto;Umsatzsteuer;Anreise;Abreise;‹bernachtungen;Buchungsdatum;Werbung;Alter_Kinder;Anzahl_Erw;Anz_Kinder");      
      csvFile.Append ("\n");
      csvFile.Append (bookingItem.Tenant.Name);
      csvFile.Append (";");
      csvFile.Append (bookingItem.Tenant.Firstname);
      csvFile.Append (";");
      csvFile.Append (SalutationType.GetName (bookingItem.Tenant.Salutation));
      csvFile.Append (";");
      csvFile.Append (bookingItem.Tenant.Title);
      csvFile.Append (";");
      csvFile.Append (bookingItem.Tenant.Street);
      csvFile.Append (";");
      csvFile.Append (bookingItem.Tenant.Zipcode);
      csvFile.Append (";");
      csvFile.Append (bookingItem.Tenant.Location);
      csvFile.Append (";");
      csvFile.Append (bookingItem.Tenant.Telephone);
      csvFile.Append (";");
      csvFile.Append (bookingItem.Tenant.Email);
      csvFile.Append (";");
      csvFile.Append (bookingItem.Tenant.Fax);
      csvFile.Append (";");
      csvFile.Append (String.Format ("{0},00", bookingItem.Price.Rent));
      csvFile.Append (";");
      csvFile.Append (String.Format ("{0},00", bookingItem.Price.CleaningCost));
      csvFile.Append (";");
      csvFile.Append (String.Format ("{0},00", (bookingItem.Price.Rent + bookingItem.Price.CleaningCost)));
      csvFile.Append (";");
      // Anzahlung
      double prepaymentValue = (bookingItem.Price.Rent + bookingItem.Price.CleaningCost) * PREPAYMENT_PERCENT / 100;
      prepaymentValue = Math.Round (prepaymentValue, 2, MidpointRounding.AwayFromZero);
      // Restzahlung
      double remainder = (bookingItem.Price.Rent + bookingItem.Price.CleaningCost) - prepaymentValue;
      // Umsatzsteuer 
      double taxValue = (bookingItem.Price.Rent + bookingItem.Price.CleaningCost) * TAX_PERCENT / (100 + TAX_PERCENT);
      taxValue = Math.Round (taxValue, 2, MidpointRounding.AwayFromZero);
      // Netto 
      double netValue = (bookingItem.Price.Rent + bookingItem.Price.CleaningCost) - taxValue;

      csvFile.Append (prepaymentValue.ToString ("#,##0.00"));
      csvFile.Append (";");
      csvFile.Append (remainder.ToString ("#,##0.00"));
      csvFile.Append (";");
      csvFile.Append (netValue.ToString ("#,##0.00"));
      csvFile.Append (";");
      csvFile.Append (taxValue.ToString ("#,##0.00"));
      csvFile.Append (";");
      csvFile.Append (bookingItem.Arrival.ToShortDateString ());
      csvFile.Append (";");
      csvFile.Append (bookingItem.Departure.ToShortDateString ());
      csvFile.Append (";");
      csvFile.Append ((int)(bookingItem.Departure - bookingItem.Arrival).TotalDays);
      csvFile.Append (";");
      csvFile.Append (bookingItem.BookingDate.ToShortDateString ());
      csvFile.Append (";");
      csvFile.Append (bookingItem.PromotionPartner);
      csvFile.Append(";");
      csvFile.Append(bookingItem.AgeChildren);
      csvFile.Append(";");
      csvFile.Append(bookingItem.CountAdults);
      csvFile.Append(";");
      csvFile.Append(bookingItem.CountChildren);

      response.ContentType = "text/csv";
      response.AppendHeader ("Content-Length", csvFile.Length.ToString ());
      response.AppendHeader ("content-disposition", "attachment; filename=booking_" + bookingIdStr + ".csv");

      response.Write (csvFile.ToString ());
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
