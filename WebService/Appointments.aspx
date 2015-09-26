<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Appointments.aspx.cs" Inherits="Appointments" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc2" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body style="background:#f3f3f3">
    <form id="form1" runat="server" style="width:60%">
    <div>
   

    <asp:XmlDataSource ID="XmlDataSource2" runat="server" DataFile="Xml/Status.xml" XPath="ListItems/ListItem"></asp:XmlDataSource> 
    <fieldset style="font-size:34px;color:#006666; width=50%">
        <legend>Book your appointment:</legend>
        <table>
            <tr id="tr2" runat="server">
                <td style="color:#006666;font-size:28px;">Order ID: </td>
                <td><asp:TextBox ID="OrderidTB" runat="server" Text=""></asp:TextBox></td>
                <td style="color:#006666;font-size:28px;">Date of Appointment: </td>
                <td><asp:TextBox ID="AppointmentDateTB" runat="server" Text="" OnTextChanged="AppointmentDateTB_TextChanged" AutoPostBack="true"></asp:TextBox>
                    <cc2:calendarextender ID="AppointmentDateTB_CalendarExtender" runat="server" Enabled="True" 
                        TargetControlID="AppointmentDateTB" Format="yyyy-MM-dd"></cc2:calendarextender>
                </td>
                <td style="color:#006666;font-size:28px;">Time: </td>
                <td><asp:DropDownList ID="TimeDDL" runat="server"></asp:DropDownList></td></tr><tr runat="server">
                <td style="color:#006666;font-size:28px;">Contact Number: </td>
                <td><asp:TextBox ID="PhoneTB" runat="server" Text=""></asp:TextBox></td> <td></td>
                <td rowspan=2><asp:Button style="color:#13335e; font-weight:bolder;" ID="AddBut" runat="server" Text="Confirm Appointment" OnClick="AddBut_Click"/></td>
                <td><asp:Button style="color:#13335e; font-weight:bolder;" ID="EnableBut" runat="server" Text="Enable" /></td>
                <td><asp:Button  style="color:#13335e; font-weight:bolder;" ID="DisableBut" runat="server" Text="Disable" /></td>
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
        <legend>Appointments</legend>
        <asp:GridView ID="grid1" runat="server" Visible="true" AutoGenerateColumns="false"
            AllowPaging="True" PageSize="5" BackColor="#DEBA84" ShowHeaderWhenEmpty="true"
            BorderColor="#DEBA84" BorderStyle="None" BorderWidth="1px" CellPadding="3" 
            CellSpacing="2" onpageindexchanging="grid1_PageIndexChanging">
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
            <asp:BoundField DataField="Phone" HeaderText="Contact Number" runat="server" />
            <asp:TemplateField HeaderText="Actions" >
                <ItemTemplate>
                    <asp:ImageButton ID="Edit" runat="server" ImageUrl="Images/icon-edit.gif" CausesValidation="false" OnClick="EditBut_Click" />
                    <asp:ImageButton ID="Delete" runat="server" ImageUrl="Images/icon-delete.gif" CausesValidation="false" OnClientClick="return confirm('Do You Want to Delete This Appointment?');" OnClick="DeleteBut_Click" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <FooterStyle BackColor="#F7DFB5" ForeColor="#8C4510" />
        <HeaderStyle BackColor="#A55129" Font-Bold="True" ForeColor="White" />
        <PagerStyle ForeColor="#8C4510" HorizontalAlign="Center" />
        <RowStyle BackColor="#FFF7E7" ForeColor="#8C4510" />
        <SelectedRowStyle BackColor="#738A9C" Font-Bold="True" ForeColor="White" />
        <SortedAscendingCellStyle BackColor="#FFF1D4" />
        <SortedAscendingHeaderStyle BackColor="#B95C30" />
        <SortedDescendingCellStyle BackColor="#F1E5CE" />
        <SortedDescendingHeaderStyle BackColor="#93451F" />
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
