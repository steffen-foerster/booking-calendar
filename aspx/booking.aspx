<%@ Page Language="C#" Inherits="de.fiok.form.BookingForm" MasterPageFile="~/custom/custom.master" 
         EnableEventValidation="false" MaintainScrollPositionOnPostback="true"%>
<%@ Register TagPrefix="booking" TagName="calendar" Src="controls/occupancyCalendar.ascx" %>
<%@ Register TagPrefix="booking" Namespace="de.fiok.controls" %>
<%@ import Namespace="de.fiok.core" %>
<asp:Content ID="Content1" ContentPlaceHolderID="main" runat="Server">
  <booking:TimeoutDetection id="timeout" runat="server" />

  <div class="window_body" style="width:100%">
    <div class="window_header">
      <booking:message key="Booking_legend.choose" runat="server"/>
    </div>
    <div class="window_text" style="text-align:left">
      <%--
      <div style="text-align:justify;margin-top:5px;font-size:9pt;">
        <booking:message key="Booking_txt.booking" runat="server"/>
      </div>
      --%>

      <table cellspacing="0" cellpadding="0" border="0">
        <tr>
          <td style="padding:5px;padding-right:10px">
            <asp:Image runat="server" ImageUrl="~/images/number_1_orange.gif"/>
          </td>
          <td colspan="3">
            <booking:message key="Booking_msg.number.1" runat="server"/>:&nbsp;&nbsp;&nbsp;
            <asp:DropDownList ID="arrivalMonthList" runat="server" EnableViewState="true"
              AutoPostBack="true"/>
          </td>
        </tr>
        <tr>
          <td style="padding:5px;padding-right:10px">
            <asp:Image runat="server" ImageUrl="~/images/number_2_orange.gif"/>
          </td>
          <td colspan="3">
            <booking:message key="Booking_msg.number.2" runat="server"/>
          </td>
        </tr>
        <tr>
          <td></td>
          <%-- arrival and departure date --%>
          <td style="padding-top:15px;padding-bottom:8px;">
            <div class="label">
              <booking:message key="Booking_lb.arrival" runat="server"/>:&nbsp;
              <asp:Label id="arrivalDate" runat="server" cssclass="data_blue"></asp:Label>
            </div>
          </td>
          <td style="padding-top:15px;padding-bottom:8px;">
            <div class="label">
              <booking:message key="Booking_lb.departure" runat="server"/>:&nbsp;
              <asp:Label id="departureDate" runat="server" cssclass="data_blue"></asp:Label>
            </div>
          </td>
          <%-- price information --%>
          <td rowspan="2" style="vertical-align:top;padding-left:50px;padding-top:15px;">
            <table cellpadding="2" border="0" cellspacing="0" style="width:250px;">
              <tr>
                <td>
                  <div class="label">
                    <booking:message key="Booking_lb.count.days" runat="server"/>:
                  </div>
                </td>
                <td class="right" style="PADDING-LEFT: 10px">
                  <span class="data"><%= price.TotalDays %></span>
                </td>
              </tr>
              <tr>
                <td>
                  <div class="label">
                    <booking:message key="Booking_lb.rent" runat="server"/>:
                  </div>
                </td>
                <td class="right" style="PADDING-LEFT: 10px">
                  <span class="data"><%= price.Rent %>&nbsp;EUR</span>
                </td>
              </tr>
              <tr>
                <td>
                  <div class="label">
                    <booking:message key="Booking_lb.cleaning" runat="server"/>:
                  </div>
                </td>
                <td class="right" style="PADDING-LEFT: 10px">
                  <% if (price.CleaningSeason || price.CleaningDays) { %>
                    <span class="data" style="COLOR: green">0 EUR</span> 
                  <% } else { %>
                    <span class="data"><%= price.CleaningCost %>&nbsp;EUR</span>
                  <% } %>
                </td>
              </tr>
              <tr>
                <td style="BORDER-TOP: #2f2f2f 1px solid">
                  <div class="label">
                    <booking:message key="Booking_lb.total.cost" runat="server"/>:
                  </div>
                </td>
                <td class="right" style="BORDER-TOP: #2f2f2f 1px solid; PADDING-LEFT: 10px">
                  <span class="data" style="FONT-WEIGHT: bold"><%= price.TotalCost %>&nbsp;EUR</span>
                </td>
              </tr>
              <tr>
                <td colspan="3">
                  <div class="ui_info_msg">
                  <% if (price.CleaningSeason) { %>
                    <booking:message ID="Message1" key="Booking_msg.case.saison.days" arg0="<%# price.House.MinDaysSeason %>" runat="server"/>
                  <% } else if (price.CleaningDays) { %>
                    <booking:message ID="Message2" key="Booking_msg.case.count.days" arg0="<%# price.House.MinDaysTotal %>" runat="server"/>
                  <% } %>
                  </div>
                </td>
              </tr>
            </table>
          </td>
        </tr>
        <%-- arrival and departure calendars --%>
        <tr>
          <td></td>
          <td style="padding-right:15px;">
            <asp:Calendar id="arrivalCalendar" runat="server" />
          </td>
          <td style="padding-right:15px;">
            <asp:Calendar id="departureCalendar" runat="server" />
          </td>
        </tr>
        <% if (errorMessage.Text != "") { %>
        <tr>
          <td></td>
          <td colspan="3" style="padding:10px;">
            <asp:Label id="errorMessage" runat="server" enableviewstate="false" cssclass="data_error"></asp:Label>
          </td>
        </tr>
        <% } %>
        <tr>
          <td style="padding:5px;padding-right:10px">
            <asp:Image runat="server" ImageUrl="~/images/number_3_orange.gif"/>
          </td>
          <td colspan="2">
            <booking:message key="Booking_msg.number.3" runat="server"/>
          </td>
          <td style="text-align:right;">
            <button id="btnCancel" runat="server" class="normal"
                    onmouseover="this.className='mouseover';" onmouseout="this.className='normal';">
              <img src="../images/arrow_left.png" class="middle" alt="next" />
              &nbsp;
              <%# AppResources.GetMessage("Globals_bt.cancel")%>
    	      </button>
    	      &nbsp;&nbsp;&nbsp;    
            <button id="btnNextPage" runat="server" class="normal" disabled="disabled"
                    onmouseover="this.className='mouseover';" onmouseout="this.className='normal';">
              <img src="../images/arrow_right.png" class="middle" alt="next" />
              &nbsp;
              <%# AppResources.GetMessage("Globals_bt.next")%>
    	      </button>                                           
          </td>
        </tr>
      </table>
    </div>
  </div>

<p/>
        
  <div class="window_body" style="width:100%;">
    <div class="window_header">
      <booking:message key="Booking_legend.booking.calendar" runat="server"/>
    </div>
    <div class="window_text" style="text-align:center;padding-top:20px;">
      <center>
        <booking:calendar houseId="<%# CurrentHouseId %>" runat="server"/>
        <div style="margin-top:20px;">
          <span>
            <img src="../images/booked.gif" height="14" width="14" style="border:1px solid gray;vertical-align:top;" class="middle" alt="booked"/>
                 &nbsp;<booking:message key="Booking_lb.booked" runat="server"/>
          </span>
          <span style="margin-left:30px;">
            <img src="../images/arrival.gif" height="14" width="14" style="border:1px solid gray;vertical-align:top;" class="middle" alt="arrival"/>
                 &nbsp;<booking:message key="Booking_lb.arrival" runat="server"/>
          </span>
          <span style="margin-left:30px;">
            <img src="../images/departure.gif" height="14" width="14" style="border:1px solid gray;vertical-align:top;" class="middle" alt="departure"/>
                 &nbsp;<booking:message key="Booking_lb.departure" runat="server"/>
          </span>
        </div>
      </center>
    </div>
  </div>
</asp:Content>