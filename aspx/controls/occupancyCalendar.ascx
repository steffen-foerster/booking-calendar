<%@ Control Language="C#" AutoEventWireup="true" %>
<%@ Register TagPrefix="booking" Namespace="de.fiok.controls" %>
<%@ import Namespace="System.Collections" %>
<%@ import Namespace="de.fiok.core" %>
<script runat="server">

  int houseId;
  
  public int HouseId {
    set { houseId = value; }
  }
  
  ArrayList dates = new ArrayList ();

  void Page_Init (Object Src, EventArgs E)
  {
    DateTime currentDate = DateTime.Now;

    for (int i = 0; i < 12; i++) {
      dates.Add (currentDate);
      currentDate = currentDate.AddMonths (1);
    }
    this.DataBind ();
  }

</script>
<table>
  <tr>
    <td style="vertical-align:top;">
      <booking:OccupancyCalendar  year="<%# ((DateTime)dates[0]).Year %>" month="<%# ((DateTime)dates[0]).Month %>" 
                                  houseId="<%# houseId %>" imagePath="../images" runat="server"/>
    </td>
    <td style="vertical-align:top;">
      <booking:OccupancyCalendar  year="<%# ((DateTime)dates[1]).Year %>" month="<%# ((DateTime)dates[1]).Month %>" 
                                  houseId="<%# houseId %>" imagePath="../images" runat="server"/>
    </td>
    <td style="vertical-align:top;">
      <booking:OccupancyCalendar  year="<%# ((DateTime)dates[2]).Year %>" month="<%# ((DateTime)dates[2]).Month %>" 
                                  houseId="<%# houseId %>" imagePath="../images" runat="server"/>
    </td>
    <td style="vertical-align:top;">
      <booking:OccupancyCalendar  year="<%# ((DateTime)dates[3]).Year %>" month="<%# ((DateTime)dates[3]).Month %>" 
                                  houseId="<%# houseId %>" imagePath="../images" runat="server"/>
    </td>
    <td style="vertical-align:top;">
      <booking:OccupancyCalendar  year="<%# ((DateTime)dates[4]).Year %>" month="<%# ((DateTime)dates[4]).Month %>" 
                                  houseId="<%# houseId %>" imagePath="../images" runat="server"/>
    </td>
    <td style="vertical-align:top;">
      <booking:OccupancyCalendar  year="<%# ((DateTime)dates[5]).Year %>" month="<%# ((DateTime)dates[5]).Month %>" 
                                  houseId="<%# houseId %>" imagePath="../images" runat="server"/>
    </td>
  </tr>

  <tr>
    <td style="vertical-align:top;">
      <booking:OccupancyCalendar  year="<%# ((DateTime)dates[6]).Year %>" month="<%# ((DateTime)dates[6]).Month %>" 
                                  houseId="<%# houseId %>" imagePath="../images" runat="server"/>
    </td>
    <td style="vertical-align:top;">
      <booking:OccupancyCalendar  year="<%# ((DateTime)dates[7]).Year %>" month="<%# ((DateTime)dates[7]).Month %>" 
                                  houseId="<%# houseId %>" imagePath="../images" runat="server"/>
    </td>
    <td style="vertical-align:top;">
      <booking:OccupancyCalendar  year="<%# ((DateTime)dates[8]).Year %>" month="<%# ((DateTime)dates[8]).Month %>" 
                                  houseId="<%# houseId %>" imagePath="../images" runat="server"/>
    </td>
    <td style="vertical-align:top;">
      <booking:OccupancyCalendar  year="<%# ((DateTime)dates[9]).Year %>" month="<%# ((DateTime)dates[9]).Month %>" 
                                  houseId="<%# houseId %>" imagePath="../images" runat="server"/>
    </td>
    <td style="vertical-align:top;">
      <booking:OccupancyCalendar  year="<%# ((DateTime)dates[10]).Year %>" month="<%# ((DateTime)dates[10]).Month %>" 
                                  houseId="<%# houseId %>" imagePath="../images" runat="server"/>
    </td>
    <td style="vertical-align:top;">
      <booking:OccupancyCalendar  year="<%# ((DateTime)dates[11]).Year %>" month="<%# ((DateTime)dates[11]).Month %>" 
                                  houseId="<%# houseId %>" imagePath="../images" runat="server"/>
    </td>
  </tr>
</table>
