using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Collections;
using System.Web.UI.WebControls;

/// <summary>
/// Summary description for DBConnect
/// </summary>
public class DBConnect
{
    private SqlConnection connection;
    public int ErrorCode;
    public string ErrorMsg;
    Common cmn = new Common();
    string connectionString;

    public string propertydatasql;
    public string propertydatasearchsql;
    public string filterexpression;

    public string uploadreference;
    public char[] PipeDelimiter = { '|' };
    public char[] EqualDelimiter = { '=' };
    public char[] HyphenDelimiter = { '-' };
    public char[] CommaDelimiter = { ',' };

    public bool IsUnitAvailable = true;

    //Constructor
    public DBConnect()
    {
        Initialize();
    }

    //Initialize values
    private void Initialize()
    {
        ResetError();

        connectionString = GetConnectionString();
        
        connection = new SqlConnection(connectionString);
    }

    public string GetConnectionString()
    {
        connectionString = ConfigurationManager.AppSettings["ConnStr"];
        int startpos = connectionString.IndexOf("password=") + 9;
        int endpos = connectionString.IndexOf(';', startpos);

        //string enc = cmn.Encryption(connectionString.Substring(startpos, endpos - startpos), "RamaKiranKumar");
        string finalconnstr = connectionString.Replace(connectionString.Substring(startpos, endpos - startpos), cmn.Decryption(connectionString.Substring(startpos, endpos - startpos), "RamaKiranKumar"));
        return finalconnstr;
    }

    private void ResetError()
    {
        ErrorCode = -1;
        ErrorMsg = "";
    }

    public string NumberToText(string inputdata)
    {
        int number = 0;
        if(!inputdata.Contains("__") && !inputdata.Trim().Contains(""))
            number = Convert.ToInt32(inputdata);
        if (number == 0) return "Zero";
        if (number == -2147483648) return "Minus Two Hundred and Fourteen Crore Seventy Four Lakh Eighty Three Thousand Six Hundred and Forty Eight";
        int[] num = new int[4];
        int first = 0;
        int u, h, t;
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        if (number < 0)
        {
            sb.Append("Minus ");
            number = -number;
        }
        string[] words0 = {"" ,"One ", "Two ", "Three ", "Four ", "Five " ,"Six ", "Seven ", "Eight ", "Nine "};
        string[] words1 = {"Ten ", "Eleven ", "Twelve ", "Thirteen ", "Fourteen ", "Fifteen ","Sixteen ","Seventeen ","Eighteen ", "Nineteen "};
        string[] words2 = {"Twenty ", "Thirty ", "Forty ", "Fifty ", "Sixty ", "Seventy ","Eighty ", "Ninety "};
        string[] words3 = {"Thousand ", "Lakh ","Crore "};
        num[0] = number % 1000; // units
        num[1] = number / 1000;
        num[2] = number / 100000;
        num[1] = num[1] - 100 * num[2]; // thousands
        num[3] = number / 10000000; // crores
        num[2] = num[2] - 100 * num[3]; // lakhs
        for(int i = 3; i > 0 ; i--)
        {
            if (num[i] != 0)
            {
                first = i;
                break;
            }
        }
        for(int i = first ; i >= 0 ; i--)
        {
            if (num[i] == 0) continue;
            u = num[i] % 10; // ones
            t = num[i] / 10;
            h = num[i] / 100; // hundreds
            t = t - 10 * h; // tens
            if (h > 0) sb.Append(words0[h] + "Hundred ");
            if (u > 0 || t > 0)
            {
                if (h > 0 || i == 0) sb.Append("and ");
                if (t == 0)
                    sb.Append(words0[u]);
                else if (t == 1)
                    sb.Append(words1[u]);
                else
                    sb.Append(words2[t-2] + words0[u]);
            }
            if (i != 0) sb.Append(words3[i-1]);
        }
        return sb.ToString().TrimEnd();
    }


