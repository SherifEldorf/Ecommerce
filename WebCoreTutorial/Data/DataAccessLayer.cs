//Abufahmi Code Generator -------- Data Access Layer ---------
using System;
using System.Data;
using System.Data.SqlClient;

namespace WebCoreTutorial.Data
{
    public class DataAccessLayer
    {
        SqlConnection con;
        public string a = string.Empty;
        public bool state = false;

        public DataAccessLayer()
        {
            con = new SqlConnection(@"Server = .\SQLExpress; Database = CoreDb; Integrated Security = True;");
        }

        public void open()
        {
            if (con.State != ConnectionState.Open)
            {
                con.Open();
            }
        }

        public void Close()
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
        }

        public DataTable SelectData(string Stored_Procedure, SqlParameter[] param)
        {
            state = false;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = Stored_Procedure;
            cmd.Connection = con;

            if (param != null)
            {
                for (int i = 0; i < param.Length; i++)
                {
                    cmd.Parameters.Add(param[i]);
                }
            }
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            try
            {
                da.Fill(dt);
                state = true;
            }
            catch
            {
                state = false;
            }
            return dt;
        }

        public void ExecuteCommand(string Stored_Procedure, SqlParameter[] param)
        {
            try
            {
                state = false;
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = Stored_Procedure;
                cmd.Connection = con;

                if (param != null)
                {
                    cmd.Parameters.AddRange(param);
                }
                cmd.ExecuteNonQuery();
                state = true;
            }
            catch
            {
                state = false;
            }
        }

        public string GetString(string Stored_Procedure, SqlParameter[] param)
        {
            a = string.Empty;
            state = false;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = Stored_Procedure;
            cmd.Connection = con;

            if (param != null)
            {
                for (int i = 0; i < param.Length; i++)
                {
                    cmd.Parameters.Add(param[i]);
                }
            }
            try
            {
                open();
                a = cmd.ExecuteScalar().ToString();
                Close();
                state = true;
            }
            catch
            {
                state = false;
            }
            return a;
        }
    }
}

