<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/custom/custom.master" EnableEventValidation="false"%>

<script language="C#" runat="server">
  private void Page_Load(Object sender, EventArgs args)
  {
    Response.Redirect(System.Configuration.ConfigurationManager.AppSettings["start_page"]);
  }
</script>