    //open connection to database
    private bool OpenConnection()
    {
        ResetError();
        try
        {
            connection.Open();
            return true;
        }
        catch (SqlException ex)
        {
            //When handling errors, you can your application's response based 
            //on the error number.
            //The two most common error numbers when connecting are as follows:
            //0: Cannot connect to server.
            //1045: Invalid user name and/or password.
            switch (ex.Number)
            {
                case 0:
                    ErrorCode = 0;
                    ErrorMsg = "Cannot connect to server.  Contact administrator";
                    break;
                case 1045:
                    ErrorCode = 1045;
                    ErrorMsg = "Invalid username/password, please try again";
                    break;
            }
            return false;
        }
    }

    //Close connection
    private bool CloseConnection()
    {
        ResetError();
        try
        {
            connection.Close();
            return true;
        }
        catch (SqlException ex)
        {
            ErrorCode = ex.Number;
            ErrorMsg = ex.Message;
            return false;
        }
    }

    //Insert statement
    public void Insert(string query)
    {
        ResetError();

        try
        {
            //open connection
            if (this.OpenConnection() == true)
            {
                //create command and assign the query and connection from the constructor
                SqlCommand cmd = new SqlCommand(query, connection);

                //Execute command
                cmd.ExecuteNonQuery();

                //close connection
                this.CloseConnection();
            }
        }
        catch (Exception ex)
        {
            ErrorCode = -1;
            ErrorMsg = ex.Message;
            if (connection.State == ConnectionState.Open)
                this.CloseConnection();
        }
    }

    //Update statement
    public void Update(string query)
    {
        ResetError();
        //string query = "UPDATE tableinfo SET name='Joe', age='22' WHERE name='John Smith'";

        try
        {
            //Open connection
            if (this.OpenConnection() == true)
            {
                //create Sql command
                SqlCommand cmd = new SqlCommand();
                //Assign the query using CommandText
                cmd.CommandText = query;
                //Assign the connection using Connection
                cmd.Connection = connection;

                //Execute query
                cmd.ExecuteNonQuery();

                //close connection
                this.CloseConnection();
            }
        }
        catch (Exception ex)
        {
            ErrorCode = -1;
            ErrorMsg = ex.Message;
            if (connection.State == ConnectionState.Open)
                this.CloseConnection();
        }
    }

    //Delete statement
    public void Delete(string query)
    {
        ResetError();
        try
        {
            if (this.OpenConnection() == true)
            {
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                this.CloseConnection();
            }
        }
        catch (Exception ex)
        {
            ErrorCode = 100;
            ErrorMsg = ex.Message;
        }
    }

    //Select statement
    public List<string>[] Select()
    {
        ResetError();
        string query = "SELECT * FROM tableinfo";

        //Create a list to store the result
        List<string>[] list = new List<string>[3];
        list[0] = new List<string>();
        list[1] = new List<string>();
        list[2] = new List<string>();

        //Open connection
        if (this.OpenConnection() == true)
        {
            //Create Command
            SqlCommand cmd = new SqlCommand(query, connection);
            //Create a data reader and Execute the command
            SqlDataReader dataReader = cmd.ExecuteReader();

            //Read the data and store them in the list
            while (dataReader.Read())
            {
                list[0].Add(dataReader["id"] + "");
                list[1].Add(dataReader["name"] + "");
                list[2].Add(dataReader["age"] + "");
            }

            //close Data Reader
            dataReader.Close();

            //close Connection
            this.CloseConnection();

            //return list to be displayed
            return list;
        }
        else
        {
            if (connection.State == ConnectionState.Open) connection.Close();
            return list;
        }
    }

