<%@ Page Inherits="de.fiok.form.AdminHouseForm" Language="C#" AutoEventWireup="true" 
         MasterPageFile="~/custom/custom.admin.master" %>

<%@ Register TagPrefix="booking" TagName="required" Src="../controls/requiredSymbol.ascx" %>
<%@ Register TagPrefix="booking" Namespace="de.fiok.controls" %>
<%@ Import Namespace="de.fiok.service" %>
<%@ Import Namespace="de.fiok.core" %>
<%@ Import Namespace="de.fiok.type" %>
<%@ Import Namespace="log4net" %>

<script runat="server" language="C#">
  public void ArrivalDaysList_Bound (Object sender, EventArgs e) 
  {
    ILog log = LogManager.GetLogger("adminHouse.aspx");

    CheckBoxList list = (CheckBoxList)sender;
    foreach (ListItem item in list.Items)
    {
      String[] tokens = item.Value.Split('_');
      if (Boolean.Parse (tokens[0]))
      {
        item.Selected = true;
      }
    }
  }

  public void ArrivalDaysDataSource_Creating (Object sender, ObjectDataSourceEventArgs e)
  {
    ILog log = LogManager.GetLogger("adminHouse.aspx");

    e.ObjectInstance = this.editItem.arrivalDays;
  }

  public void DepartureDaysList_Bound(Object sender, EventArgs e)
  {
    ILog log = LogManager.GetLogger("adminHouse.aspx");

    CheckBoxList list = (CheckBoxList)sender;
    foreach (ListItem item in list.Items)
    {
      String[] tokens = item.Value.Split('_');
      if (Boolean.Parse(tokens[0]))
      {
        item.Selected = true;
      }
    }
  }

  public void DepartureDaysDataSource_Creating(Object sender, ObjectDataSourceEventArgs e)
  {
    ILog log = LogManager.GetLogger("adminHouse.aspx");

    e.ObjectInstance = this.editItem.departureDays;
  }
</script>

