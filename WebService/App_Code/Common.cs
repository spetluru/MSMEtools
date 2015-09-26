using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.IO;
using System.Data;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Threading;
using System.Reflection;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Management;


/// <summary>
/// Summary description for Common
/// </summary>
public class Common
{
    public Dictionary<string, string> dbcolumns = new Dictionary<string, string>();
    char[] pipe = { '|' };

	public Common()
	{
		
	}

    public void GetPropertyDataColumns()
    {
        char[] Pipedelimiter = { '|' };
        char[] equaldelimiter = { '=' };
        string[] cols = ConfigurationManager.AppSettings["ColumnMappings"].ToString().Split(Pipedelimiter);
        for (int i = 0; i < cols.Length; i++)
        {
            string[] data = cols[i].Split(equaldelimiter);
            dbcolumns.Add(data[0], data[1]);
        }
    }

    public static void Export(string fileName, GridView gv)
    {
        HttpContext.Current.Response.Clear();
        HttpContext.Current.Response.AddHeader("content-disposition", string.Format("attachment; filename={0}", fileName));
        HttpContext.Current.Response.ContentType = "application/ms-excel";

        using (StringWriter sw = new StringWriter())
        {
            using (HtmlTextWriter htw = new HtmlTextWriter(sw))
            {
                //  Create a table to contain the grid
                System.Web.UI.WebControls.Table table = new System.Web.UI.WebControls.Table();

                //  include the gridline settings
                table.GridLines = gv.GridLines;

                //  add the header row to the table
                if (gv.HeaderRow != null)
                {
                    PrepareControlForExport(gv.HeaderRow);
                    table.Rows.Add(gv.HeaderRow);
                }

                //  add each of the data rows to the table
                foreach (GridViewRow row in gv.Rows)
                {
                    PrepareControlForExport(row);
                    table.Rows.Add(row);
                }

                //  add the footer row to the table
                if (gv.FooterRow != null)
                {
                    PrepareControlForExport(gv.FooterRow);
                    table.Rows.Add(gv.FooterRow);
                }

                //  render the table into the htmlwriter
                table.RenderControl(htw);

                //  render the htmlwriter into the response
                HttpContext.Current.Response.Write(sw.ToString());
                HttpContext.Current.Response.End();
            }
        }
    }

    private static void PrepareControlForExport(Control control)
    {
        for (int i = 0; i < control.Controls.Count; i++)
        {
            Control current = control.Controls[i];
            if (current is LinkButton)
            {
                control.Controls.Remove(current);
                control.Controls.AddAt(i, new LiteralControl((current as LinkButton).Text));
            }
            else if (current is ImageButton)
            {
                control.Controls.Remove(current);
                control.Controls.AddAt(i, new LiteralControl((current as ImageButton).AlternateText));
            }
            else if (current is HyperLink)
            {
                control.Controls.Remove(current);
                control.Controls.AddAt(i, new LiteralControl((current as HyperLink).Text));
            }
            else if (current is DropDownList)
            {
                control.Controls.Remove(current);
                control.Controls.AddAt(i, new LiteralControl((current as DropDownList).SelectedItem.Text));
            }
            else if (current is CheckBox)
            {
                control.Controls.Remove(current);
                control.Controls.AddAt(i, new LiteralControl((current as CheckBox).Checked ? "True" : "False"));
            }

            if (current.HasControls())
            {
                PrepareControlForExport(current);
            }
        }
    }