    //Count statement
    public int Count(string query)
    {
        ResetError();
        
        int Count = -1;

        //Open Connection
        if (this.OpenConnection() == true)
        {
            //Create Sql Command
            SqlCommand cmd = new SqlCommand(query, connection);

            //ExecuteScalar will return one value
            Count = int.Parse(cmd.ExecuteScalar() + "");

            //close Connection
            this.CloseConnection();

            return Count;
        }
        else
        {
            if (connection.State == ConnectionState.Open) connection.Close();
            return Count;
        }
    }

    public string Get4characters(string data)
    {
        string result = "";
        if (data == "-1")
            data = "1";
        if (data.Length < 4)
        {
            for (int i = 0; i < (4 - data.Length); i++)
            {
                result = result + "0";
            }
            result = result + data;
        }
        else
            result = data;
        return result;
    }

    public int ReferenceCount(string tablename)
    {
        ResetError();
        string query = "SELECT Count(*) FROM " + tablename;
        int Count = -1;

        //Open Connection
        if (this.OpenConnection() == true)
        {
            //Create Sql Command
            SqlCommand cmd = new SqlCommand(query, connection);

            //ExecuteScalar will return one value
            Count = int.Parse(cmd.ExecuteScalar() + "");

            //close Connection
            this.CloseConnection();

            if (Count == 0)
                Count = 1;
            return Count;
        }
        else
        {
            if (connection.State == ConnectionState.Open) connection.Close();
            return Count;
        }
    }


    public List<string>[] GetStates()
    {
        ResetError();
        string query = "SELECT state, short_name FROM epvs_states";

        //Create a list to store the result
        List<string>[] list = new List<string>[2];
        list[0] = new List<string>();
        list[1] = new List<string>();

        list[0].Add("--Select--");
        list[1].Add("--None--");
        //Open connection
        if (this.OpenConnection() == true)
        {
            //Create Command
            SqlCommand cmd = new SqlCommand(query, connection);
            //Create a data reader and Execute the command
            SqlDataReader dataReader = cmd.ExecuteReader();

            //Read the data and store them in the list
            while (dataReader.Read())
            {
                list[0].Add(dataReader["state"] + "");
                list[1].Add(dataReader["short_name"] + "");
            }

            //close Data Reader
            dataReader.Close();

            //close Connection
            this.CloseConnection();

            //return list to be displayed
            return list;
        }
        else
        {
            if (connection.State == ConnectionState.Open) connection.Close();
            return list;
        }
    }

    public List<string>[] GetNotesList()
    {
        ResetError();
        string query = "SELECT notes, notes_id FROM epvs_notes_mst";

        //Create a list to store the result
        List<string>[] list = new List<string>[2];
        list[0] = new List<string>();
        list[1] = new List<string>();

        list[0].Add("--Select--");
        list[1].Add("--None--");
        //Open connection
        if (this.OpenConnection() == true)
        {
            //Create Command
            SqlCommand cmd = new SqlCommand(query, connection);
            //Create a data reader and Execute the command
            SqlDataReader dataReader = cmd.ExecuteReader();

            //Read the data and store them in the list
            while (dataReader.Read())
            {
                list[0].Add(dataReader["notes"] + "");
                list[1].Add(dataReader["notes_id"] + "");
            }

            //close Data Reader
            dataReader.Close();

            //close Connection
            this.CloseConnection();

            //return list to be displayed
            return list;
        }
        else
        {
            if (connection.State == ConnectionState.Open) connection.Close();
            return list;
        }
    }

