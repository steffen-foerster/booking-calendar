<%@ Page Inherits="de.fiok.form.TenantEntryForm" Language="C#" MasterPageFile="~/custom/custom.master" 
         EnableEventValidation="false" MaintainScrollPositionOnPostback="true"%>

<%@ Register TagPrefix="booking" TagName="required" Src="controls/requiredSymbol.ascx" %>
<%@ Register TagPrefix="booking" Namespace="de.fiok.controls" %>
<%@ Import Namespace="de.fiok.core" %>
<asp:Content ID="Content1" ContentPlaceHolderID="main" runat="Server">
  <booking:TimeoutDetection ID="timeout" runat="server" />
  <div class="window_body" style="width: 100%;">
    <div class="window_header">
      <booking:Message Key="TenantEntry_legend.tenant.data" runat="server" />
    </div>
    <div class="window_text" style="text-align: center;">
      <div style="text-align: left; padding-right: 250px; margin-top: 10px;">
        <booking:Message Key="TenantEntry_txt.entry" runat="server" />
      </div>
      <asp:ValidationSummary ID="validationSummary" runat="server" DisplayMode="BulletList"
        CssClass="error_summary" HeaderText='<%# AppResources.GetMessage ("Globals_msg.errors.summary") %>'>
      </asp:ValidationSummary>
      <div style="text-align: left; padding-top: 15px;">
        <table cellspacing="0" cellpadding="5" border="0">
          <%-- salutation, title --%>
          <tr>
            <td class="label">
              <booking:Message Key="TenantEntry_lb.salutation" runat="server" />
              :</td>
            <td>
              <asp:DropDownList ID="salutationSelect" runat="server" EnableViewState="False" />
            </td>
            <td class="label">
              <booking:Message Key="TenantEntry_lb.title" runat="server" />
              :</td>
            <td>
              <asp:TextBox ID="title" runat="server" CssClass="text" Columns="20" MaxLength="15"
                EnableViewState="False"></asp:TextBox>
            </td>
          </tr>
          <%-- firstname, name --%>
          <tr>
            <td class="label">
              <booking:Message Key="TenantEntry_lb.firstname" runat="server" />
              :</td>
            <td>
              <asp:TextBox ID="firstname" runat="server" CssClass="text" Columns="20" MaxLength="30"
                EnableViewState="False"></asp:TextBox>
              <booking:required runat="server" />
            </td>
            <td class="label">
              <booking:Message Key="TenantEntry_lb.name" runat="server" />
              :</td>
            <td>
              <asp:TextBox ID="name" runat="server" CssClass="text" Columns="20" MaxLength="20"
                EnableViewState="False"></asp:TextBox>
              <booking:required runat="server" />
            </td>
          </tr>
          <%-- street, zipcode, location --%>
          <tr>
            <td class="label">
              <booking:Message Key="TenantEntry_lb.street" runat="server" />
              :</td>
            <td>
              <asp:TextBox ID="street" runat="server" CssClass="text" Columns="20" MaxLength="30"
                EnableViewState="False"></asp:TextBox>
              <booking:required runat="server" />
            </td>
            <td class="label">
              <booking:Message Key="TenantEntry_lb.location" runat="server" />
              :</td>
            <td>
              <asp:TextBox ID="zipcode" runat="server" CssClass="text" Columns="5" MaxLength="5"
                EnableViewState="False"></asp:TextBox>
              <booking:required runat="server" />
              &nbsp;
              <asp:TextBox ID="location" runat="server" CssClass="text" Columns="20" MaxLength="25"
                EnableViewState="False"></asp:TextBox>
              <booking:required runat="server" />
            </td>
          </tr>
          <%-- email --%>
          <tr>
            <td class="label">
              <booking:Message Key="TenantEntry_lb.mail" runat="server" />
              :</td>
            <td>
              <asp:TextBox ID="email" runat="server" CssClass="text" Columns="20" MaxLength="50"
                EnableViewState="False"></asp:TextBox>
              <booking:required runat="server" />
            </td>
          </tr>
          <%-- telephone, fax --%>
          <tr>
            <td class="label">
              <booking:Message Key="TenantEntry_lb.telephone" runat="server" />
              :</td>
            <td>
              <asp:TextBox ID="telephone" runat="server" CssClass="text" Columns="20" MaxLength="20"
                EnableViewState="False"></asp:TextBox>
              <booking:required runat="server" />
            </td>
            <td class="label">
              <booking:Message Key="TenantEntry_lb.fax" runat="server" />
              :</td>
            <td>
              <asp:TextBox ID="fax" runat="server" CssClass="text" Columns="20" MaxLength="20"
                EnableViewState="False"></asp:TextBox>
            </td>
          </tr>
          <%-- count adult, count children --%>
          <tr>
            <td class="label">
              <booking:Message Key="TenantEntry_lb.count.adult" runat="server" />
              :</td>
            <td>
              <asp:DropDownList ID="adultCountSelect" runat="server" Width="40px">
                <asp:ListItem Value="1">1</asp:ListItem>
                <asp:ListItem Value="2" Selected="True">2</asp:ListItem>
                <asp:ListItem Value="3">3</asp:ListItem>
                <asp:ListItem Value="4">4</asp:ListItem>
              </asp:DropDownList>
            </td>
            <td class="label">
              <booking:Message Key="TenantEntry_lb.count.children" runat="server" />
              :</td>
            <td>
              <asp:DropDownList ID="childrenCountSelect" runat="server" Width="40px" AutoPostBack="true" EnableViewState="true">
                <asp:ListItem Value="0" Selected="True">0</asp:ListItem>
                <asp:ListItem Value="1">1</asp:ListItem>
                <asp:ListItem Value="2">2</asp:ListItem>
                <asp:ListItem Value="3">3</asp:ListItem>
                <asp:ListItem Value="4">4</asp:ListItem>
              </asp:DropDownList>
            </td>
          </tr>
          <tr>
            <td colspan="2"></td>
            <td class="label">
              Alter der Kinder:
            </td>
            <td>
              <asp:TextBox ID="ageChildren" runat="server" CssClass="text" Columns="20" MaxLength="255"
                EnableViewState="False"></asp:TextBox>
            </td>
          </tr>
          <%-- source --%>
          <tr>
            <td class="label">
              Bitte auswählen:
            </td>         
            <td colspan="2">
              <asp:DropDownList ID="promotionPartnerList" runat="server">
                <asp:ListItem Value="0" Selected="True">Wie haben Sie uns gefunden?</asp:ListItem>
                <asp:ListItem Value="1">Google</asp:ListItem>
                <asp:ListItem Value="4">andere Suchmaschine</asp:ListItem>
                <asp:ListItem Value="5">Link auf anderer Seite</asp:ListItem>
              </asp:DropDownList>
            </td>
          </tr>
        </table>
      </div>
      <div style="width: 100%; text-align: left; padding-top: 10px;">
        <booking:required runat="server" />
        &nbsp;<booking:Message Key="Globals_lb.mandatory.fields" runat="server" />
      </div>
      <%-- command button --%>
      <div style="width: 100%; text-align: right; padding-top: 30px;">
        <button id="btnPreviousPage" runat="server" style="width: 100px;" class="normal"
          causesvalidation="false" onmouseover="this.className='mouseover';" onmouseout="this.className='normal';">
          <img src="../images/arrow_left.png" class="middle" alt="previous" />
          &nbsp;
          <%# AppResources.GetMessage("Globals_bt.previous")%>
        </button>
        &nbsp;&nbsp;
        <button id="btnNextPage" runat="server" style="width: 100px;" class="normal" onmouseover="this.className='mouseover';"
          onmouseout="this.className='normal';">
          <img src="../images/arrow_right.png" class="middle" alt="next" />
          &nbsp;
          <%# AppResources.GetMessage("Globals_bt.next")%>
        </button>
      </div>
    </div>
  </div>
  <%-- Validators --%>
  <%-- name --%>
  <booking:RequiredFieldValidator ControlToValidate="name" runat="server" ErrorMessage='<%# AppResources.GetMessage ("Validation_required.name") %>' />
  <%-- firstname --%>
  <booking:RequiredFieldValidator ControlToValidate="firstname" runat="server" ErrorMessage='<%# AppResources.GetMessage ("Validation_required.firstname") %>' />
  <%-- street --%>
  <booking:RequiredFieldValidator ControlToValidate="street" runat="server" ErrorMessage='<%# AppResources.GetMessage ("Validation_required.street")%>' />
  <%-- zipcode --%>
  <booking:RequiredFieldValidator ControlToValidate="zipcode" runat="server" ErrorMessage='<%# AppResources.GetMessage ("Validation_required.zipcode")%>' />
  <booking:RegularExpressionValidator ControlToValidate="zipcode" runat="server" ValidationExpression="\d{5}"
    ErrorMessage='<%# AppResources.GetMessage ("Validation_invalid.zipcode")%>' ResetBorderColor="false"/>
  <%-- location --%>
  <booking:RequiredFieldValidator ControlToValidate="location" runat="server" ErrorMessage='<%# AppResources.GetMessage ("Validation_required.location")%>' />
  <%-- telephone --%>
  <booking:RequiredFieldValidator ControlToValidate="telephone" runat="server" ErrorMessage='<%# AppResources.GetMessage ("Validation_required.telephone")%>' />
  <%-- email --%>
  <booking:RequiredFieldValidator ControlToValidate="email" runat="server" ErrorMessage='<%# AppResources.GetMessage ("Validation_required.email")%>' />
  <booking:RegularExpressionValidator ControlToValidate="email" runat="server" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
    ErrorMessage='<%# AppResources.GetMessage ("Validation_invalid.email")%>'/>
  <%-- source --%>
  <booking:RangeValidator runat="server" MinimumValue="1" MaximumValue="5" ControlToValidate="promotionPartnerList" Type="Integer"
                      ErrorMessage="Bitte sagen Sie uns noch, wie Sie unsere Seite gefunden haben."></booking:RangeValidator>
  <%-- ageChildren --%>
  <booking:RequiredFieldValidator ID="ageChildrenValidator" ControlToValidate="ageChildren" runat="server" Enabled="false"
        ErrorMessage='Wir benötigen das Alter Ihrer Kinder für die Berechnung der Kurtaxe.' EnableViewState="true"/>
</asp:Content>
