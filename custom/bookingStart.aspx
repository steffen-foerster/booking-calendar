<%@ Page Language="c#" MasterPageFile="~/custom/custom.master" AutoEventWireup="true" %>

<%@ import Namespace="de.fiok.controller" %>

<script runat="server">
  private void Page_Load(Object sender, EventArgs args)
  {
    NavProvider.Instance.StartNavigation(NavProvider.FLOW_BOOKING);
  }
</script>

<asp:Content ID="Content1" ContentPlaceHolderID="main" runat="Server">
  <span style="color:blue">Sie werden zur Onlinebuchung weitergeleitet ...</span>
</asp:Content>