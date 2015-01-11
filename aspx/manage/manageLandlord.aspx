<%@ Page Language="C#" MasterPageFile="~/aspx/manage/calendar.admin.master" AutoEventWireup="true" 
         CodeFile="manageLandlord.aspx.cs" Inherits="aspx_manage_manageLandlord" %>
<%@ Import Namespace="de.fiok.type" %>    
     
<asp:Content ID="Content1" ContentPlaceHolderID="main" Runat="Server">
  <div class="window_body">
    <div class="window_header">
      Administration Vermieter 
    </div>
    <div class="window_text">

      <%-- DataSources für Vermieter --%>
      <asp:ObjectDataSource ID="landlordList" runat="server" TypeName="de.fiok.manage.LandlordDS"
        DataObjectTypeName="de.fiok.service.LandlordBean" SelectMethod="GetLandlords"  />
      
      <asp:ObjectDataSource ID="landlordInsert" runat="server" 
        DataObjectTypeName="de.fiok.service.LandlordBean" TypeName="de.fiok.manage.LandlordDS" 
        InsertMethod="InsertLandlord" SelectMethod="GetLandlord" UpdateMethod="UpdateLandlord" DeleteMethod="DeleteLandlord">
        <SelectParameters>
          <asp:ControlParameter ControlID="GridViewLandlords" Name="ID" 
                                PropertyName="SelectedValue" Type="Int32" />
        </SelectParameters>
      </asp:ObjectDataSource>  
      
      <%-- DataSources für Objekte --%>
      <asp:ObjectDataSource ID="houseList" runat="server" TypeName="de.fiok.manage.HouseDS"
        DataObjectTypeName="de.fiok.service.HouseBean" SelectMethod="GetHouses">
        <SelectParameters>
          <asp:ControlParameter ControlID="GridViewLandlords" Name="ID" 
                                PropertyName="SelectedValue" Type="Int32" />
        </SelectParameters>
      </asp:ObjectDataSource>
      
      <asp:ObjectDataSource ID="houseInsert" runat="server" 
        DataObjectTypeName="de.fiok.service.HouseBean" TypeName="de.fiok.manage.HouseDS" 
        SelectMethod="GetHouse" UpdateMethod="UpdateHouse" InsertMethod="InsertHouse" DeleteMethod="DeleteHouse">
        <SelectParameters>
          <asp:ControlParameter ControlID="GridViewHouses" Name="ID" 
                                PropertyName="SelectedValue" Type="Int32" />
        </SelectParameters>
      </asp:ObjectDataSource>  
     
        
      
      <%-- Tabelle mit Vermietern --%>  
      <div style="text-align:left;">
        <asp:GridView ID="GridViewLandlords" runat="server" AutoGenerateColumns="False" CellPadding="4"
          DataSourceID="landlordList" ForeColor="#333333" GridLines="Both" DataKeyNames="ID">
          <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
          <Columns>
            <asp:BoundField DataField="ID" HeaderText="ID" SortExpression="ID" ReadOnly="True"/>
            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
            <asp:BoundField DataField="Firstname" HeaderText="Vorname" SortExpression="Firstname" />
            <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" />
            <asp:BoundField DataField="Domain" HeaderText="Domain" SortExpression="Domain" />
            <asp:TemplateField HeaderText="Rolle" SortExpression="Role">
              <ItemTemplate>
                <asp:Label ID="roleLabel" runat="server" Text='<%# Bind("Role") %>'></asp:Label>
              </ItemTemplate>
            </asp:TemplateField>
            <asp:CommandField ShowSelectButton="true" HeaderText="Aktion"/>
          </Columns>
          <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
          <EditRowStyle BackColor="#999999" />
          <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
          <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
          <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
          <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
        </asp:GridView>
      </div>
      
      <p />
      
      <div style="text-align:left;">
        <table style="border:0" cellpadding="0" cellspacing="0">
          <tr>
            <td style="vertical-align:top;">
              <%-- Einfügen, Ändern eines Vermieters --%>
              <asp:DetailsView ID="LandlordDetails" runat="server" AutoGenerateRows="False" 
                               DataSourceID="landlordInsert" OnItemUpdated="LandlordDetails_ItemUpdated"
                               OnItemInserted="LandlordDetails_ItemInserted" OnItemDeleted="LandlordDetails_ItemDeleted"
                               OnItemInserting="LandlordDetails_ItemInserting"
                               Width="400px" CellPadding="4" ForeColor="#333333" DataKeyNames="ID">
                <Fields>
                  <asp:TemplateField HeaderText="ID" >
                    <ItemTemplate>
                      <asp:Label ID="id" runat="server" Text='<%# Bind("ID") %>'></asp:Label>
                    </ItemTemplate>
                  </asp:TemplateField >
                  <asp:BoundField DataField="Firstname" HeaderText="Vorname"  />
                  <asp:BoundField DataField="Name" HeaderText="Name"  />
                  <asp:BoundField DataField="Email" HeaderText="Email"  />
                  <asp:BoundField DataField="Domain" HeaderText="Domain" />
                  <asp:TemplateField HeaderText="Rolle">
                    <EditItemTemplate>
                      <asp:DropDownList ID="roleList" runat="server" SelectedValue='<%# Bind("Role") %>'>
                        <asp:ListItem>ADMIN</asp:ListItem>
                        <asp:ListItem>STANDARD</asp:ListItem>
                        <asp:ListItem>CALENDAR</asp:ListItem>
                        <asp:ListItem>DEMO</asp:ListItem>
                      </asp:DropDownList>
                    </EditItemTemplate>
                    <InsertItemTemplate>
                      <asp:DropDownList ID="roleList" runat="server" SelectedValue='<%# Bind("Role") %>'>
                        <asp:ListItem>ADMIN</asp:ListItem>
                        <asp:ListItem>STANDARD</asp:ListItem>
                        <asp:ListItem>CALENDAR</asp:ListItem>
                        <asp:ListItem Selected="True">DEMO</asp:ListItem>
                      </asp:DropDownList>
                    </InsertItemTemplate>
                    <ItemTemplate>
                      <asp:Label ID="roleLabel" runat="server" Text='<%# Bind("Role") %>'></asp:Label>
                    </ItemTemplate>
                  </asp:TemplateField>
                  <asp:BoundField DataField="NewPassword" HeaderText="Passwort" />
                  <asp:CommandField ShowInsertButton="True" ShowEditButton="True" ShowDeleteButton="true"/>
                </Fields>
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <CommandRowStyle BackColor="#E2DED6"/>
                <EditRowStyle BackColor="#999999" />
                <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                <FieldHeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
              </asp:DetailsView>
            </td>
            <td style="vertical-align:top;padding-left:15px">
              <%-- Tabelle mit Vermietungsobjekten --%>  
              <div style="text-align:left;">
                <asp:GridView ID="GridViewHouses" runat="server" AutoGenerateColumns="False" CellPadding="4"
                  DataSourceID="houseList" ForeColor="#333333" GridLines="Both" DataKeyNames="ID">
                  <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                  <Columns>
                    <asp:BoundField DataField="ID" HeaderText="ID" SortExpression="ID" />
                    <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
                    <asp:BoundField DataField="Location" HeaderText="Ort" SortExpression="Location" />
                    <asp:CommandField ShowSelectButton="true" HeaderText="Aktion"/>
                  </Columns>
                  <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                  <EditRowStyle BackColor="#999999" />
                  <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                  <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                  <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                  <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                </asp:GridView>
              </div>
            </td>
          </tr>
        </table>
        
        <p />
        
        <div>
          <%-- Einfügen, Ändern eines Hauses --%>
          <asp:DetailsView ID="HouseDetails" runat="server" AutoGenerateRows="False" 
                           DataSourceID="houseInsert" OnItemUpdated="HouseDetails_ItemUpdated"
                           OnItemInserting="HouseDetails_ItemInserting" OnItemInserted="HouseDetails_ItemInserted"
                           OnItemDeleted="HouseDetails_ItemDeleted"
                           Width="400px" CellPadding="4" ForeColor="#333333" DataKeyNames="ID">
            <Fields>
              <asp:TemplateField HeaderText="ID" >
                <ItemTemplate>
                  <asp:Label ID="id" runat="server" Text='<%# Bind("ID") %>'></asp:Label>
                </ItemTemplate>
              </asp:TemplateField >
              <asp:BoundField DataField="Name" HeaderText="Name"  />
              <asp:BoundField DataField="Location" HeaderText="Ort"  />
              <asp:CommandField ShowEditButton="True" ShowInsertButton="true" ShowDeleteButton="true"/>
            </Fields>
            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <CommandRowStyle BackColor="#E2DED6"/>
            <EditRowStyle BackColor="#999999" />
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
            <FieldHeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
          </asp:DetailsView>
        </div>    
      </div>
    </div>
  </div>
</asp:Content>