<asp:Content ID="Content1" runat="Server" ContentPlaceHolderID="main">
  <booking:TimeoutDetection ID="timeout" runat="server" />
  <div class="window_body" style="width: 100%;">
    <div class="window_header">
      <booking:Message Key="AdminHouse_legend.edit.data" runat="server" />
    </div>
    <div class="window_text" style="text-align: center;">
      <asp:ValidationSummary ID="validationSummary" runat="server" DisplayMode="BulletList"
        CssClass="error_summary" HeaderText='<%# AppResources.GetMessage ("Globals_msg.errors.summary") %>'>
      </asp:ValidationSummary>
      <div style="text-align: left; padding-top: 15px;">
        <table cellspacing="0" cellpadding="5" border="0">
          <%-- house selection --%>
          <tr>
            <td class="label">
              <booking:Message Key="AdminHouse_lb.choose.house" runat="server" />:
            </td>
            <td>
              <asp:DropDownList ID="houseSelect" runat="server" EnableViewState="true" />
            </td>
          </tr>
          <%-- location, cleaningCost, bedclothes --%>
          <tr>
            <td class="label">
              <booking:Message Key="AdminHouse_lb.location" runat="server" />:
            </td>
            <td class="input">
              <asp:TextBox ID="location" runat="server" CssClass="text" Columns="20" MaxLength="25"
                EnableViewState="False"></asp:TextBox>
              <booking:required runat="server" />
            </td>
            <td class="label">
              <booking:Message Key="AdminHouse_lb.cleaning.cost" runat="server" />:
            </td>
            <td class="input">
              <asp:TextBox ID="cleaningCost" runat="server" Columns="5" CssClass="text" EnableViewState="False"
                MaxLength="3"></asp:TextBox>
              <booking:required runat="server" />
            </td>
            <td class="label">
              <booking:Message Key="AdminHouse_lb.bedclothes" runat="server" />:
            </td>
            <td class="input">
              <asp:CheckBox ID="bedClothes" runat="server" />  
            </td>
          </tr>
        </table>
        <div class="label_left" 
             style="white-space: normal; font-weight: normal; padding-bottom: 15px; padding-top: 15px;">
          <booking:Message ID="Message1" Key="AdminHouse_lb.no.cleaning.cost" runat="server" />:
        </div>  
        <table cellspacing="0" cellpadding="5" border="0">
          <%-- minDaysSeason, minDaysTotal --%>
          <tr>
            <td class="label">
              <booking:Message Key="AdminHouse_lb.min.days.season" runat="server" />:
            </td>
            <td class="input">
              <asp:TextBox ID="minDaysSeason" runat="server" CssClass="text" Columns="5" MaxLength="2"
                EnableViewState="False"></asp:TextBox>
              <booking:required runat="server" />
            </td>
            <td class="label">
              <booking:Message Key="AdminHouse_lb.min.days.total" runat="server" />:
            </td>
            <td class="input">
              <asp:TextBox ID="minDaysTotal" runat="server" CssClass="text" Columns="5" MaxLength="2"
                EnableViewState="False"></asp:TextBox>
              <booking:required runat="server" />
            </td>
          </tr>
        </table>
      </div>
    </div>
  </div>
  <div class="window_body" style="margin-top: 30px;width:100%">
    <div class="window_header">
      <booking:Message Key="AdminHouse_legend.edit.prices" runat="server" />
    </div>
    <div class="window_text" style="text-align: center;">
      <div style="text-align: left; padding-top: 15px;">
        <%-- year selection --%>
        <div class="label" style="padding-bottom: 20px;">
          <booking:Message Key="AdminHouse_lb.choose.year" runat="server" />:
          &nbsp;&nbsp;
          <asp:DropDownList ID="yearSelect" runat="server" EnableViewState="true" />
        </div>
        <booking:DataListValidationSummary ID="editValidationSummary" CssClass="error_summary"
          runat="server" HeaderText='<%# AppResources.GetMessage ("Globals_msg.errors.summary") %>' />
        <div>
          <%-- Warning --%>
          <asp:Panel ID="intervalWarning" runat="server" Visible="false" EnableViewState="false">
            <div style="padding-bottom: 15px;">
              <img src="../../images/warning.png" alt="warn" class="normal" />
              <span class="warn_msg">
                <%# AppResources.GetMessage("Validation_not.all.days.contained", yearSelect.SelectedValue)%>
              </span>
            </div>
          </asp:Panel>
          <asp:DataList ID="priceList" runat="server" CssClass="datalist_table">
            <HeaderStyle CssClass="header_style" />
            <ItemStyle CssClass="item_style" />
            <AlternatingItemStyle CssClass="alternating_item_style" />
            <HeaderTemplate>
              <table cellpadding="0" cellspacing="0" border="0" class="item_table" width="100%">
                <tr>
                  <td class="hcell_style" style="width: 75px;">
                    <booking:Message Key="AdminHouse_lb.head.date.from" runat="server" />
                  </td>
                  <td class="hcell_style" style="width: 75px;">
                    <booking:Message Key="AdminHouse_lb.head.date.to" runat="server" />
                  </td>
                  <td class="hcell_style" style="width: 60px;">
                    <booking:Message Key="AdminHouse_lb.head.price" runat="server" />
                  </td>
                  <td class="hcell_style" style="width: 50px;">
                    <booking:Message Key="AdminHouse_lb.head.peak.season" runat="server" />
                  </td>
                  <td class="hcell_style" style="width: 170px;">
                    <booking:Message Key="AdminHouse_lb.head.arrival.days" runat="server" />
                  </td>
                  <td class="hcell_style" style="width: 170px;">
                    <booking:Message Key="AdminHouse_lb.head.departure.days" runat="server" />
                  </td>
                  <td class="hcell_style" style="width: 80px;">
                    <booking:Message Key="AdminHouse_lb.head.min.booking.days" runat="server" />
                  </td>
                  <td class="hcell_style">
                    <booking:Message Key="AdminHouse_lb.head.action" runat="server" />
                  </td>
                </tr>
              </table>
            </HeaderTemplate>
            <ItemTemplate>
              <table cellpadding="0" cellspacing="0" border="0" class="item_table" width="100%">
                <tr>
                  <td class="cell_style" style="width: 75px;">
                    <%# Eval ("start", "{0:d}")%>
                  </td>
                  <td class="cell_style" style="width: 75px;">
                    <%# Eval ("end", "{0:d}")%>
                  </td>
                  <td class="cell_style" style="width: 60px;">
                    <%# Eval ("price")%> EUR
                  </td>
                  <td class="cell_style" style="width: 50px;">
                    <%# AppResources.GetMessage("Globals_boolean." + ((HousePriceInterval)Container.DataItem).PeakSeason)%>
                  </td>
                  <td class="cell_style" style="width: 170px;">
                    <%# Eval("arrivalDays.BuildDayListString") %>
                  </td>
                  <td class="cell_style" style="width: 170px;">
                    <%# Eval("departureDays.BuildDayListString") %>
                  </td>
                  <td class="cell_style" style="width: 80px;">
                    <%# AppResources.GetMessage("AdminHouse_lb.min.booking.days", ((HousePriceInterval)Container.DataItem).MinBookingDays)%>
                  </td>
                  <td class="cell_style" style="text-align:left">
                    <asp:LinkButton CommandName="edit" runat="server" ID="cmdEdit" CausesValidation="false"
                      CssClass="linkbutton">
                        									  <img src="../../images/page_edit.png" alt="edit" class="middle"/>
                        									  <%# AppResources.GetMessage("AdminHouse_lb.title.edit")%></asp:LinkButton>
                    <br /><div style="margin-top:2px" />
                    <asp:LinkButton CommandName="delete" runat="server" ID="cmdDelete" CausesValidation="false"
                      CssClass="linkbutton">
                        									  <img src="../../images/page_delete.png" alt="delete" class="middle"/>
                        									  <%# AppResources.GetMessage("AdminHouse_lb.title.delete")%></asp:LinkButton>
                  </td>
                </tr>
              </table>
            </ItemTemplate>
            <EditItemTemplate>
              <table cellpadding="0" cellspacing="0" border="0" class="item_table" width="100%">
                <tr>
                  <%-- 'Datum von' editieren --%>
                  <td class="cell_style" style="width: 75px;">
                    <asp:TextBox ID="editStart" Columns="8" MaxLength="10" CssClass="text_small" value='<%# editItem.start %>'
                      Style='<%# editValidationSummary.GetErrorStyle ("editStart")%>' runat="server" />
                    <booking:RequiredFieldValidator ControlToValidate="editStart" runat="server" Enabled="false"
                      ErrorMessage='<%# AppResources.GetMessage ("Validation_required.start.date") %>' />
                    <booking:CompareValidator ControlToValidate="editStart" runat="server" Enabled="false"
                      ErrorMessage='<%# AppResources.GetMessage ("Validation_invalid.date") %>' Type="Date"
                      Operator="DataTypeCheck" />
                    <booking:CompareValidator ControlToValidate="editStart" ControlToCompare="editEnd"
                      runat="server" Enabled="false" Type="Date" ErrorMessage='<%# AppResources.GetMessage ("Validation_invalid.start.date.less") %>'
                      Operator="LessThan" />
                  </td>
                  <%-- 'Datum bis' editieren --%>
                  <td class="cell_style" style="width: 75px;">
                    <asp:TextBox ID="editEnd" Columns="8" MaxLength="10" CssClass="text_small" value='<%# editItem.end %>'
                      Style='<%# editValidationSummary.GetErrorStyle ("editEnd")%>' runat="server" />
                    <booking:RequiredFieldValidator ControlToValidate="editEnd" runat="server" Enabled="false"
                      ErrorMessage='<%# AppResources.GetMessage ("Validation_required.end.date") %>' />
                    <booking:CompareValidator ControlToValidate="editEnd" runat="server" Enabled="false"
                      ErrorMessage='<%# AppResources.GetMessage ("Validation_invalid.date") %>' Type="Date"
                      Operator="DataTypeCheck" />
                  </td>
                  <%-- 'Preis' editieren --%>
                  <td class="cell_style" style="width: 60px;">
                    <asp:TextBox ID="editPrice" Columns="4" MaxLength="3" CssClass="text_small" value='<%# editItem.price %>'
                      Style='<%# editValidationSummary.GetErrorStyle ("editPrice")%>' runat="server" />
                    <booking:RequiredFieldValidator ControlToValidate="editPrice" runat="server" Enabled="false"
                      ErrorMessage='<%# AppResources.GetMessage ("Validation_required.interval.price") %>' />
                    <booking:CompareValidator ControlToValidate="editPrice" runat="server" Enabled="false"
                      ErrorMessage='<%# AppResources.GetMessage ("Validation_invalid.integer") %>' Type="Integer"
                      Operator="DataTypeCheck" />
                    <booking:RangeValidator runat="server" ControlToValidate="editPrice" MinimumValue="1" MaximumValue="9999999" Enabled="false"
                      ErrorMessage='<%# AppResources.GetMessage ("Validation_min.interval.price")%>' Type="Integer" />
                  </td>
                  <%-- 'Hochsaison' editieren --%>
                  <td class="cell_style" style="width: 50px;">
                    <asp:CheckBox ID="editPeakSeason" runat="server" Checked='<%# editItem.peakSeason %>' />
                  </td>
                  <%-- 'Anreisetage' editieren --%>
                  <td class="cell_style" style="width: 170px;">
                    <asp:ObjectDataSource ID="arrivalDaysDataSource" TypeName="de.fiok.service.BookingDays" 
                                          SelectMethod="GetItems" OnObjectCreated="ArrivalDaysDataSource_Creating" runat="server">
                    </asp:ObjectDataSource>
                    <asp:CheckBoxList ID="arrivalDaysList" DataTextField="Name" DataValueField="Value" 
                                      runat="server" DataSourceID="arrivalDaysDataSource" TextAlign="Left" 
                                      RepeatLayout="Table" RepeatDirection="Horizontal" OnDataBound="ArrivalDaysList_Bound">
                    </asp:CheckBoxList>
                  </td>
                  <%-- 'Abreisetage' editieren --%>
                  <td class="cell_style" style="width: 170px;">
                    <asp:ObjectDataSource ID="departureDaysDataSource" TypeName="de.fiok.service.BookingDays" 
                      SelectMethod="GetItems" OnObjectCreated="DepartureDaysDataSource_Creating" runat="server">
                    </asp:ObjectDataSource>
                    <asp:CheckBoxList ID="departureDaysList" DataTextField="Name" DataValueField="Value" 
                      runat="server" DataSourceID="departureDaysDataSource" TextAlign="Left" 
                      RepeatLayout="Table" RepeatDirection="Horizontal" OnDataBound="DepartureDaysList_Bound">
                    </asp:CheckBoxList>
                  </td>
                  <%-- 'Min. Aufenthalt' editieren --%>
                  <td class="cell_style" style="width: 80px;">
                    <asp:TextBox ID="editMinBookingDays" Columns="4" MaxLength="2" CssClass="text" value='<%# editItem.minBookingDays %>'
                      Style='<%# editValidationSummary.GetErrorStyle ("editMinBookingDays")%>' runat="server" />
                    <booking:RequiredFieldValidator ControlToValidate="editMinBookingDays" runat="server" Enabled="false"
                      ErrorMessage='<%# AppResources.GetMessage ("Validation_required.min.booking.days") %>' />
                    <booking:CompareValidator ControlToValidate="editMinBookingDays" runat="server" Enabled="false"
                      ErrorMessage='<%# AppResources.GetMessage ("Validation_invalid.integer") %>' Type="Integer"
                      Operator="DataTypeCheck" />
                    <booking:RangeValidator runat="server" ControlToValidate="editMinBookingDays" MinimumValue="1" MaximumValue="9999999" Enabled="false"
                      ErrorMessage='<%# AppResources.GetMessage ("Validation_min.booking.days")%>' Type="Integer"/>
                  </td>
                  <td class="cell_style" style="text-align:left;">
                    <asp:LinkButton CommandName="update" runat="server" ID="cmdUpdate" CausesValidation="false"
                      CssClass="linkbutton">
                  							            <img src="../../images/disk.png" alt="update" class="middle"/>
                  							            <%# AppResources.GetMessage("AdminHouse_lb.title.update")%></asp:LinkButton>
                    <br /><div style="margin-top:2px" />
                    <asp:LinkButton CommandName="cancel" runat="server" ID="cmdCancel" CausesValidation="false"
                      CssClass="linkbutton">
                  								          <img src="../../images/cancel.png" alt="cancel" class="middle"/>
                  								          <%# AppResources.GetMessage("AdminHouse_lb.title.cancel")%></asp:LinkButton>
                  </td>
                </tr>
              </table>
            </EditItemTemplate>
          </asp:DataList>
          <%--Preis hinzufügen --%>
          <asp:Panel ID="addPriceItem" runat="server" Visible="false" EnableViewState="false">
            <div style="padding-top: 15px;">
              <asp:LinkButton CommandName="insert" runat="server" ID="addPriceButton" CausesValidation="false"
                CssClass="linkbutton">
        	                          <img src="../../images/add.png" alt="add" class="middle" />
        	                          <%# AppResources.GetMessage("AdminHouse_lb.add.price")%>
              </asp:LinkButton>
            </div>
          </asp:Panel>
        </div>
      </div>
    </div>
  </div>
  <%--Message --%>
  <booking:UIMessage ID="uiMsgSave" runat="server" style="text-align: left;" imagePath="../../images" />
  <%-- Abbrechen / Hausdaten speichern --%>
  <div style="text-align: left; padding-top: 20px;">
    <button id="btnSaveHouseData" runat="server" class="normal" causesvalidation="false"
      onmouseover="this.className='mouseover';" onmouseout="this.className='normal';">
      <img src="../../images/disk.png" alt="add" class="middle" />
      &nbsp;
      <%# AppResources.GetMessage("Globals_bt.save")%>
    </button>
    &nbsp;&nbsp;
    <button id="btnCancel" runat="server" class="normal" causesvalidation="false" onmouseover="this.className='mouseover';"
      onmouseout="this.className='normal';">
      <img src="../../images/cancel.png" alt="add" class="middle" />
      &nbsp;
      <%# AppResources.GetMessage("Globals_bt.reset")%>
    </button>
  </div>
  <%-- Validators --%>
  <%-- Validators sind disabled und werden erst explizit beim Speichern aktiviert--%>
  <%-- location --%>
  <booking:RequiredFieldValidator ControlToValidate="location" runat="server" Enabled="false"
    ID="house_req_location" ErrorMessage='<%# AppResources.GetMessage ("Validation_required.house.location") %>' />
  <%-- cleaning cost --%>
  <booking:RequiredFieldValidator ControlToValidate="cleaningCost" runat="server" Enabled="false"
    ID="house_req_cost" ErrorMessage='<%# AppResources.GetMessage ("Validation_required.cleaning.cost") %>' />
  <booking:RegularExpressionValidator ControlToValidate="cleaningCost" runat="server"
    Enabled="false" ID="house_reg_cost" ValidationExpression="\d{1,3}" ErrorMessage='<%# AppResources.GetMessage ("Validation_invalid.cleaning.cost")%>' />
  <%-- min days season --%>
  <booking:RequiredFieldValidator ControlToValidate="minDaysSeason" runat="server"
    Enabled="false" ID="house_req_season" ErrorMessage='<%# AppResources.GetMessage ("Validation_required.min.days.season")%>' />
  <booking:RegularExpressionValidator ControlToValidate="minDaysSeason" runat="server"
    Enabled="false" ID="house_reg_season" ValidationExpression="\d{1,2}" ErrorMessage='<%# AppResources.GetMessage ("Validation_invalid.min.days")%>' />
  <%-- min days total --%>
  <booking:RequiredFieldValidator ControlToValidate="minDaysTotal" runat="server" Enabled="false"
    ID="house_req_total" ErrorMessage='<%# AppResources.GetMessage ("Validation_required.min.days.total")%>' />
  <booking:RegularExpressionValidator ControlToValidate="minDaysTotal" runat="server"
    Enabled="false" ID="house_reg_total" ValidationExpression="\d{1,2}" ErrorMessage='<%# AppResources.GetMessage ("Validation_invalid.min.days")%>' />
</asp:Content>
