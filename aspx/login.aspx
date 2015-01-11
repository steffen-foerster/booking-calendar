<%@ Page Language="C#" Inherits="de.fiok.form.LoginForm" MasterPageFile="~/custom/custom.master" %>

<%@ Register TagPrefix="booking" Namespace="de.fiok.controls" %>
<%@ Register TagPrefix="booking" TagName="required" Src="controls/requiredSymbol.ascx" %>
<%@ Import Namespace="de.fiok.core" %>
<asp:Content ID="Content1" ContentPlaceHolderID="main" runat="Server">
  <div style="width:250px;">
    <div class="info_dialog">
      <booking:Message Key="Login_txt.login" runat="server" />
      <p />
      <p />
      <asp:ValidationSummary ID="validationSummary" runat="server" DisplayMode="BulletList"
        CssClass="error_summary" HeaderText='<%# AppResources.GetMessage ("Globals_msg.errors.summary") %>'>
      </asp:ValidationSummary>
      <table cellspacing="0" cellpadding="5" border="0">
        <tr>
          <td class="label">
            <booking:Message Key="Login_lb.email" runat="server" />
            :</td>
          <td>
            <asp:TextBox ID="email" runat="server" CssClass="text" Columns="20" MaxLength="50"
              EnableViewState="False">
            </asp:TextBox>
            <booking:required runat="server" />
          </td>
        </tr>
        <tr>
          <td class="label">
            <booking:Message Key="Login_lb.password" runat="server" />
            :</td>
          <td>
            <asp:TextBox ID="password" runat="server" CssClass="text" Columns="20" MaxLength="20"
              EnableViewState="False" TextMode="password">
            </asp:TextBox>
            <booking:required runat="server" />
          </td>
        </tr>
      </table>
      <p />
      <%--Message --%>
      <booking:UIMessage ID="uiMsgLogin" runat="server" Style="text-align: left;" ImagePath="../images" />
      <div style="text-align: right;">
        <button runat="server" id="btnLogin" class="normal" causesvalidation="true">
          <booking:Message Key="Login_bt.login" runat="server" />
        </button>
      </div>
    </div>
    <%-- Validators --%>
    <%-- email --%>
    <booking:RequiredFieldValidator ControlToValidate="email" runat="server" ErrorMessage='<%# AppResources.GetMessage ("Validation_required.login.email")%>' />
    <booking:RegularExpressionValidator ControlToValidate="email" runat="server" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
      ErrorMessage='<%# AppResources.GetMessage ("Validation_invalid.email")%>' />
    <%-- password --%>
    <booking:RequiredFieldValidator ControlToValidate="password" runat="server" ErrorMessage='<%# AppResources.GetMessage ("Validation_required.password") %>' />
  </div>
</asp:Content>
