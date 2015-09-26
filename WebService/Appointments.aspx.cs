using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Text;
using System.Net;
using System.Globalization;

public partial class Appointments : System.Web.UI.Page
{
    public SqlDataAdapter Da = new SqlDataAdapter();
    public string Connection = ConfigurationManager.AppSettings["ConnStr"];
    public DataView categoriesdv = new DataView();
    private SqlConnection Con;
    DataSet ds = new DataSet();
    DataTable dt = new DataTable();

    protected void Page_Load(object sender, EventArgs e)
    {
        Connection = ConfigurationManager.AppSettings["ConnStr"];
        Session["user"] = "Customer";
        if (!IsPostBack)
        {
            if (Session["user"] != null)
            {
                if(Session["user"].ToString() == "Customer")
                {
                    EnableBut.Visible = false;
                    DisableBut.Visible = false;
                    gridtable.Visible = false;
                } else if (Session["user"].ToString() == "Admin")
                {
                    gridtable.Visible = true;
                }
                ErrorMsg.Text = "";
                /*for (int i = 0; i <= 10; i++)
                {
                    ListItem item = new ListItem((10 + i).ToString() + ":00", (10 + i).ToString() + ":00");
                    TimeDDL.Items.Add(item);
                }*/
                CreateGrid();
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }
    }

    public void CreateGrid()
    {
        try
        {
            Con = new SqlConnection(ConfigurationManager.AppSettings["ConnStr"]);
            string sql = "";
            sql = "select ID, Orderid, CONVERT(VARCHAR(10),Appointmentdate,21) as Appointmentdate, Appointmenttime, Phone from nva_appointments";
            Da = new SqlDataAdapter(sql, Con);
            DataTable dt = new DataTable();
            Da.Fill(dt);
            dt.TableName = "appointments";
            categoriesdv.Table = dt;
            Con.Close();
            grid1.DataSource = categoriesdv;
            grid1.DataBind();
        }
        catch (Exception ex)
        {
            if (Con.State == ConnectionState.Open)
                Con.Close();
            ErrorMsg.Text = "Error: " + ex.Message;
        }
    }

    protected void AddBut_Click(object sender, EventArgs e)
    {
        string query = "";
        try
        {
            ErrorMsg.Text = "";

            if (AddBut.Text == "Confirm Appointment")
            {
                Con = new SqlConnection(ConfigurationManager.AppSettings["ConnStr"]);
                Con.Open();
                ErrorMsg.Text = "";
                query = "insert into nva_appointments (Orderid, Appointmentdate, Appointmenttime, Phone, CreatedOn, CreatedBy) ";
                query += "values ('" + OrderidTB.Text.Trim() + "', '" + AppointmentDateTB.Text.Trim() + "', '" + TimeDDL.SelectedValue + "', '" + PhoneTB.Text.Trim() + "', GETDATE(), '" + Session["user"].ToString() + "')";
                SqlCommand cmd = new SqlCommand(query, Con);
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                ErrorMsg.Text = "Appointment Created Successfully.";

                SendSMS(false, true);
                SendSMS(false, false);
                SendSMS(true, true);

                Page.ClientScript.RegisterStartupScript(this.GetType(), "a", "<script language='javascript'>alert('Appointment Created Successfully.');</script>");
                Con.Close();

                OrderidTB.Text = "";
                AppointmentDateTB.Text = "";
            }
            else if (AddBut.Text == "Update")
            {
                Con = new SqlConnection(Connection);
                Con.Open();
                query = "update nva_appointments set Orderid='" + OrderidTB.Text.Trim() + "', Appointmentdate='" + AppointmentDateTB.Text.Trim() + "', ";
                query += "Appointmenttime='" + TimeDDL.SelectedValue + "', Phone='" + PhoneTB.Text.Trim() + "', ";
                query += "ModifiedOn=GETDATE(), ModifiedBy='" + Session["user"].ToString() + "' where ID='" + IDTB.Value + "'";
                SqlCommand cmd = new SqlCommand(query, Con);
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                ErrorMsg.Text = "Appointment Updated Successfully.";
                IDTB.Value = "";
                OrderidTB.Text = "";
                AppointmentDateTB.Text = "";

                SendSMS(false, true);
                SendSMS(false, false);
                SendSMS(true, false);

                Page.ClientScript.RegisterStartupScript(this.GetType(), "a", "<script language='javascript'>alert('Appointment Updated Successfully.');</script>");
                AddBut.Text = "Confirm Appointment";
                Con.Close();
            }

            CreateGrid();
        }
        catch (Exception ex)
        {
            if (Con.State == ConnectionState.Open)
                Con.Close();
            ErrorMsg.Text = "Error in Creating Appointment - " + ex.Message;
        }
    }

    public void SendSMS(bool schedule, bool owner)
    {
        StringBuilder sb = new StringBuilder();
        byte[] buf = new byte[8192];
        int time1 = Convert.ToInt32(TimeDDL.SelectedValue.ToString().Substring(0, 2));
        time1 = time1 - Convert.ToInt32(ConfigurationManager.AppSettings["Scheduletime"]);
        string query = "";
        query = "http://trans.deifysolutions.com/sendsms.jsp?user=testuser&password=P@ssw0rd&mobiles=";
        if (!owner)
            query += PhoneTB.Text;
        else
            query += ConfigurationManager.AppSettings["OwnerMobile"];

        
        if(!owner)
            query += "&sms=" + ConfigurationManager.AppSettings["Appointmentsms"];
        else
            query += "&sms=" + ConfigurationManager.AppSettings["Adminsms"];
        query += "&senderid=" + ConfigurationManager.AppSettings["Senderid"] + "&clientsmsid=123002";
        query = query.Replace("(Date)", AppointmentDateTB.Text.Trim());
        query = query.Replace("(Time)", time1.ToString() + ":00:00");
        if(schedule)
            query += "&scheduletime=" + AppointmentDateTB.Text + " " + time1.ToString() + ":00:00";
        //do get request
        HttpWebRequest request = (HttpWebRequest)
            WebRequest.Create(query);
        HttpWebResponse response = (HttpWebResponse)
            request.GetResponse();
        Stream resStream = response.GetResponseStream();
        string tempString = null;
        int count = 0;
        //read the data and print it
        do
        {
            count = resStream.Read(buf, 0, buf.Length);
            if (count != 0)
            {
                tempString = Encoding.ASCII.GetString(buf, 0, count);
                sb.Append(tempString);
            }
        }
        while (count > 0);
        //Response.Write(sb.ToString());
    }

    protected void EditBut_Click(object sender, ImageClickEventArgs e)
    {
        ImageButton lb = (ImageButton)sender;
        GridViewRow gvr = (GridViewRow)lb.NamingContainer;
        IDTB.Value = gvr.Cells[0].Text.ToString();
        OrderidTB.Text = gvr.Cells[2].Text.ToString() == "&nbsp;" ? "" : gvr.Cells[2].Text.ToString();
        AppointmentDateTB.Text = gvr.Cells[3].Text.ToString() == "&nbsp;" ? "" : gvr.Cells[3].Text.ToString();
        TimeDDL.SelectedValue = gvr.Cells[4].Text.ToString() == "&nbsp;" ? "0" : gvr.Cells[4].Text.ToString();
        PhoneTB.Text = gvr.Cells[5].Text.ToString() == "&nbsp;" ? "" : gvr.Cells[5].Text.ToString();
        AddBut.Text = "Update";
    }

    protected void DeleteBut_Click(object sender, ImageClickEventArgs e)
    {
        ImageButton lb = (ImageButton)sender;
        GridViewRow gvr = (GridViewRow)lb.NamingContainer;
        int id = Convert.ToInt32(gvr.Cells[0].Text.ToString());
        string query = "";
        try
        {
            ErrorMsg.Text = "";
            Con = new SqlConnection(Connection);
            Con.Open();
            query = "delete from nva_appointments where ID=" + id;
            SqlCommand cmd = new SqlCommand(query, Con);
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            Con.Close();
            CreateGrid();
            OrderidTB.Text = "";
            AppointmentDateTB.Text = "";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "a", "<script language='javascript'>alert('Appointment Deleted Successfully.');</script>");
        }
        catch (Exception ex)
        {
            if (Con.State == ConnectionState.Open)
                Con.Close();
            ErrorMsg.Text = "Error in Deleting Appointment - " + ex.Message;
        }
    }

