<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/custom/error.master" %>

<%@ Register TagPrefix="booking" Namespace="de.fiok.controls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="error_msg" runat="Server">
  <booking:Message ID="Message1" Key="Error_txt.access.denied" runat="server" />
</asp:Content>