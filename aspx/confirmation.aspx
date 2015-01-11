<%@ Page Inherits="de.fiok.form.ConfirmationForm" Language="C#" MasterPageFile="~/custom/custom.master" 
         EnableEventValidation="false" MaintainScrollPositionOnPostback="true"%>

<%@ Register TagPrefix="booking" TagName="required" Src="controls/requiredSymbol.ascx" %>
<%@ Register TagPrefix="booking" Namespace="de.fiok.controls" %>
<%@ Import Namespace="de.fiok.core" %>
<%@ Import Namespace="de.fiok.type" %>
<%@ Import Namespace="System.Configuration" %>
<asp:Content ID="Content1" ContentPlaceHolderID="main" runat="Server">

  <script type="text/javascript">
    function disableSendBtn ()
    {
      try {
        document.forms[0].btnNextPage.disabled = true;
      }
      catch (e) {            
      }
    }
  </script>

  <booking:TimeoutDetection ID="timeout" runat="server" />
  <div class="window_body" style="width: 100%;">
    <div class="window_header">
      <booking:Message Key="Confirmation_legend.confirmation" runat="server" />
    </div>
    <div class="window_text" style="text-align: center;">
      <div style="text-align: left; padding-right: 250px; margin-top: 10px;">
        <booking:Message Key="Confirmation_txt.confirm" runat="server" />
      </div>
      <div style="text-align: left; padding-top: 15px;">
        <table cellspacing="0" cellpadding="5" border="0">
          <%-- personal data --%>
          <tr>
            <td class="label_left" rowspan="3" style="vertical-align: top;">
              <booking:Message Key="Confirmation_lb.tenant.address" runat="server" />:
            </td>
            <td class="data" style="padding-top: 5px; padding-bottom: 0px;">
              <%# SalutationType.GetName (tenantData.Tenant.Salutation) %>
              &nbsp;
              <%# tenantData.Tenant.Title %>
              &nbsp;
              <%# tenantData.Tenant.Firstname %>
              &nbsp;
              <%# tenantData.Tenant.Name %>
              &nbsp;
            </td>
            <td rowspan="7" style="text-align: right; vertical-align: top;">
              <asp:Image ImageUrl="~/custom/images/ferienhaus.jpg" Style="vertical-align: top;
                width: 200px" runat="server" />
            </td>
          </tr>
          <tr>
            <td class="data" style="padding-top: 0px; padding-bottom: 0px;">
              <%# tenantData.Tenant.Street %>
            </td>
          </tr>
          <tr>
            <td class="data" style="padding-top: 0px; padding-bottom: 5px;">
              <%# tenantData.Tenant.Zipcode %>
              &nbsp;
              <%# tenantData.Tenant.Location %>
            </td>
          </tr>
          <tr>
            <td class="label_left">
              <booking:Message Key="TenantEntry_lb.telephone" runat="server" />:
            </td>
            <td class="data" style="padding-top: 0px; padding-bottom: 0px;">
              <%# tenantData.Tenant.Telephone %>
              &nbsp;&nbsp;
            </td>
          </tr>
          <tr>
            <td class="label_left">
              <booking:Message Key="TenantEntry_lb.fax" runat="server" />:
            </td>
            <td class="data" style="padding-top: 0px; padding-bottom: 0px;">
              <%# tenantData.Tenant.Fax %>
            </td>
          </tr>
          <tr>
            <td class="label_left">
              <booking:Message Key="TenantEntry_lb.mail" runat="server" />:
            </td>
            <td class="data" style="padding-top: 0px; padding-bottom: 0px;">
              <%# tenantData.Tenant.Email %>
            </td>
          </tr>
          <% if (bookingData.Price.House.BedClothesHirable) { %>
          <tr>
            <td class="label_left">
              <booking:Message Key="Confirmation_lb.bedclothes" runat="server" />:
            </td>
            <td class="input">
              <asp:CheckBox ID="bedClothes" runat="server" />
            </td>
          </tr>
          <% } %>
          <tr>
            <td class="label_left" style="padding-top: 40px;">
              <booking:Message Key="Confirmation_lb.notes" runat="server" />:
            </td>
          </tr>
          <tr>
            <td class="label_left" colspan="2">
              <asp:TextBox ID="notes" runat="server" Columns="40" Rows="5" TextMode="MultiLine"
                CssClass="text" />
            </td>
            <td style="padding-left: 30px; vertical-align: top;">
              <%-- period and price --%>
              <table cellspacing="0" cellpadding="5" border="0">
                <tr>
                  <td class="label">
                    <booking:Message Key="Confirmation_lb.booking.period" runat="server" />
                    :</td>
                  <td class="data" style="text-align: right; padding-left: 10px;">
                    <%# DataBinder.Eval (bookingData, "ArrivalDate", "{0:d}") %>
                    -
                    <%# DataBinder.Eval (bookingData, "DepartureDate", "{0:d}") %>
                  </td>
                </tr>
                <tr>
                  <td class="label" style="padding-left: 30px;">
                    <booking:Message Key="Booking_lb.total.cost" runat="server" />
                    :</td>
                  <td class="data" style="text-align: right; padding-left: 10px;">
                    <%# bookingData.Price.TotalCost  %>
                    EUR</td>
                </tr>
              </table>
            </td>
          </tr>
        </table>
      </div>
      <%--Message --%>
      <booking:UIMessage ID="uiMsgBooking" runat="server" Style="text-align: right; padding-top: 10px;"
        ImagePath="../images" />
      <div style="width: 100%; text-align: right; padding-top: 20px;">
        <button id="btnPreviousPage" runat="server" class="normal" causesvalidation="false"
          onmouseover="this.className='mouseover';" onmouseout="this.className='normal';">
          <img src="../images/arrow_left.png" class="middle" alt="back" />
          &nbsp;
          <%# AppResources.GetMessage("Globals_bt.previous")%>
        </button>
        &nbsp;&nbsp;
        <% if (System.Configuration.ConfigurationManager.AppSettings["booking_active"].Equals ("true")) { %>
        <button id="btnNextPage" runat="server" class="normal" onclick="window.setTimeout('disableSendBtn();',10);" onmouseover="this.className='mouseover';"
          onmouseout="this.className='normal';">
          <img src="../images/accept.png" class="middle" alt="reserve" />
          &nbsp;
          <%# AppResources.GetMessage("Confirmation_bt.send")%>
        </button>
        <% } %>
      </div>
    </div>
  </div>
</asp:Content>
