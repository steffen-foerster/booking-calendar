<%@ Page Language="C#" AutoEventWireup="true" CodeFile="calendar-test.aspx.cs" Inherits="aspx_calendar_calendar" %>
<%@ Register TagPrefix="booking" Namespace="de.fiok.controls" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title><booking:message key="Booking_lb.title.calendar" runat="server"/></title>
    <style type="text/css">
      a:link {
        color:#E07D20;
        text-decoration:none;
      }
      a:hover {
        color:#E07D20;
        text-decoration:none;
      }
      a:active {
        color:#E07D20;
        text-decoration:none;
      }
      a:visited {
        color:#E07D20;
        text-decoration:none;
      }
    </style>
</head>
<body style="margin:10px;background-color:<%# bgColor %>">
  <form id="calendar" runat="server">
    <div style="border:1px solid <%# borderColor %>;padding-top:10px;padding-bottom:5px;background-color:#FFFFFF">
      <center>
      <table style="border:0;margin:0" cellpadding="0" cellspacing="0">
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
            <booking:OccupancyCalendar ID="OccupancyCalendar1"  year="<%# ((DateTime)dates[4]).Year %>" month="<%# ((DateTime)dates[4]).Month %>" 
                                        houseId="<%# houseId %>" imagePath="../../images" runat="server"/>
          </td>
          <td style="vertical-align:top;">
            <booking:OccupancyCalendar ID="OccupancyCalendar2"  year="<%# ((DateTime)dates[5]).Year %>" month="<%# ((DateTime)dates[5]).Month %>" 
                                        houseId="<%# houseId %>" imagePath="../../images" runat="server"/>
          </td>
          <td style="vertical-align:top;">
            <booking:OccupancyCalendar ID="OccupancyCalendar3"  year="<%# ((DateTime)dates[6]).Year %>" month="<%# ((DateTime)dates[6]).Month %>" 
                                        houseId="<%# houseId %>" imagePath="../../images" runat="server"/>
          </td>
          <td style="vertical-align:top;">
            <booking:OccupancyCalendar ID="OccupancyCalendar4"  year="<%# ((DateTime)dates[7]).Year %>" month="<%# ((DateTime)dates[7]).Month %>" 
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
      <div style="margin-top:10px;margin-bottom:5px">
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
      </center>
    </div>
  </form>
</body>
</html>
