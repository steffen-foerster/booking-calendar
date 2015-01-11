<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/custom/error.master" %>

<%@ Register TagPrefix="booking" Namespace="de.fiok.controls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="error_msg" runat="Server">
  <booking:Message Key="Error_txt.file.not.found" runat="server" />
</asp:Content>