    public DataSet GetUserDetails(string username, string password)
    {
        DataSet dsInfo = new DataSet();
        String Connstr = ConfigurationManager.AppSettings["ConnStr"];
        using (SqlConnection conn = new SqlConnection(GetConnectionString()))
        using (SqlCommand cmd = new SqlCommand())
        {
            try
            {
                conn.Open();
                cmd.Connection = conn;
                cmd.CommandText = "select a.ID, a.TYPE, b.ROLE, a.PERSONNAME, a.CLIENTID, a.VENDORID, a.STATUS from ATS_users a, ATS_Roles b where a.Email='" + username + "' and a.password='" + password + "' and a.Role=b.ID";
                cmd.CommandType = CommandType.Text;

                SqlDataAdapter SqlDataAdapter = new SqlDataAdapter(cmd);

                SqlDataAdapter.Fill(dsInfo, "UserInfo");
                conn.Close();
            }
            catch (Exception ex)
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }


        }
        return dsInfo;
    }

    public DataSet GetDistrictsList1(string statename)
    {
        DataSet dsInfo = new DataSet();


        using (SqlConnection conn = new SqlConnection(GetConnectionString()))
        using (SqlCommand cmd = new SqlCommand())
        {
            conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = "select district from epvs_district_mst where state='" + statename + "'";
            cmd.CommandType = CommandType.Text;
            
            SqlDataAdapter SqlDataAdapter = new SqlDataAdapter(cmd);
            try
            {
                SqlDataAdapter.Fill(dsInfo, "DistrictInfo");
                conn.Close();
            }
            catch { if (conn.State == ConnectionState.Open) conn.Close(); }


        }
        return dsInfo;
    }

    public List<string>[] GetReferencesList()
    {
        ResetError();
        string query = "SELECT distinct(Reference) FROM epvs_references_mst";

        //Create a list to store the result
        List<string>[] list = new List<string>[1];
        list[0] = new List<string>();

        list[0].Add("--Select--");

        //Open connection
        if (this.OpenConnection() == true)
        {
            //Create Command
            SqlCommand cmd = new SqlCommand(query, connection);
            //Create a data reader and Execute the command
            SqlDataReader dataReader = cmd.ExecuteReader();

            //Read the data and store them in the list
            while (dataReader.Read())
            {
                list[0].Add(dataReader["Reference"] + "");
            }

            //close Data Reader
            dataReader.Close();

            //close Connection
            this.CloseConnection();

            //return list to be displayed
            return list;
        }
        else
        {
            if (connection.State == ConnectionState.Open) connection.Close();
            return list;
        }
    }

    public List<string>[] GetClientsList()
    {
        ResetError();
        string query = "SELECT distinct(Client) FROM epvs_clients_mst where Confirmed='Yes'";

        //Create a list to store the result
        List<string>[] list = new List<string>[1];
        list[0] = new List<string>();

        list[0].Add("--Select--");

        //Open connection
        if (this.OpenConnection() == true)
        {
            //Create Command
            SqlCommand cmd = new SqlCommand(query, connection);
            //Create a data reader and Execute the command
            SqlDataReader dataReader = cmd.ExecuteReader();

            //Read the data and store them in the list
            while (dataReader.Read())
            {
                list[0].Add(dataReader["Client"] + "");
            }

            //close Data Reader
            dataReader.Close();

            //close Connection
            this.CloseConnection();

            //return list to be displayed
            return list;
        }
        else
        {
            if (connection.State == ConnectionState.Open) connection.Close();
            return list;
        }
    }

    public List<string>[] GetDelayLettersList()
    {
        ResetError();
        
        string query = "SELECT distinct(EmailSubject) FROM epvs_EmailTemplateTbl";

        //Create a list to store the result
        List<string>[] list = new List<string>[1];
        list[0] = new List<string>();

        list[0].Add("--Select--");

        //Open connection
        if (this.OpenConnection() == true)
        {
            //Create Command
            SqlCommand cmd = new SqlCommand(query, connection);
            //Create a data reader and Execute the command
            SqlDataReader dataReader = cmd.ExecuteReader();

            //Read the data and store them in the list
            while (dataReader.Read())
            {
                list[0].Add(dataReader["EmailSubject"] + "");
            }

            //close Data Reader
            dataReader.Close();

            //close Connection
            this.CloseConnection();

            //return list to be displayed
            return list;
        }
        else
        {
            if (connection.State == ConnectionState.Open) connection.Close();
            return list;
        }
    }

    public DataSet GetDataSetOfTable(string qry, string tablename)
    {
        DataSet dsInfo = new DataSet();


        using (SqlConnection conn = new SqlConnection(GetConnectionString()))
        using (SqlCommand cmd = new SqlCommand())
        {
            cmd.CommandTimeout = 0;
            conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = qry;
            cmd.CommandType = CommandType.Text;

            SqlDataAdapter SqlDataAdapter = new SqlDataAdapter(cmd);
            try
            {
                SqlDataAdapter.Fill(dsInfo, tablename);
                conn.Close();
            }
            catch { if (conn.State == ConnectionState.Open) conn.Close(); }


        }
        return dsInfo;
    }

    public Dictionary<string, string> GetTableDescription(string tablename)
    {
        Dictionary<string, string> description = new Dictionary<string, string>();
        SqlConnection conn = new SqlConnection(GetConnectionString());
        DataSet ds = new DataSet();
        using (SqlCommand cmd = new SqlCommand())
        {
            conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = "show columns from " + tablename;
            cmd.CommandType = CommandType.Text;

            SqlDataAdapter SqlDataAdapter = new SqlDataAdapter(cmd);
            try
            {
                SqlDataAdapter.Fill(ds, tablename);
                DataTable item = ds.Tables[0];

                foreach (DataRow row in item.Rows)
                {
                    description.Add(row["Field"].ToString().ToLower(), row["Type"].ToString().ToLower());
                }
                conn.Close();
            }
            catch { if (conn.State == ConnectionState.Open) conn.Close(); }
        }
        return description;
    }
   
    public Dictionary<string, string> GetTableDescription()
    {
        Dictionary<string, string> description = new Dictionary<string, string>();
        SqlConnection conn = new SqlConnection(GetConnectionString());
        DataSet ds = new DataSet();
        using (SqlCommand cmd = new SqlCommand())
        {
            conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = "show columns from epvs_propertydata";
            cmd.CommandType = CommandType.Text;

            SqlDataAdapter SqlDataAdapter = new SqlDataAdapter(cmd);
            try
            {
                SqlDataAdapter.Fill(ds, "propertydata");
                DataTable item = ds.Tables[0];

                foreach (DataRow row in item.Rows)
                {
                    description.Add(row["Field"].ToString().ToLower(), row["Type"].ToString().ToLower());
                }
                conn.Close();
            }
            catch { if (conn.State == ConnectionState.Open) conn.Close(); }
        }
        return description;
    }
      
    public DataSet GetDataFromTable(string qry, string tablename, string colname)
    {
        DataSet ds = new DataSet();
        if (OpenConnection() == true)
        {
            SqlDataAdapter dataadapter = new SqlDataAdapter(qry, connection);
            dataadapter.Fill(ds, tablename);
            CloseConnection();
        }
        if (connection.State == ConnectionState.Open) connection.Close();
        DataRow newRow = ds.Tables[0].NewRow();
        newRow[colname] = "--Select--";
        ds.Tables[0].Rows.InsertAt(newRow, 0);
        return ds;
    }

    public DataSet GetStoreysAboveLand(string qry, string tablename, string colname)
    {
        DataSet ds = new DataSet();
        if (OpenConnection() == true)
        {
            SqlDataAdapter dataadapter = new SqlDataAdapter(qry, connection);
            dataadapter.Fill(ds, tablename);
            CloseConnection();
        }
        if (connection.State == ConnectionState.Open) connection.Close();
        DataRow newRow = ds.Tables[0].NewRow();
        newRow[colname] = "-1";
        ds.Tables[0].Rows.InsertAt(newRow, 0);
        return ds;
    }

    public List<string>[] GetUserProjects(int userid)
    {
        ResetError();
        string query = "SELECT ProjectName FROM User_Projects where Status='Active' and UserID=" + userid;

        //Create a list to store the result
        List<string>[] list = new List<string>[1];
        list[0] = new List<string>();

        list[0].Add("--Select--");

        //Open connection
        if (this.OpenConnection() == true)
        {
            //Create Command
            SqlCommand cmd = new SqlCommand(query, connection);
            //Create a data reader and Execute the command
            SqlDataReader dataReader = cmd.ExecuteReader();

            //Read the data and store them in the list
            while (dataReader.Read())
            {
                list[0].Add(dataReader["ProjectName"] + "");
            }

            //close Data Reader
            dataReader.Close();

            //close Connection
            this.CloseConnection();

            //return list to be displayed
            return list;
        }
        else
        {
            return list;
        }
    }

    public DropDownList GetUsers()
    {
        ResetError();
        string query = "select user_id, name from users";
        DropDownList ddl = new DropDownList();
        ListItem item = new ListItem("--Select--", "0");
        ddl.Items.Add(item);
        if (this.OpenConnection() == true)
        {
            SqlCommand cmd = new SqlCommand(query, connection);
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                item = new ListItem(dataReader["name"].ToString(), dataReader["user_id"].ToString());
                ddl.Items.Add(item);
            }
            dataReader.Close();
            this.CloseConnection();
            return ddl;
        }
        else
        {
            return ddl;
        }
    }

    public DropDownList GetAdminUsers()
    {
        ResetError();
        string query = "select user_id, name from users";
        DropDownList ddl = new DropDownList();
        ListItem item = new ListItem("--Select--", "0");
        ddl.Items.Add(item);
        item = new ListItem("All", "0");
        ddl.Items.Add(item);
        if (this.OpenConnection() == true)
        {
            SqlCommand cmd = new SqlCommand(query, connection);
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                item = new ListItem(dataReader["name"].ToString(), dataReader["user_id"].ToString());
                ddl.Items.Add(item);
            }
            dataReader.Close();
            this.CloseConnection();
            return ddl;
        }
        else
        {
            return ddl;
        }
    }

    public DropDownList GetUserProjectIDs(int userid)
    {
        ResetError();
        string query = "select a.Name, a.ProjectID from projects a, user_projects b where a.projectid=b.projectid and a.Status='Active' and b.UserID=" + userid;
        DropDownList ddl = new DropDownList();
        ListItem item = new ListItem("--Select--", "0");
        ddl.Items.Add(item);
        if (this.OpenConnection() == true)
        {
            SqlCommand cmd = new SqlCommand(query, connection);
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                item = new ListItem(dataReader["Name"].ToString(), dataReader["ProjectID"].ToString());
                ddl.Items.Add(item);
            }
            dataReader.Close();
            this.CloseConnection();
            return ddl;
        }
        else
        {
            return ddl;
        }
    }

    public DropDownList GetTasks()
    {
        ResetError();
        string query = "select TaskName, ID from tasks where Status='Active'";
        DropDownList ddl = new DropDownList();
        ListItem item = new ListItem("--Select--", "0");
        ddl.Items.Add(item);
        if (this.OpenConnection() == true)
        {
            SqlCommand cmd = new SqlCommand(query, connection);
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                item = new ListItem(dataReader["TaskName"].ToString(), dataReader["ID"].ToString());
                ddl.Items.Add(item);
            }
            dataReader.Close();
            this.CloseConnection();
            return ddl;
        }
        else
        {
            return ddl;
        }
    }

    public DropDownList GetCategories()
    {
        ResetError();
        string query = "select CategoryName, ID from Categories where Status='Active'";
        DropDownList ddl = new DropDownList();
        ListItem item = new ListItem("--Select--", "0");
        ddl.Items.Add(item);
        if (this.OpenConnection() == true)
        {
            SqlCommand cmd = new SqlCommand(query, connection);
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                item = new ListItem(dataReader["CategoryName"].ToString(), dataReader["ID"].ToString());
                ddl.Items.Add(item);
            }
            dataReader.Close();
            this.CloseConnection();
            return ddl;
        }
        else
        {
            return ddl;
        }
    }

    public DropDownList GetProducts()
    {
        ResetError();
        string query = "select Name, ProductID from Products where Status='Active'";
        DropDownList ddl = new DropDownList();
        ListItem item = new ListItem("--Select--", "0");
        ddl.Items.Add(item);
        if (this.OpenConnection() == true)
        {
            SqlCommand cmd = new SqlCommand(query, connection);
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                item = new ListItem(dataReader["Name"].ToString(), dataReader["ProductID"].ToString());
                ddl.Items.Add(item);
            }
            dataReader.Close();
            this.CloseConnection();
            return ddl;
        }
        else
        {
            return ddl;
        }
    }

    public DropDownList GetPurposes()
    {
        ResetError();
        string query = "select PurposeName, ID from Purposes where Status='Active'";
        DropDownList ddl = new DropDownList();
        ListItem item = new ListItem("--Select--", "0");
        ddl.Items.Add(item);
        if (this.OpenConnection() == true)
        {
            SqlCommand cmd = new SqlCommand(query, connection);
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                item = new ListItem(dataReader["PurposeName"].ToString(), dataReader["ID"].ToString());
                ddl.Items.Add(item);
            }
            dataReader.Close();
            this.CloseConnection();
            return ddl;
        }
        else
        {
            return ddl;
        }
    }

    public List<string>[] GetDesignations()
    {
        ResetError();
        string query = "SELECT Designation FROM Designations where Status='Active'";

        //Create a list to store the result
        List<string>[] list = new List<string>[1];
        list[0] = new List<string>();

        list[0].Add("--Select--");

        //Open connection
        if (this.OpenConnection() == true)
        {
            //Create Command
            SqlCommand cmd = new SqlCommand(query, connection);
            //Create a data reader and Execute the command
            SqlDataReader dataReader = cmd.ExecuteReader();

            //Read the data and store them in the list
            while (dataReader.Read())
            {
                list[0].Add(dataReader["Designation"] + "");
            }

            //close Data Reader
            dataReader.Close();

            //close Connection
            this.CloseConnection();

            //return list to be displayed
            return list;
        }
        else
        {
            return list;
        }
    }

    public List<string>[] GetRoles()
    {
        ResetError();
        string query = "SELECT Role FROM Roles where Status=1";

        //Create a list to store the result
        List<string>[] list = new List<string>[1];
        list[0] = new List<string>();

        list[0].Add("--Select--");

        //Open connection
        if (this.OpenConnection() == true)
        {
            //Create Command
            SqlCommand cmd = new SqlCommand(query, connection);
            //Create a data reader and Execute the command
            SqlDataReader dataReader = cmd.ExecuteReader();

            //Read the data and store them in the list
            while (dataReader.Read())
            {
                list[0].Add(dataReader["Role"] + "");
            }

            //close Data Reader
            dataReader.Close();

            //close Connection
            this.CloseConnection();

            //return list to be displayed
            return list;
        }
        else
        {
            return list;
        }
    }

    public List<string>[] GetDepartments()
    {
        ResetError();
        string query = "SELECT Department FROM Departments where Status=1";

        //Create a list to store the result
        List<string>[] list = new List<string>[1];
        list[0] = new List<string>();

        list[0].Add("--Select--");

        //Open connection
        if (this.OpenConnection() == true)
        {
            //Create Command
            SqlCommand cmd = new SqlCommand(query, connection);
            //Create a data reader and Execute the command
            SqlDataReader dataReader = cmd.ExecuteReader();

            //Read the data and store them in the list
            while (dataReader.Read())
            {
                list[0].Add(dataReader["Department"] + "");
            }

            //close Data Reader
            dataReader.Close();

            //close Connection
            this.CloseConnection();

            //return list to be displayed
            return list;
        }
        else
        {
            return list;
        }
    }

   
}