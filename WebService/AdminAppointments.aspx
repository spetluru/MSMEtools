<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AdminAppointments.aspx.cs" Inherits="AdminAppointments" %>
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
    <fieldset style="font-size:34px;color:#006666;">
        <legend>Reserve an Appointment</legend>
        <table >
            <tr id="tr2" runat="server">
                <td style="color:#006666;font-size:28px;">Order ID: </td>
                <td ><asp:TextBox ID="OrderidTB" runat="server" Text=""></asp:TextBox></td>
                <td style="color:#006666;font-size:28px;">Date of Appointment: </td>
                <td style="color:#006666;font-size:28px;"><asp:TextBox ID="AppointmentDateTB" runat="server" Text="" OnTextChanged="AppointmentDateTB_TextChanged" AutoPostBack="true"></asp:TextBox>
                    <cc2:calendarextender ID="AppointmentDateTB_CalendarExtender" runat="server" Enabled="True" 
                        TargetControlID="AppointmentDateTB" Format="yyyy-MM-dd"></cc2:calendarextender>
                </td>
                <td style="color:#006666;font-size:28px;">Time: </td>
                <td><asp:DropDownList ID="TimeDDL" runat="server"></asp:DropDownList></td>
                <td><asp:Button style="color:#13335e; font-weight:bolder;" ID="AddBut" runat="server" Text="Add Appointment" OnClick="AddBut_Click"/></td>
                <td><asp:Button ID="EnableBut" runat="server" Text="Enable" /></td>
                <td><asp:Button ID="DisableBut" runat="server" Text="Disable" /></td>
            </tr>            
            <tr>
                <td colspan="6"><asp:Label ID="ErrorMsg" runat="server" Text=""></asp:Label>
                    <asp:HiddenField ID="IDTB" runat="server" />
                    <asp:ScriptManager ID="ScriptManager2" runat="server">
                    </asp:ScriptManager>
                </td>
            </tr>
        </table>
    </fieldset>
    <br />
     <fieldset id="gridtable" runat="server" style="font-size:34px;color:#006666;">
        <legend>Existing Appointments</legend>
        <asp:GridView ID="grid1" runat="server" Visible="true" AutoGenerateColumns="false"
            AllowPaging="True" PageSize="5" BackColor="#d8d8d8" ShowHeaderWhenEmpty="true"
            BorderColor="#97d9df" BorderStyle="None" BorderWidth="3px" CellPadding="4"  


            CellSpacing="15" onpageindexchanging="grid1_PageIndexChanging" font-size="Large">
        <Columns>
            <asp:BoundField DataField="ID" ReadOnly="true" HeaderText="ID" runat="server" />
            <asp:TemplateField HeaderText="Sl.No">
                <ItemTemplate>
                    <%# ((GridViewRow)Container).RowIndex + 1%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="Orderid" HeaderText="Order ID" runat="server" />
            <asp:BoundField DataField="Appointmentdate" HeaderText="Appointment Date" runat="server" />
            <asp:BoundField DataField="Appointmenttime" HeaderText="Time" runat="server" />
            <asp:TemplateField HeaderText="Actions" >
                <ItemTemplate>
                    <asp:ImageButton ID="Edit" runat="server" ImageUrl="icon-edit.png" CausesValidation="false" OnClick="EditBut_Click" />
                    <asp:ImageButton ID="Delete" runat="server" ImageUrl="icon-delete.png" CausesValidation="false" OnClientClick="return confirm('Do You Want to Delete This Appointment?');" OnClick="DeleteBut_Click" />
                </ItemTemplate>
            </asp:TemplateField>
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
    </fieldset>
    </div>
    </form>
</body>
</html>
