<%@ Page Language="C#" Inherits="de.fiok.form.AdminBookingForm" MasterPageFile="~/custom/custom.admin.master" 
    MaintainScrollPositionOnPostback="true"%>

<%@ Register TagPrefix="booking" TagName="required" Src="../controls/requiredSymbol.ascx" %>
<%@ Register TagPrefix="booking" TagName="calendar" Src="../controls/editOccupancyCalendar.ascx" %>
<%@ Register TagPrefix="booking" Namespace="de.fiok.controls" %>
<%@ Import Namespace="de.fiok.core" %>
<%@ Import Namespace="de.fiok.service" %>
<%@ Import Namespace="de.fiok.type" %>
<%@ Import Namespace="de.fiok.web" %>

<asp:Content ID="Content1" ContentPlaceHolderID="main" runat="Server">
  <booking:TimeoutDetection ID="timeout" runat="server" />
  
  <div style="text-align:left;padding-bottom:5px;" class="data_blue">
    <booking:Message ID="Message1" Key="AdminBooking_lb.landlord.name" runat="server" />
    <%# house.Landlord.Firstname %> <%# house.Landlord.Name %>
  </div>
  
  <div class="window_body" style="width: 100%;">
  
<%-- ******************************************************************************************* --%>  
<%--   Suche von Buchungen                                                                       --%>
<%-- ******************************************************************************************* --%>
  
    <div class="window_header">
      <booking:Message Key="AdminBooking_legend.search" runat="server" />
    </div>
    <div class="window_text">
      <asp:ValidationSummary ID="searchValidationSummary" runat="server" DisplayMode="BulletList" 
        ValidationGroup="search" CssClass="error_summary"
        HeaderText='<%# AppResources.GetMessage ("Globals_msg.errors.summary") %>'>
      </asp:ValidationSummary>
      <div style="text-align: left; padding-top: 15px;">
        <table cellspacing="0" cellpadding="5" border="0">
          <%-- Haus wählen --%>
          <% if (LandlordRoleProvider.IsInRole(LandlordRole.ADMIN) || LandlordRoleProvider.IsInRole(LandlordRole.CALENDAR)) {%>
          <tr>
            <td colspan="2" class="label">
              <booking:Message Key="AdminBooking_lb.choose.house" runat="server" />:
            </td>
            <td colspan="3">
              <asp:DropDownList ID="houseSelect" runat="server" EnableViewState="true" CssClass="normal" />
            </td>
          </tr>
          <% } %>
          <%-- Zeitbereich auswählen --%>
          <tr>
            <td class="label">
              <booking:Message Key="AdminBooking_lb.select.period" runat="server" />
            </td>
            <td class="label">
              <booking:Message Key="Globals_lb.from" runat="server" />:
            </td>
            <td class="input">
              <asp:TextBox ID="dateFrom" runat="server" CssClass="text" Columns="10" MaxLength="10"
                EnableViewState="False">
              </asp:TextBox>
            </td>
            <td class="label">
              <booking:Message Key="Globals_lb.to" runat="server" />
              :</td>
            <td class="input">
              <asp:TextBox ID="dateTo" runat="server" CssClass="text" Columns="10" MaxLength="10"
                EnableViewState="False">
              </asp:TextBox>
            </td>
          </tr>
          <%-- Status --%>
          <tr>
            <td colspan="2" class="label" style="vertical-align: top;">
              <booking:Message Key="AdminBooking_lb.status" runat="server" />
              :
            </td>
            <td class="input">
              <asp:ListBox ID="statusSelect" runat="server" EnableViewState="true" CssClass="normal" />
            </td>
          </tr>
        </table>
      </div>
      <%--Message --%>
      <booking:UIMessage ID="uiMsgSearch" runat="server" Style="text-align: left;" ImagePath="../../images" />
      <%-- Suchen --%>
      <div style="width: 100%; text-align: left;margin-top:10px;">
        <button id="btnSearch" runat="server" class="normal" causesvalidation="false" onmouseover="this.className='mouseover';"
          onmouseout="this.className='normal';">
          <img src="../../images/search.png" alt="add" class="normal" />
          &nbsp;
          <%# AppResources.GetMessage ("Globals_bt.search") %>
        </button>
        &nbsp;&nbsp;
        <button id="btnCancel" runat="server" class="normal" causesvalidation="false" 
                onmouseover="this.className='mouseover';"
                onmouseout="this.className='normal';">
          <img src="../../images/cancel.png" alt="add" class="normal" />
          &nbsp;
          <%# AppResources.GetMessage("Globals_bt.reset")%>
        </button>
      </div>
    </div>
  </div>
  
  <%-- ValidationGroup 'search' --%>
  <booking:CompareValidator ID="search_date_from" ControlToValidate="dateFrom" runat="server"
    ErrorMessage='<%# AppResources.GetMessage ("Validation_invalid.date") %>'
    Type="Date" Operator="DataTypeCheck" ValidationGroup="search" />
  <booking:CompareValidator ID="search_date_to" ControlToValidate="dateTo" runat="server"
    ErrorMessage='<%# AppResources.GetMessage ("Validation_invalid.date") %>'
    Type="Date" Operator="DataTypeCheck" ValidationGroup="search" />
  
  <p />
  <p />
  
