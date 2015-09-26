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

public partial class ViewFeedback : System.Web.UI.Page
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
            CreateGrid();
        }
    }

    public void CreateGrid()
    {
        try
        {
            Con = new SqlConnection(ConfigurationManager.AppSettings["ConnStr"]);
            string sql = "";
            sql = "select ID, Orderid, Customername, Contactnumber, Serviceopted, Servicestaff, Quality, Ambiance, Greeting, Value, Overall, Comments, Createdon from nva_feedback";
            Da = new SqlDataAdapter(sql, Con);
            DataTable dt = new DataTable();
            Da.Fill(dt);
            dt.TableName = "feedback";
            categoriesdv.Table = dt;
            Con.Close();
            grid1.DataSource = categoriesdv;
            grid1.DataBind();
            if(dt.Rows.Count == 0)
            {
                btnExportExcel.Visible = false;
                btnExportWord.Visible = false;
            }
        }
        catch (Exception ex)
        {
            if (Con.State == ConnectionState.Open)
                Con.Close();
            ErrorMsg.Text = "Error: " + ex.Message;
            btnExportExcel.Visible = false;
            btnExportWord.Visible = false;
        }
    }

    
    protected void grid1_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        grid1.PageIndex = e.NewPageIndex;
        CreateGrid();
    }

    private void ExportGrid(string fileName, string contentType)
    {
        Response.Clear();
        Response.Buffer = true;
        Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
        Response.Charset = "";
        Response.ContentType = contentType;

        StringWriter sw = new StringWriter();
        HtmlTextWriter HW = new HtmlTextWriter(sw);


        // Read Style file (css) here and add to response 
        FileInfo fi = new FileInfo(Server.MapPath(".") + "\\styles\\myGrid.css");
        StringBuilder sb = new StringBuilder();
        StreamReader sr = fi.OpenText();
        while (sr.Peek() >= 0)
        {
            sb.Append(sr.ReadLine());
        }
        sr.Close();

        grid1.RenderControl(HW);
        Response.Write("<html><head><style type='text/css'>" + sb.ToString() + "</style></head><body>" + sw.ToString() + "</body></html>");
        Response.Flush();
        Response.Close();
        Response.End();
    }

    protected void btnExportWord_Click(object sender, EventArgs e)
    {
        // Export Gridview to Word
        ExportGrid("Report.doc", "application/vnd.ms-word");
    }

    protected void btnExportExcel_Click(object sender, EventArgs e)
    {
        // Export Gridview to Excel
        ExportGrid("Report.xls", "application/vnd.ms-excel");
    }

}