    protected void grid1_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        grid1.PageIndex = e.NewPageIndex;
        CreateGrid();
    }

    protected void AppointmentDateTB_TextChanged(object sender, EventArgs e)
    {
        DateTime appdt = DateTime.ParseExact(AppointmentDateTB.Text.Trim(), "yyyy-MM-dd",  new CultureInfo("en-Us", true), DateTimeStyles.NoCurrentDateDefault);
        if(appdt.DayOfWeek == DayOfWeek.Tuesday)
        {
            AppointmentDateTB.Text = "";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "a", "<script language='javascript'>alert('Tuesday is Non-Working day.');</script>");
            return;
        }
        Con = new SqlConnection(ConfigurationManager.AppSettings["ConnStr"]);
        string sql = "";
        sql = "select ID, CONVERT(VARCHAR(10),Appointmentdate,21) as Appointmentdate, Appointmenttime, Phone from nva_scheduler where Appointmentdate='" + AppointmentDateTB.Text.Trim() + "'";
        Da = new SqlDataAdapter(sql, Con);
        DataTable dt = new DataTable();
        Da.Fill(dt);
        Con.Close();
        TimeDDL.Items.Clear();
        for (int i = 0; i <= 10; i++)
        {
            ListItem item = new ListItem((10 + i).ToString() + ":00", (10 + i).ToString() + ":00");
            bool found = false;
            if (dt.Rows.Count > 0)
            {
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    if(dt.Rows[j]["Appointmenttime"].ToString() == (10 + i).ToString() + ":00")
                    {
                        found = true;
                        break;
                    }
                }
            }
            if(!found)
                TimeDDL.Items.Add(item);
        }
        
    }

}