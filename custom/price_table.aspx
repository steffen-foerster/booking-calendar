<%@ Page Language="c#" AutoEventWireup="true" MasterPageFile="~/custom/custom.master" %>

<%@ Register TagPrefix="price" Namespace="House.Controls" %>

<script runat="server">
  protected int houseId = 0;
  protected String url;

  protected void Page_Load (Object sender, EventArgs args)
  {
    try {
      url = ConfigurationManager.AppSettings["price_handler_url"];
      houseId = Int32.Parse (Request.Params["houseId"]);
    }
    catch (Exception e) {
      houseId = 0;      
    }
    this.DataBind ();
  }
</script>

<asp:Content ID="Content1" ContentPlaceHolderID="main" runat="Server">
  <price:PriceControl ID="PriceControl1" runat="server" Url="<%# url %>" HouseId="<%# houseId %>" />
</asp:Content>