<%-- ******************************************************************************************* --%>  
<%--   Liste mit Buchungen - Buchung zur Bearbeitung                                             --%>
<%-- ******************************************************************************************* --%>  
  
  <div class="window_body" style="width: 100%">
    <div class="window_header">
      <booking:Message Key="AdminBooking_legend.edit.bookings" runat="server" />
    </div>
    <div class="window_text" style="text-align: center;">
      <div style="text-align: left; padding-top: 15px;">
        <div>
          <%-- Listen-Message --%>
          <div>
            <booking:UIMessage ID="uiMsgList" runat="server" Style="text-align: left;" ImagePath="../../images" />
          </div>      
        
          <%-- Linke Seite: Liste mit Buchungen, Rechte Seite: Buchung zur Bearbeitung --%>
          <table>
            <tr>
            
              <%-- ******************************************************************************************* --%>  
              <%--   Liste mit Buchungen                                                                       --%>
              <%-- ******************************************************************************************* --%>  

              <td style="vertical-align:top;text-align:left;padding-right:10px;">
                <%-- Buchung hinzufügen --%>
                <asp:Panel ID="addPriceItem" runat="server" Visible="true">
                  <div style="padding-bottom:10px;">
                    <asp:LinkButton CommandName="insert" runat="server" ID="addBookingButton" 
                                    CausesValidation="false" CssClass="linkbutton">
        	            <img src="../../images/add.png" alt="add" class="middle" />
        	            <%# AppResources.GetMessage("AdminBooking_lb.add.booking")%>
                    </asp:LinkButton>
                  </div>
                </asp:Panel>
                <asp:DataList ID="bookingList" runat="server" CssClass="datalist_table" ExtractTemplateRows="true">
                  <HeaderStyle CssClass="header_style" />
                  <ItemStyle CssClass="item_style" />
                  <HeaderTemplate>
                    <asp:Table runat="server">
                      <asp:TableRow>
                        <asp:TableCell cssClass="hcell_style">
                          <booking:Message Key="AdminBooking_lb.head.period" runat="server" />
                        </asp:TableCell>
                        <asp:TableCell cssClass="hcell_style">
                          <booking:Message Key="AdminBooking_lb.head.name" runat="server" />
                        </asp:TableCell>
                        <asp:TableCell cssClass="hcell_style">
                          <booking:Message Key="AdminBooking_lb.head.status" runat="server" />
                        </asp:TableCell>
                        <asp:TableCell cssClass="hcell_style">
                          <booking:Message Key="AdminBooking_lb.head.action" runat="server" />
                        </asp:TableCell>
                      </asp:TableRow>
                    </asp:Table>
                  </HeaderTemplate>
                  <ItemTemplate>
                    <asp:Table runat="server">
                      <asp:TableRow ID="itemRow">
                        <asp:TableCell cssClass="cell_style">
                          <%# Eval ("arrival", "{0:d}")%><br/>
                          <%# Eval ("departure", "{0:d}")%>
                        </asp:TableCell>
                        <asp:TableCell cssClass="cell_style" style="text-align:left;">
                          <%# SalutationType.GetName (((BookingItem)Container.DataItem).Tenant.Salutation) %>&nbsp;
                          <%# Eval ("tenant.title") %>&nbsp;<br />
                          <%# Eval ("tenant.firstname") %>&nbsp;
                          <%# Eval ("tenant.name") %>&nbsp;<br/>
                        </asp:TableCell>
                        <asp:TableCell cssClass="cell_style">
                          <%# BookingStatusType.GetName (((BookingItem)Container.DataItem).Status) %> 
                        </asp:TableCell>
                        <asp:TableCell cssClass="cell_style" style="text-align:left;">
                          <asp:LinkButton CommandName="edit" runat="server" ID="cmdEdit" CausesValidation="false"
                                          CssClass="linkbutton">
                            <img src="../../images/page_edit.png" alt="edit" class="middle"/>
                            <%# AppResources.GetMessage("AdminHouse_lb.title.edit")%>
                          </asp:LinkButton>
                          <br />    									  
                          <a href="bookingAsCSV.ashx?bookingId=<%# Eval ("id") %>" class="linkbutton">
                            <span style="white-space:nowrap">
                              <img src="../../images/page_excel.png" alt="exportCSV" class="middle"/>
                              <%# AppResources.GetMessage("AdminBooking_lb.title.export")%>
                            </span>
                          </a>			
                        </asp:TableCell>
                      </asp:TableRow>
                    </asp:Table>
                  </ItemTemplate>
                </asp:DataList>
              </td>
              
              <%-- ******************************************************************************************* --%>  
              <%--   Bearbeitung einer Buchung / Anzeige des Kalenders                                         --%>
              <%-- ******************************************************************************************* --%> 
              
              <td style="vertical-align:top;text-align:left;border-left:1px dashed #666666;padding-left:10px;padding-bottom:20px">
                <asp:MultiView ID="itemCalendarMultiView" ActiveViewIndex="0" runat="server">
                  <%-- *********** Bearbeitung einer Buchung *********** --%>
                  <asp:View ID="calendarView" runat="server">
                    <div style="padding:5px;">
                      <booking:message key="AdminBooking_lb.calendar" runat="server"/>
                    </div>
                    <booking:calendar ID="calendar" runat="server"/>
                  </asp:View>
                  <%-- *********** Bearbeitung einer Buchung *********** --%>
                  <asp:View ID="editView" runat="server">
                    <table style="border:0;margin:0" cellpadding="5" cellspacing="0">
                      <tr>
                        <% if (editItemMultiView.ActiveViewIndex == 0) { %>
                        <td style="background-color:#AAAAAA;color:White;font-weight:bold;padding:5px;">
                          <booking:Message Key="AdminBooking_btn.booking" runat="server" />
                        </td>
                        <% } else { %>
                        <td style="padding:5px;">
                          <asp:LinkButton runat="server" ID="tabBtnBooking" 
                                          CausesValidation="false" CssClass="linkbutton" OnClick="TabBtnBooking_OnClick">
        	                  <booking:Message Key="AdminBooking_btn.booking" runat="server" />  
                          </asp:LinkButton>
                        </td>
                        <% } %>
                        <% if (editItemMultiView.ActiveViewIndex == 1) { %>
                        <td style="background-color:#AAAAAA;color:White;font-weight:bold;padding:5px;">
                          <booking:Message Key="AdminBooking_btn.tenant" runat="server" />
                        </td>
                        <% } else { %>
                        <td style="padding:5px;">
                          <asp:LinkButton runat="server" ID="tabBtnTenant" 
                                          CausesValidation="false" CssClass="linkbutton" OnClick="TabBtnTenant_OnClick">
        	                  <booking:Message Key="AdminBooking_btn.tenant" runat="server" />  
                          </asp:LinkButton>
                        </td>
                        <% } %>
                        <% if (editItemMultiView.ActiveViewIndex == 2) { %>
                        <td style="background-color:#AAAAAA;color:White;font-weight:bold;padding:5px;">
                          <booking:Message Key="AdminBooking_btn.email" runat="server" />
                        </td>
                        <% } else { %>
                        <td style="padding:5px;">
                          <asp:LinkButton runat="server" ID="tabBtnEmail" 
                                          CausesValidation="false" CssClass="linkbutton" OnClick="TabBtnEmail_OnClick">
        	                  <booking:Message Key="AdminBooking_btn.email" runat="server" /> 
                          </asp:LinkButton>
                        </td>
                        <% } %>
                      </tr>
                    </table>
                    <div style="border-top:1px solid #AAAAAA;padding-top:15px;">
                    
                      <%-- Anzeige der Validierungsmeldungen --%>
                      <asp:ValidationSummary ID="editValidationSummary" runat="server" CssClass="error_summary"
                        DisplayMode="BulletList" ValidationGroup="edit"
                        HeaderText='<%# AppResources.GetMessage ("Globals_msg.errors.summary") %>'>
                      </asp:ValidationSummary>
                  
                      <asp:MultiView ID="editItemMultiView" ActiveViewIndex="0" runat="server">
                        <asp:View ID="viewBooking" runat="server">
                          <table cellspacing="0" cellpadding="5" border="0">
                            <%-- Buchungsdatum --%>
                            <tr>
                              <td class="label">Buchungsdatum:</td>
                              <td>
                                <asp:Label runat="server" ID="bookingDate" />
                              </td>
                            </tr>
                            <%-- Status --%>
                            <tr>
                              <td class="label"><booking:Message Key="AdminBooking_lb.status" runat="server" />:</td>
                              <td>
                                <asp:DropDownList ID="editStatusSelect" runat="server" CssClass="normal" />
                              </td>
                            </tr>
                            <%-- Anreise, Abreise --%>
                            <tr>
                              <td class="label"><booking:Message Key="AdminBooking_lb.period" runat="server" />:</td>
                              <td>
                                <asp:TextBox ID="arrival" runat="server" CssClass="text" Columns="12" MaxLength="10"/>
                                &nbsp;&nbsp;
                                <asp:TextBox ID="departure" runat="server" CssClass="text" Columns="12" MaxLength="10"/>
                              </td>
                            </tr>
                            <% if (LandlordRoleProvider.IsInRole(LandlordRole.ADMIN)) { %>
                            <%-- Mietpreis --%>
                            <tr>
                              <td class="label"><booking:Message Key="AdminBooking_lb.rent" runat="server" />:</td>
                              <td>
                                <asp:TextBox ID="rent" runat="server" CssClass="text" Columns="12" MaxLength="4"/>
                              </td>
                            </tr>
                            <%-- Endreinigung --%>
                            <tr>
                              <td class="label"><booking:Message Key="AdminBooking_lb.cleaningCost" runat="server" />:</td>
                              <td>
                                <asp:TextBox ID="cleaningCost" runat="server" CssClass="text" Columns="12" MaxLength="3"/>
                              </td>
                            </tr>
                            <%-- Preise berechnen --%>
                            <tr>
                              <td class="label"><booking:Message Key="AdminBooking_lb.calculate.price" runat="server" />:</td>
                              <td>
                                <asp:CheckBox ID="calculatePrice" runat="server" CssClass="normal" />
                              </td>
                            </tr>
                            <% } %>
                            <%-- Anzahl Erwachsene --%>
                            <tr>
                              <td class="label"><booking:Message Key="TenantEntry_lb.count.adult" runat="server" />:</td>
                              <td>
                                <asp:DropDownList ID="adultCountSelect" runat="server" Width="40px">
                                  <asp:ListItem Value="1">1</asp:ListItem>
                                  <asp:ListItem Value="2" Selected="True">2</asp:ListItem>
                                  <asp:ListItem Value="3">3</asp:ListItem>
                                  <asp:ListItem Value="4">4</asp:ListItem>
                                  <asp:ListItem Value="5">5</asp:ListItem>
                                  <asp:ListItem Value="6">6</asp:ListItem>
                                </asp:DropDownList>
                              </td>
                            </tr>
                            <%-- Anzahl Kinder --%>
                            <tr>
                              <td class="label"><booking:Message Key="TenantEntry_lb.count.children" runat="server" />:</td>
                              <td>
                                <asp:DropDownList ID="childrenCountSelect" runat="server" Width="40px">
                                  <asp:ListItem Value="0" Selected="True">0</asp:ListItem>
                                  <asp:ListItem Value="1">1</asp:ListItem>
                                  <asp:ListItem Value="2">2</asp:ListItem>
                                  <asp:ListItem Value="3">3</asp:ListItem>
                                  <asp:ListItem Value="4">4</asp:ListItem>
                                </asp:DropDownList>
                              </td>
                            </tr>
                            <%-- Alter der Kinder --%>
                            <tr>
                              <td class="label">Alter der Kinder:</td>
                              <td>
                                <asp:TextBox ID="ageChildren" runat="server" CssClass="normal" Columns="30" Rows="2" TextMode="MultiLine"/>
                              </td>
                            </tr>   

                            <% if (LandlordRoleProvider.IsInRole(LandlordRole.ADMIN)) { %>
                            <%-- Bettwäsche --%>
                            <tr>
                              <td class="label"><booking:Message Key="AdminBooking_lb.bedclothes" runat="server" />:</td>
                              <td>
                                <asp:CheckBox ID="bedClothes" runat="server" CssClass="normal" />
                              </td>
                            </tr>
                            <% } %>
                            <%-- Bemerkung --%>
                            <tr>
                              <td class="label"><booking:Message Key="AdminBooking_lb.notes" runat="server" />:</td>
                              <td>
                                <asp:TextBox ID="notes" runat="server" CssClass="normal" Columns="30" Rows="3" TextMode="MultiLine"/>
                              </td>
                            </tr>   
                            <% if (LandlordRoleProvider.IsInRole(LandlordRole.ADMIN)) { %>
                            <%-- Seite gefunden über: --%> 
                            <tr>
                              <td class="label">Seite gefunden:</td>
                              <td>
                                <asp:DropDownList ID="promotionPartnerList" runat="server">
                                  <asp:ListItem Value="0" Selected="True">keine Auswahl</asp:ListItem>
                                  <asp:ListItem Value="1">Google</asp:ListItem>
                                  <asp:ListItem Value="4">andere Suchmaschine</asp:ListItem>
                                  <asp:ListItem Value="5">Link auf anderer Seite</asp:ListItem>
                                </asp:DropDownList>
                              </td>
                            </tr>  
                            <% } %>                 
                          </table>
                          
                          <%-- Validationgroup 'edit' --%>
                          <%-- Anreisedatum --%>
                          <booking:RequiredFieldValidator ID="RequiredFieldValidatorArrival" ControlToValidate="arrival" runat="server" 
                            ErrorMessage='<%# AppResources.GetMessage ("Validation_required.arrival") %>' ValidationGroup="edit"/>
                          <booking:CompareValidator ID="CompareValidatorArrival" ControlToValidate="arrival" runat="server"
                            ErrorMessage='<%# AppResources.GetMessage ("Validation_invalid.date") %>' ResetBorderColor="false"
                            Type="Date" Operator="DataTypeCheck" ValidationGroup="edit" />
                          <%-- Abreisedatum --%>
                          <booking:RequiredFieldValidator ID="RequiredFieldValidatorDeparture" ControlToValidate="departure" runat="server" 
                            ErrorMessage='<%# AppResources.GetMessage ("Validation_required.departure") %>' ValidationGroup="edit"/>
                          <booking:CompareValidator ID="CompareValidatorDeparture" ControlToValidate="departure" runat="server"
                            ErrorMessage='<%# AppResources.GetMessage ("Validation_invalid.date") %>' ResetBorderColor="false"
                            Type="Date" Operator="DataTypeCheck" ValidationGroup="edit" />
                          <%-- Mietpreis --%>
                          <booking:RegularExpressionValidator ID="RequiredFieldValidatorRent" ControlToValidate="rent" runat="server"
                            ErrorMessage='<%# AppResources.GetMessage ("Validation_invalid.rent")%>' ValidationExpression="\d{1,4}"
                            ValidationGroup="edit" />
                          <%-- Endreinigung --%>
                          <booking:RegularExpressionValidator ID="RegularExpressioncleningCost" ControlToValidate="cleaningCost" runat="server"
                            ErrorMessage='<%# AppResources.GetMessage ("Validation_invalid.cleaning.cost")%>' ValidationExpression="\d{1,3}"
                            ValidationGroup="edit" />
                        </asp:View><%-- ID="viewBooking" --%>    
                        <%-- Seite 1 - Mieterdaten --%>
                        <asp:View ID="viewTenant" runat="server">
                          <table cellspacing="0" cellpadding="5" border="0">    
                            <%-- Anrede --%>
                            <tr>
                              <td class="label"><booking:Message Key="TenantEntry_lb.salutation" runat="server" />:</td>
                              <td>
                                <asp:DropDownList ID="salutationSelect" runat="server" CssClass="normal"/>
                              </td>
                            </tr>
                            <%-- Titel --%>
                            <tr>
                              <td class="label"><booking:Message Key="TenantEntry_lb.title" runat="server" />:</td>
                              <td>
                                <asp:TextBox ID="title" runat="server" CssClass="text" Columns="20" MaxLength="15" />
                              </td>
                            </tr>
                            <%-- Vorname --%>
                            <tr>
                              <td class="label"><booking:Message Key="TenantEntry_lb.firstname" runat="server" />:</td>
                              <td>
                                <asp:TextBox ID="firstname" runat="server" CssClass="text" Columns="20" MaxLength="30" />
                              </td>
                            </tr>
                            <%-- Name --%>
                            <tr>  
                              <td class="label"><booking:Message  Key="TenantEntry_lb.name" runat="server" />:</td>
                              <td>
                                <asp:TextBox ID="name" runat="server" CssClass="text" Columns="20" MaxLength="20" />
                              </td>
                            </tr>
                            <%-- Straße --%>
                            <tr>
                              <td class="label"><booking:Message Key="TenantEntry_lb.street" runat="server" />:</td>
                              <td>
                                <asp:TextBox ID="street" runat="server" CssClass="text" Columns="20" MaxLength="30" />
                              </td>
                            </tr>
                            <%-- PLZ, Ort --%>
                            <tr>
                              <td class="label"><booking:Message Key="TenantEntry_lb.location" runat="server" />:</td>
                              <td>
                                <asp:TextBox ID="zipcode" runat="server" CssClass="text" Columns="5" MaxLength="5" />
                                &nbsp;
                                <asp:TextBox ID="location" runat="server" CssClass="text" Columns="20" MaxLength="25" />
                              </td>
                            </tr>
                            <%-- E-Mail --%>
                            <tr>
                              <td class="label"><booking:Message Key="TenantEntry_lb.mail" runat="server" />:</td>
                              <td>
                                <asp:TextBox ID="email" runat="server" CssClass="text" Columns="20" MaxLength="50" />
                              </td>
                            </tr>
                            <%-- Telefon--%>
                            <tr>
                              <td class="label"><booking:Message Key="TenantEntry_lb.telephone" runat="server" />:</td>
                              <td>
                                <asp:TextBox ID="telephone" runat="server" CssClass="text" Columns="20" MaxLength="20" />
                              </td>
                            </tr>
                            <%-- FAX --%>
                            <tr>
                              <td class="label"><booking:Message Key="TenantEntry_lb.fax" runat="server" />:</td>
                              <td>
                                <asp:TextBox ID="fax" runat="server" CssClass="text" Columns="20" MaxLength="20" />
                              </td>
                            </tr>
                          </table>
                          <%-- ValidationGroup 'edit' --%>
                          <%-- name --%>
                          <booking:RequiredFieldValidator ID="RequiredFieldValidatorName" ControlToValidate="name" runat="server" 
                            ErrorMessage='<%# AppResources.GetMessage ("Validation_required.name.edit") %>' ValidationGroup="edit"/>
                          <%-- firstname --%>
                          <booking:RequiredFieldValidator ID="RequiredFieldValidatorFirstname" ControlToValidate="firstname" runat="server" 
                            ErrorMessage='<%# AppResources.GetMessage ("Validation_required.firstname.edit")  %>' ValidationGroup="edit" />
                          <%-- email --%>
                          <booking:RegularExpressionValidator ID="RegularExpressionValidatorEmail" ControlToValidate="email" runat="server" 
                            ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ValidationGroup="edit"
                            ErrorMessage='<%# AppResources.GetMessage ("Validation_invalid.email")%>' />
                          <%-- zipcode --%>
                          <booking:RegularExpressionValidator ID="RegularExpressionValidatorZipcode" ControlToValidate="zipcode" runat="server" 
                            ValidationExpression="\d{5}" ValidationGroup="edit"
                            ErrorMessage='<%# AppResources.GetMessage ("Validation_invalid.zipcode")%>' />
                        </asp:View><%-- ID="viewTenant" --%> 
                        <asp:View ID="viewEmail" runat="server">
                          <div style="padding:5px;">
                            <asp:HyperLink runat="server" ID="btnSendMail" CssClass="linkbutton">
          				            <img src="../../images/mail.png" alt="send mail" class="middle"/>
          				            <%# AppResources.GetMessage("AdminBooking_btn.send.mail")%>
          		              </asp:HyperLink>
          		            </div>
          		            <div style="padding:5px;">
          		              <asp:HyperLink runat="server" ID="btnSendPaid1Mail" CssClass="linkbutton">
          				            <img src="../../images/mail.png" alt="send mail" class="middle"/>
          				            <%# AppResources.GetMessage("AdminBooking_btn.send.paid1.mail")%>
          		              </asp:HyperLink>
          		            </div>
          		            <div style="padding:5px;">
          		              <asp:HyperLink runat="server" ID="btnSendPaid2Mail" CssClass="linkbutton">
          				            <img src="../../images/mail.png" alt="send mail" class="middle"/>
          				            <%# AppResources.GetMessage("AdminBooking_btn.send.paid2.mail")%>
          		              </asp:HyperLink>
          		            </div>
          		            <div style="padding:25px;" class="ui_info_msg">
                            <asp:Label ID="msgMail" runat="server" />
                          </div>
                        </asp:View><%-- ID="viewEmail" --%>
                      </asp:MultiView><%-- ID="editItemMultiView" --%>
                      
                      <%--Message --%>
                      <div>
                         <booking:UIMessage ID="uiMsgItem" runat="server" Style="text-align: left;" ImagePath="../../images" />
                      </div>
                      <%-- Speichern und Abbrechen Buttons --%>
                      <div style="padding-top:25px">
                        <asp:LinkButton runat="server" ID="btnApplyItem" CausesValidation="false" CssClass="linkbutton">
          			          <img src="../../images/disk.png" alt="update" class="middle"/>
          				        <%# AppResources.GetMessage("Globals_bt.save")%>
          		          </asp:LinkButton>
          		          &nbsp;&nbsp;
                        <asp:LinkButton runat="server" ID="btnCancelItem" CausesValidation="false" CssClass="linkbutton">
          				        <img src="../../images/cancel.png" alt="cancel" class="middle"/>
          				        <%# AppResources.GetMessage("AdminHouse_lb.title.cancel")%>
          		          </asp:LinkButton>
                      </div>
                    </div>
                  </asp:View> <%-- ID="editView" --%>
                </asp:MultiView>
              </td>
            </tr>
          </table>
        </div>
      </div>      
    </div>
  </div>
</asp:Content>
