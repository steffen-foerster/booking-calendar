<%@ Page Language="c#" MasterPageFile="~/custom/custom.master" AutoEventWireup="true" %>

<%@ import Namespace="de.fiok.controller" %>

<script runat="server">
  private void Page_Load(Object sender, EventArgs args)
  {
    NavProvider.Instance.StartNavigation(NavProvider.FLOW_ADMIN);
  }
</script>

<asp:Content ID="Content1" ContentPlaceHolderID="main" runat="Server">
  <span style="color:blue">Sie werden zur Administrationsseite weitergeleitet ...</span>
</asp:Content>
