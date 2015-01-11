<%@ Control Language="C#" AutoEventWireup="true" %>
<%@ Register TagPrefix="booking" Namespace="de.fiok.controls" %>
<%@ import Namespace="System.Collections" %>
<%@ import Namespace="de.fiok.core" %>

<script runat="server">

  int houseId;
  ArrayList dates = new ArrayList ();
  
  public int HouseID 
  {
    set { houseId = value; } 
  }

  void Page_Init (Object sender, EventArgs e)
  {
    DateTime currentDate = DateTime.Now;

    for (int i = 0; i < 12; i++)
    {
      dates.Add(currentDate);
      currentDate = currentDate.AddMonths(1);
    }
  }
  
  void Page_PreRender (Object sender, EventArgs e)
  {
    this.DataBind();
  }
</script>

<table>
  <tr>
    <td style="vertical-align:top;">
      <booking:OccupancyCalendar  year="<%# ((DateTime)dates[0]).Year %>" month="<%# ((DateTime)dates[0]).Month %>" 
                                  houseId="<%# houseId %>" imagePath="../../images" runat="server"/>
    </td>
    <td style="vertical-align:top;">
      <booking:OccupancyCalendar  year="<%# ((DateTime)dates[1]).Year %>" month="<%# ((DateTime)dates[1]).Month %>" 
                                  houseId="<%# houseId %>" imagePath="../../images" runat="server"/>
    </td>
    <td style="vertical-align:top;">
      <booking:OccupancyCalendar  year="<%# ((DateTime)dates[2]).Year %>" month="<%# ((DateTime)dates[2]).Month %>" 
                                  houseId="<%# houseId %>" imagePath="../../images" runat="server"/>
    </td>
    <td style="vertical-align:top;">
      <booking:OccupancyCalendar  year="<%# ((DateTime)dates[3]).Year %>" month="<%# ((DateTime)dates[3]).Month %>" 
                                  houseId="<%# houseId %>" imagePath="../../images" runat="server"/>
    </td>
  </tr>
  <tr>
    <td style="vertical-align:top;">
      <booking:OccupancyCalendar  year="<%# ((DateTime)dates[4]).Year %>" month="<%# ((DateTime)dates[4]).Month %>" 
                                  houseId="<%# houseId %>" imagePath="../../images" runat="server"/>
    </td>
    <td style="vertical-align:top;">
      <booking:OccupancyCalendar  year="<%# ((DateTime)dates[5]).Year %>" month="<%# ((DateTime)dates[5]).Month %>" 
                                  houseId="<%# houseId %>" imagePath="../../images" runat="server"/>
    </td>
    <td style="vertical-align:top;">
      <booking:OccupancyCalendar  year="<%# ((DateTime)dates[6]).Year %>" month="<%# ((DateTime)dates[6]).Month %>" 
                                  houseId="<%# houseId %>" imagePath="../../images" runat="server"/>
    </td>
    <td style="vertical-align:top;">
      <booking:OccupancyCalendar  year="<%# ((DateTime)dates[7]).Year %>" month="<%# ((DateTime)dates[7]).Month %>" 
                                  houseId="<%# houseId %>" imagePath="../../images" runat="server"/>
    </td>
  </tr>
  <tr>
    <td style="vertical-align:top;">
      <booking:OccupancyCalendar  year="<%# ((DateTime)dates[8]).Year %>" month="<%# ((DateTime)dates[8]).Month %>" 
                                  houseId="<%# houseId %>" imagePath="../../images" runat="server"/>
    </td>
    <td style="vertical-align:top;">
      <booking:OccupancyCalendar  year="<%# ((DateTime)dates[9]).Year %>" month="<%# ((DateTime)dates[9]).Month %>" 
                                  houseId="<%# houseId %>" imagePath="../../images" runat="server"/>
    </td>
    <td style="vertical-align:top;">
      <booking:OccupancyCalendar  year="<%# ((DateTime)dates[10]).Year %>" month="<%# ((DateTime)dates[10]).Month %>" 
                                  houseId="<%# houseId %>" imagePath="../../images" runat="server"/>
    </td>
    <td style="vertical-align:top;">
      <booking:OccupancyCalendar  year="<%# ((DateTime)dates[11]).Year %>" month="<%# ((DateTime)dates[11]).Month %>" 
                                  houseId="<%# houseId %>" imagePath="../../images" runat="server"/>
    </td>
  </tr>
</table>

<div style="margin-top:20px;text-align:left;padding-left:5px;">
  <span>
    <img src="../../images/booked.gif" height="14" width="14" style="border:1px solid gray;vertical-align:top;" class="middle" alt="booked"/>
         &nbsp;<booking:message key="Booking_lb.booked" runat="server"/>
  </span>
  <span style="margin-left:30px;">
    <img src="../../images/arrival.gif" height="14" width="14" style="border:1px solid gray;vertical-align:top;" class="middle" alt="arrival"/>
         &nbsp;<booking:message key="Booking_lb.arrival" runat="server"/>
  </span>
  <span style="margin-left:30px;">
    <img src="../../images/departure.gif" height="14" width="14" style="border:1px solid gray;vertical-align:top;" class="middle" alt="departure"/>
         &nbsp;<booking:message key="Booking_lb.departure" runat="server"/>
  </span>
</div>
