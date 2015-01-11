namespace de.fiok.controls
{
  using System;
  using System.Web;
  using System.Web.UI;
  using de.fiok.service;
  using de.fiok.core;
  using log4net;

  /// <summary>
  /// Control zur Ausgabe eines Monat als Kalender mit farbiger Kennzeichnung 
  /// der belegten Tage.
  /// </summary>
  /// <remarks>
  /// created by - Steffen Förster  
  /// </remarks>
  public class OccupancyCalendar : Control
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(OccupancyCalendar));

    private static BookingService bService = BookingService.GetInstance();

    #region Properties

    private int year;
    private int month;
    private int houseId = -1;
    private String imagePath;

    public int Year
    {
      set { this.year = value; }
    }

    public int Month
    {
      set { this.month = value; }
    }

    public int HouseId
    {
      set { this.houseId = value; }
    }

    public String ImagePath
    {
      set { this.imagePath = value; }
    }

    #endregion

    /// <summary>
    /// Rendert einen Monat. Es werden die gebuchten Tage farbig hinterlegt.
    /// </summary>
    protected override void Render(HtmlTextWriter html)
    {
      MonthOccupancy occupancy = bService.CreateMonthOccupancy(this.houseId, this.year, this.month);
      OccupancyCalendarModel model = new OccupancyCalendarModel(this.year, this.month, occupancy);

      html.Write("<table border='0' cellspacing='1' cellpadding='0'>");
      html.Write("<tr><td style='border:1px solid #888888'>");
      html.Write("<table border='0' cellspacing='1' cellpadding='0'>");

      writeHeader(html, model);
      writeDayNames(html, model);
      writeWeeks(html, model);

      html.Write("</table>");
      html.Write("</td></tr>");
      html.Write("</table>");
    }

    /// <summary>
    /// "Monat Jahr" ausgeben.
    /// </summary>
    private void writeHeader(HtmlTextWriter html, OccupancyCalendarModel model)
    {
      html.Write("<tr><td class='month' colspan='7'>" + model.AsString + "</td></tr>");
    }

    /// <summary>
    /// Namen der Wochentage ausgeben.
    /// </summary> 
    private void writeDayNames (HtmlTextWriter html, OccupancyCalendarModel model)
    {
      html.Write("<tr>");

      foreach (DayName day in model.DaysOfWeek)
      {
        html.Write("<td class='dayname'>" + day.Name + "</td>");
      }

      html.Write("</tr>");
    }

    /// <summary>
    /// Die einzelnen Wochen ausgeben.
    /// </summary> 
    private void writeWeeks(HtmlTextWriter html, OccupancyCalendarModel model)
    {
      foreach (Week week in model.Weeks)
      {
        html.Write("<tr>");

        int index = 0;
        foreach (int day in week.Days)
        {
          if (day == -1) {
            html.Write("<td class='" + GetStyleClass(index) + "'>&nbsp;</td>");
          }
          else {
            DateTime date = new DateTime(model.Year, model.Month, day);
            BookingDay bookingDay = model.Occupancy.GetBookingDay(date);

            html.Write("<td class='" + GetStyleClass(index) + "'" +
                       "    style='" + GetBackgroundStyle(bookingDay) + "'>" + day + "</td>");
          }
          
          index++;
        }

        html.Write("</tr>");
      }
    }

    /// <summary>
    /// Liefert die CSS-Klasse für den entsprechenden Wochentag.
    /// </summary> 
    //private String GetStyleClass(int index, OccupancyCalendarModel model)
    //{
    //  DayName dayName = (DayName)model.DaysOfWeek[index];
    //  String result = "weekday";

    //  switch (dayName.DayWeek)
    //  {
    //    case DayOfWeek.Sunday:
    //      result = "sunday";
    //      break;
    //    case DayOfWeek.Saturday:
    //      result = "saturday";
    //      break;
    //  }

    //  return result;
    //}

    private String GetStyleClass (int index)
    {
      String result = "weekday";

      switch (index)
      {
        case 6:
          result = "sunday";
          break;
        case 5:
          result = "saturday";
          break;
      }

      return result;
    }

    /// <summary>
    /// Liefert das Hintergrundbild für die einzelnen Tage im Monat entsprechend der Buchungen.
    /// </summary> 
    private String GetBackgroundStyle(BookingDay day)
    {
      if (day == null)
      {
        return "";
      }
      else if (day.Arrival && day.Departure)
      {
        return "background-image:url(" + imagePath + "/booked.gif);";
      }
      else if (day.Arrival)
      {
        return "background-image:url(" + imagePath + "/arrival.gif);";
      }
      else if (day.Departure)
      {
        return "background-image:url(" + imagePath + "/departure.gif);";
      }
      else
      {
        return "background-image:url(" + imagePath + "/booked.gif);";
      }
    }
  }
}
