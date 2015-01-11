<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/custom/error.master" %>

<%@ Register TagPrefix="booking" Namespace="de.fiok.controls" %>

<script language="C#" runat="server">
  private void Page_Load(Object sender, EventArgs args)
  {
    DataBind();
  }
</script>

<asp:Content ID="Content1" ContentPlaceHolderID="error_msg" runat="Server">
  <booking:Message Key="Error_txt.timeout" Arg0="<%# Session.Timeout %>" runat="server" />
</asp:Content>


