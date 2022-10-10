using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace WebCoreTutorial.Data
{
    public static class AppAuthentication
    {
        public static string GetRoleId(string roleName)
        {
            string str = string.Empty;
            DataTable dt = new DataTable();
            SqlParameter[] param = new SqlParameter[1];
            param[0] = new SqlParameter("@RoleName", SqlDbType.NVarChar, 150);
            param[0].Value = roleName;

            DataAccessLayer DAL = new DataAccessLayer();
            dt = DAL.SelectData("GetMemberId", param);
            if(dt.Rows.Count > 0)
            {
                str = dt.Rows[0][0].ToString();
            }

            return str;
        }

        public static string GetRoleName(string userName)
        {
            string str = string.Empty;
            DataTable dt = new DataTable();
            SqlParameter[] param = new SqlParameter[1];
            param[0] = new SqlParameter("@UserName", SqlDbType.NVarChar, 250);
            param[0].Value = userName;

            DataAccessLayer DAL = new DataAccessLayer();
            dt = DAL.SelectData("GetUserRoles_ByUserName", param);
            if (dt.Rows.Count > 0)
            {
                str = dt.Rows[0][3].ToString();
            }

            return str;
        }

        public static string GetIdByUserName(string username)
        {
            string name = string.Empty;
            DataTable dt = new DataTable();
            Users users = new Users();
            dt = users.CheckUserNameExist(username);
            if(dt.Rows.Count > 0)
            {
                name = dt.Rows[0][0].ToString();
            }
            return name;
        }
    }
}