    public void ExportToPdf(DataTable ExDataTable)
    {
        //Here set page size as A4

        Document pdfDoc = new Document(PageSize.A4, 10, 10, 10, 10);

        try
        {
            PdfWriter.GetInstance(pdfDoc, System.Web.HttpContext.Current.Response.OutputStream);
            pdfDoc.Open();

            //Set Font Properties for PDF File
            Font fnt = FontFactory.GetFont("Times New Roman", 12);
            DataTable dt = ExDataTable;

            if (dt != null)
            {

                PdfPTable PdfTable = new PdfPTable(dt.Columns.Count);
                PdfPCell PdfPCell = null;

                //Here we create PDF file tables

                for (int rows = 0; rows < dt.Rows.Count; rows++)
                {
                    if (rows == 0)
                    {
                        for (int column = 0; column < dt.Columns.Count; column++)
                        {
                            PdfPCell = new PdfPCell(new Phrase(new Chunk(dt.Columns[column].ColumnName.ToString(), fnt)));
                            PdfTable.AddCell(PdfPCell);
                        }
                    }
                    for (int column = 0; column < dt.Columns.Count; column++)
                    {
                        PdfPCell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), fnt)));
                        PdfTable.AddCell(PdfPCell);
                    }
                }

                // Finally Add pdf table to the document 
                pdfDoc.Add(PdfTable);
            }

            pdfDoc.Close();

            HttpContext.Current.Response.ContentType = "application/pdf";

            //Set default file Name as current datetime
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=" + DateTime.Now.ToString("yyyyMMdd") + ".pdf");
            
            System.Web.HttpContext.Current.Response.Write(pdfDoc);

            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();

        }
        catch (Exception ex)
        {
            HttpContext.Current.Response.Write(ex.ToString());
        }
    }

    public TripleDES CreateDES(string key)
    {
        MD5 md5 = new MD5CryptoServiceProvider();
        TripleDES des = new TripleDESCryptoServiceProvider();
        des.Key = md5.ComputeHash(Encoding.Unicode.GetBytes(key));
        des.IV = new byte[des.BlockSize / 8];
        return des;
    }

    public string Encryption(string PlainText, string key)
    {
        TripleDES des = CreateDES(key);
        ICryptoTransform ct = des.CreateEncryptor();
        byte[] input = Encoding.Unicode.GetBytes(PlainText);
        return Convert.ToBase64String(ct.TransformFinalBlock(input, 0, input.Length));  
    }

    public string Decryption(string CypherText, string key)
    {
        byte[] b = Convert.FromBase64String(CypherText);
        TripleDES des = CreateDES(key);
        ICryptoTransform ct = des.CreateDecryptor();
        byte[] output = ct.TransformFinalBlock(b, 0, b.Length);
        return Encoding.Unicode.GetString(output);
    }

    public string EncryptTripleDES(string Plaintext, string Key)
    {

        System.Security.Cryptography.TripleDESCryptoServiceProvider DES =

        new System.Security.Cryptography.TripleDESCryptoServiceProvider();

        System.Security.Cryptography.MD5CryptoServiceProvider hashMD5 =

        new System.Security.Cryptography.MD5CryptoServiceProvider();

        DES.Key = hashMD5.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(Key));

        DES.Mode = System.Security.Cryptography.CipherMode.ECB;

        System.Security.Cryptography.ICryptoTransform DESEncrypt = DES.CreateEncryptor();

        byte[] Buffer = System.Text.ASCIIEncoding.ASCII.GetBytes(Plaintext);
        string TripleDES = Convert.ToBase64String(DESEncrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));

        return TripleDES;

    }
    public string DecryptTripleDES(string base64Text, string Key)
    {

        System.Security.Cryptography.TripleDESCryptoServiceProvider DES =

        new System.Security.Cryptography.TripleDESCryptoServiceProvider();

        System.Security.Cryptography.MD5CryptoServiceProvider hashMD5 =

        new System.Security.Cryptography.MD5CryptoServiceProvider();
        DES.Key = hashMD5.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(Key));
        DES.Mode = System.Security.Cryptography.CipherMode.ECB;
        System.Security.Cryptography.ICryptoTransform DESDecrypt = DES.CreateDecryptor();
        byte[] Buffer = Convert.FromBase64String(base64Text);

        string DecTripleDES = System.Text.ASCIIEncoding.ASCII.GetString(DESDecrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
        return DecTripleDES;

    }

    public void WriteFile(string FilePath, string Data)
    {
        //this code segment write data to file.
        FileStream fs1 = new FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.Write);
        StreamWriter writer = new StreamWriter(fs1);
        writer.Write(Data);
        writer.Close();
    }

    
    public string GetGUID()
    {
        string GUID = System.Guid.NewGuid().ToString();

        return (GUID);
    }

    public string ReadFile(string FilePath)
    {
        //this code segment read data from the file.
        string readdata;
        FileStream fs2 = new FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.Read);
        StreamReader reader = new StreamReader(fs2);
        readdata = reader.ReadToEnd();
        reader.Close();
        return readdata;
    }


}