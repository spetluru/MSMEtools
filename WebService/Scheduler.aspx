<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Scheduler.aspx.cs" Inherits="Scheduler" %>
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
        <legend>Disable Time Slot</legend>
        <table>
            <tr id="tr2" runat="server">
                <td style="color:#006666;font-size:28px;">Date: </td>
                <td><asp:TextBox ID="AppointmentDateTB" runat="server" Text=""></asp:TextBox>
                    <cc2:calendarextender ID="AppointmentDateTB_CalendarExtender" runat="server" Enabled="True" 
                        TargetControlID="AppointmentDateTB" Format="yyyy-MM-dd"></cc2:calendarextender>
                </td>
                <td style="color:#006666;font-size:28px;">Time to disable: </td>
                <td><asp:DropDownList ID="TimeDDL" runat="server"></asp:DropDownList></td>
                <td style="color:#13335e; font-weight:bolder;"><asp:Button ID="AddBut" runat="server" Text="Add Appointment" /></td>
                <td style="color:#13335e; font-weight:bolder;"><asp:Button ID="EnableBut" runat="server" Text="Enable" OnClick="EnableBut_Click"/></td>
                <td style="color:#13335e; font-weight:bolder;"><asp:Button ID="DisableBut" runat="server" Text="Disable" OnClick="DisableBut_Click"/></td>
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
        <legend>Timeslot Schedule</legend>
        <asp:GridView ID="grid1" runat="server" Visible="true" AutoGenerateColumns="false"
            AllowPaging="True" PageSize="5" BackColor="#d8d8d8" ShowHeaderWhenEmpty="true"
            BorderColor="#97d9df" BorderStyle="None" BorderWidth="1px" CellPadding="3" 
            CellSpacing="2" onpageindexchanging="grid1_PageIndexChanging" font-size="Large">
        <Columns>
            <asp:BoundField DataField="ID" ReadOnly="true" HeaderText="ID" runat="server" />
            <asp:TemplateField HeaderText="Sl.No">
                <ItemTemplate>
                    <%# ((GridViewRow)Container).RowIndex + 1%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="Appointmentdate" HeaderText="Appointment Date" runat="server" />
            <asp:BoundField DataField="Appointmenttime" HeaderText="Time" runat="server" />
            <asp:TemplateField HeaderText="Actions" >
                <ItemTemplate>
                    <asp:ImageButton ID="Delete" runat="server" ImageUrl="icon-delete.png" CausesValidation="false" OnClientClick="return confirm('Do You Want to Delete This Timeslot?');" OnClick="DeleteBut_Click" />
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
