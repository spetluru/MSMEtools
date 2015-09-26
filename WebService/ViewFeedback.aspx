<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ViewFeedback.aspx.cs" Inherits="ViewFeedback" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc2" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body style="background:#f3f3f3">
    <form id="form1" runat="server">
    <div>
                <table border="0" cellspacing="0" id="table1" cellpadding="0" width="100%">
	    <tr>
		    <td height="18">
            <table style=" height:50px; width:100%;">
            <tr>
            <td align="left">
                <img src="adminlogo.png" alt="Logo" />
            </td>
     </tr>
            </table>
            
            </td>
		</tr>
    </table>

    <asp:XmlDataSource ID="XmlDataSource2" runat="server" DataFile="Xml/Status.xml" XPath="ListItems/ListItem"></asp:XmlDataSource> 
    <br />
        <asp:Label ID="ErrorMsg" runat="server" Text=""></asp:Label>
     <fieldset id="gridtable" runat="server" style="font-size:34px;color:#006666;">
        <legend>Feedback</legend>
        <asp:GridView ID="grid1" runat="server" Visible="true" AutoGenerateColumns="false"
            AllowPaging="True" PageSize="5" BackColor="#d8d8d8" ShowHeaderWhenEmpty="true"
            BorderColor="#97d9df" BorderStyle="None" BorderWidth="1px" CellPadding="5" 
            CellSpacing="15" onpageindexchanging="grid1_PageIndexChanging" font-size="Large">
        <Columns>
            <asp:BoundField DataField="ID" ReadOnly="true" HeaderText="ID" runat="server" />
            <asp:TemplateField HeaderText="Sl.No">
                <ItemTemplate>
                    <%# ((GridViewRow)Container).RowIndex + 1%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="Orderid" HeaderText="Order ID" runat="server" />
            <asp:BoundField DataField="Customername" HeaderText="Customer Name" runat="server" />
            <asp:BoundField DataField="Contactnumber" HeaderText="Contact Number" runat="server" />
            <asp:BoundField DataField="Serviceopted" HeaderText="Service Opted" runat="server" />
            <asp:BoundField DataField="Servicestaff" HeaderText="Service Staff" runat="server" />
            <asp:BoundField DataField="Quality" HeaderText="Quality" runat="server" />
            <asp:BoundField DataField="Ambiance" HeaderText="Ambiance" runat="server" />
            <asp:BoundField DataField="Greeting" HeaderText="Greeting" runat="server" />
            <asp:BoundField DataField="Value" HeaderText="Value" runat="server" />
            <asp:BoundField DataField="Overall" HeaderText="Overall" runat="server" />
            <asp:BoundField DataField="Comments" HeaderText="Comments" runat="server" />
            <asp:BoundField DataField="Createdon" HeaderText="Created On" runat="server" />
        </Columns>
         <FooterStyle BackColor="#97d9df" ForeColor="#13335e" />
        <HeaderStyle BackColor="#13335e" Font-Bold="True" ForeColor="White" />
        <PagerStyle ForeColor="#13335e" HorizontalAlign="Center" />
        <RowStyle BackColor="#d8d8d8" ForeColor="#000000" />
        <SelectedRowStyle BackColor="#738A9C" Font-Bold="True" ForeColor="White" />
        <SortedAscendingCellStyle BackColor="#97d9df" />
        <SortedAscendingHeaderStyle BackColor="#13335e" />
        <SortedDescendingCellStyle BackColor="#97d9df" />
        <SortedDescendingHeaderStyle BackColor="#13335e" />
        <EmptyDataTemplate>
            <div>
                No Data Available
            </div>
        </EmptyDataTemplate>
    </asp:GridView>
         <br />
         <asp:Button ID="btnExportWord" runat="server" Text="Export to Word" OnClick="btnExportWord_Click"  />
    <asp:Button ID="btnExportExcel" runat="server" Text="Export to Excel" OnClick="btnExportExcel_Click"  />
    </fieldset>
    </div>
    </form>
</body>
</html>
