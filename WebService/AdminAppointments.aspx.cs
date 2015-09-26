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

public partial class AdminAppointments : System.Web.UI.Page
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
        Session["user"] = "Admin";
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
                    EnableBut.Visible = false;
                    DisableBut.Visible = false;
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
            sql = "select ID, Orderid, CONVERT(VARCHAR(10),Appointmentdate,21) as Appointmentdate, Appointmenttime from nva_appointments";
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

            if (AddBut.Text == "Add Appointment")
            {
                Con = new SqlConnection(ConfigurationManager.AppSettings["ConnStr"]);
                Con.Open();
                ErrorMsg.Text = "";
                query = "insert into nva_appointments (Orderid, Appointmentdate, Appointmenttime, CreatedOn, CreatedBy) ";
                query += "values ('" + OrderidTB.Text.Trim() + "', '" + AppointmentDateTB.Text.Trim() + "', '" + TimeDDL.SelectedValue + "', GETDATE(), '" + Session["user"].ToString() + "')";
                SqlCommand cmd = new SqlCommand(query, Con);
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                ErrorMsg.Text = "Appointment Created Successfully.";


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
                query += "Appointmenttime='" + TimeDDL.SelectedValue + "', ";
                query += "ModifiedOn=GETDATE(), ModifiedBy='" + Session["user"].ToString() + "' where ID='" + IDTB.Value + "'";
                SqlCommand cmd = new SqlCommand(query, Con);
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                ErrorMsg.Text = "Appointment Updated Successfully.";
                IDTB.Value = "";
                OrderidTB.Text = "";
                AppointmentDateTB.Text = "";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "a", "<script language='javascript'>alert('Appointment Updated Successfully.');</script>");
                AddBut.Text = "Add Appointment";
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

    protected void EditBut_Click(object sender, ImageClickEventArgs e)
    {
        ImageButton lb = (ImageButton)sender;
        GridViewRow gvr = (GridViewRow)lb.NamingContainer;
        IDTB.Value = gvr.Cells[0].Text.ToString();
        OrderidTB.Text = gvr.Cells[2].Text.ToString() == "&nbsp;" ? "" : gvr.Cells[2].Text.ToString();
        AppointmentDateTB.Text = gvr.Cells[3].Text.ToString() == "&nbsp;" ? "" : gvr.Cells[3].Text.ToString();
        TimeDDL.SelectedValue = gvr.Cells[4].Text.ToString() == "&nbsp;" ? "0" : gvr.Cells[4].Text.ToString();
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
        Con = new SqlConnection(ConfigurationManager.AppSettings["ConnStr"]);
        string sql = "";
        sql = "select ID, CONVERT(VARCHAR(10),Appointmentdate,21) as Appointmentdate, Appointmenttime from nva_scheduler where Appointmentdate='" + AppointmentDateTB.Text.Trim() + "'";
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