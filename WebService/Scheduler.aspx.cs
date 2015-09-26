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

public partial class Scheduler : System.Web.UI.Page
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
                if (Session["user"].ToString() == "Admin")
                {
                    AddBut.Visible = false;
                    EnableBut.Visible = false;
                }
                ErrorMsg.Text = "";
                for (int i = 0; i <= 10; i++)
                {
                    ListItem item = new ListItem((10 + i).ToString() + ":00", (10 + i).ToString() + ":00");
                    TimeDDL.Items.Add(item);
                }
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
            sql = "select ID, CONVERT(VARCHAR(10),Appointmentdate,21) as Appointmentdate, Appointmenttime from nva_scheduler";
            Da = new SqlDataAdapter(sql, Con);
            DataTable dt = new DataTable();
            Da.Fill(dt);
            dt.TableName = "scheduler";
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
            query = "delete from nva_scheduler where ID=" + id;
            SqlCommand cmd = new SqlCommand(query, Con);
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            Con.Close();
            CreateGrid();
            AppointmentDateTB.Text = "";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "a", "<script language='javascript'>alert('Timeslot Deleted Successfully.');</script>");
        }
        catch (Exception ex)
        {
            if (Con.State == ConnectionState.Open)
                Con.Close();
            ErrorMsg.Text = "Error in Deleting Timeslot - " + ex.Message;
        }
    }

    protected void grid1_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        grid1.PageIndex = e.NewPageIndex;
        CreateGrid();
    }

    protected void EnableBut_Click(object sender, EventArgs e)
    {

    }
    protected void DisableBut_Click(object sender, EventArgs e)
    {
        string query = "";
        try
        {
            ErrorMsg.Text = "";
            Con = new SqlConnection(ConfigurationManager.AppSettings["ConnStr"]);
            Con.Open();
            ErrorMsg.Text = "";
            query = "insert into nva_scheduler (Appointmentdate, Appointmenttime, CreatedOn, CreatedBy) ";
            query += "values ('" + AppointmentDateTB.Text.Trim() + "', '" + TimeDDL.SelectedValue + "', GETDATE(), '" + Session["user"].ToString() + "')";
            SqlCommand cmd = new SqlCommand(query, Con);
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            ErrorMsg.Text = "Timeslot Disabled Successfully.";


            Page.ClientScript.RegisterStartupScript(this.GetType(), "a", "<script language='javascript'>alert('Timeslot Disabled Successfully.');</script>");
            Con.Close();

            AppointmentDateTB.Text = "";
            CreateGrid();
        }
        catch (Exception ex)
        {
            if (Con.State == ConnectionState.Open)
                Con.Close();
            ErrorMsg.Text = "Error in disabling Timeslot - " + ex.Message;
        }
    }
}