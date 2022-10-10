using System.Data;
using System.Data.SqlClient;

namespace WebCoreTutorial.Data
{
    public class Users
    {
        DataAccessLayer DAL = new DataAccessLayer();
        DataTable dt = new DataTable();
        public bool state = false;

        public DataTable CheckUserNameExist(string username)
        {
            state = false;
            dt = new DataTable();
            SqlParameter[] param = new SqlParameter[1];
            param[0] = new SqlParameter("@UserName", SqlDbType.NVarChar, 250);
            param[0].Value = username;

            dt = DAL.SelectData("CheckUserNameExist", param);
            this.state = DAL.state;
            return dt;
        }

        public DataTable GetSingleUserRow(string id)
        {
            state = false;
            dt = new DataTable();
            SqlParameter[] param = new SqlParameter[1];
            param[0] = new SqlParameter("@id", SqlDbType.NVarChar, 450);
            param[0].Value = id;

            dt = DAL.SelectData("GetSingleUserRow", param);
            this.state = DAL.state;
            return dt;
        }

        public DataTable CheckEmailConfirmExist(string userId)
        {
            state = false;
            dt = new DataTable();
            SqlParameter[] param = new SqlParameter[1];
            param[0] = new SqlParameter("@UserId", SqlDbType.NVarChar, 450);
            param[0].Value = userId;

            dt = DAL.SelectData("CheckEmailConfirmExist", param);
            this.state = DAL.state;
            return dt;
        }

        public DataTable GetUserRoles()
        {
            state = false;
            dt = new DataTable();
            dt = DAL.SelectData("GetUserRoles", null);
            this.state = DAL.state;
            return dt;
        }

        public DataTable CheckLogin(string username, string password)
        {
            state = false;
            dt = new DataTable();
            SqlParameter[] param = new SqlParameter[2];
            param[0] = new SqlParameter("@UserName", SqlDbType.NVarChar, 250);
            param[0].Value = username;

            param[1] = new SqlParameter("@Password", SqlDbType.NVarChar, 650);
            param[1].Value = password;

            dt = DAL.SelectData("userLogin", param);
            this.state = DAL.state;
            return dt;
        }

        public void UpdateEmailConfirm(string id, bool emailConfirm)
        {
            state = false;
            DAL.open();
            SqlParameter[] param = new SqlParameter[2];
            param[0] = new SqlParameter("@id", SqlDbType.NVarChar, 450);
            param[0].Value = id;

            param[1] = new SqlParameter("@EmailConfirm", SqlDbType.Bit);
            param[1].Value = emailConfirm;

            DAL.ExecuteCommand("UpdateEmailConfirm", param);
            DAL.Close();
            this.state = DAL.state;
        }

        public void DeleteEmailConfirm(string id)
        {
            state = false;
            DAL.open();
            SqlParameter[] param = new SqlParameter[1];
            param[0] = new SqlParameter("@id", SqlDbType.NVarChar, 450);
            param[0].Value = id;

            DAL.ExecuteCommand("DeleteConfirm", param);
            DAL.Close();
            this.state = DAL.state;
        }
    }
}
