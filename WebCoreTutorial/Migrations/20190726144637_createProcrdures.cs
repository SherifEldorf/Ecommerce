using Microsoft.EntityFrameworkCore.Migrations;

namespace WebCoreTutorial.Migrations
{
    public partial class createProcrdures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sp = "Create Procedure CheckUserNameExist" + "\r\n" +
          "@UserName nvarchar(250)" + "\r\n" +
          "as" + "\r\n" +
          "Select * from AppUsers" + "\r\n" +
          "Where UserName=@UserName";
            migrationBuilder.Sql(sp);

            sp = "Create Procedure GetSingleUserRow" + "\r\n" +
               "@id nvarchar(450)" + "\r\n" +
               "as" + "\r\n" +
               "Select * from AppUsers" + "\r\n" +
               "Where id=@id";
            migrationBuilder.Sql(sp);

            sp = "Create Procedure CheckEmailConfirmExist" + "\r\n" +
               "@UserId nvarchar(450)" + "\r\n" +
               "as" + "\r\n" +
               "Select * from AppConfirms" + "\r\n" +
               "Where UserId=@UserId";
            migrationBuilder.Sql(sp);

            sp = "Create Procedure UpdateEmailConfirm" + "\r\n" +
               "@id nvarchar(450)," + "\r\n" +
               "@EmailConfirm bit" + "\r\n" +
               "as" + "\r\n" +
               "Update AppUsers Set" + "\r\n" +
               "EmailConfirm=@EmailConfirm" + "\r\n" +
               "Where id=@id";
            migrationBuilder.Sql(sp);

            sp = "Create Procedure DeleteConfirm" + "\r\n" +
             "@id nvarchar(450)" + "\r\n" +
             "as" + "\r\n" +
             "Delete from AppConfirms" + "\r\n" +
             "Where UserId=@id";
            migrationBuilder.Sql(sp);

            sp = "Create Procedure GetMemberId" + "\r\n" +
            "@RoleName nvarchar(150)" + "\r\n" +
            "as" + "\r\n" +
            "Select * from AppRoles" + "\r\n" +
            "Where RoleName=@RoleName";
            migrationBuilder.Sql(sp);

            sp = "Create Procedure GetUserRoles" + "\r\n" +
           "as" + "\r\n" +
           "Select" + "\r\n" +
           "UserRoles.id," + "\r\n" +
           "RoleId," + "\r\n" +
           "UserId," + "\r\n" +
           "RoleName," + "\r\n" +
           "UserName" + "\r\n" +
           "from UserRoles" + "\r\n" + "\r\n" +
           "inner join AppRoles" + "\r\n" +
           "on AppRoles.id = UserRoles.RoleId" + "\r\n" + "\r\n" +
           "inner join AppUsers" + "\r\n" +
           "on AppUsers.id = UserRoles.UserId";
            migrationBuilder.Sql(sp);

            sp = "create proc userLogin" + "\r\n" +
                      "@UserName nvarchar(250)," + "\r\n" +
                      "@Password nvarchar(650)" + "\r\n" +
                      "as" + "\r\n" +
                      "select * from AppUsers" + "\r\n" +
                      "where UserName = @UserName and[Password] = @Password";
            migrationBuilder.Sql(sp);

            sp = "create Procedure [dbo].[GetUserRoles_ByUserName]" + "\r\n" +
            "@UserName nvarchar(250)" + "\r\n" +
            "as" + "\r\n" +
            "Select" + "\r\n" +
            "UserRoles.id," + "\r\n" +
            "RoleId," + "\r\n" +
            "UserId," + "\r\n" +
            "RoleName," + "\r\n" +
            "UserName" + "\r\n" +
            "from UserRoles" + "\r\n" + "\r\n" +
            "inner join AppRoles" + "\r\n" +
            "on AppRoles.id = UserRoles.RoleId" + "\r\n" + "\r\n" +
            "inner join AppUsers" + "\r\n" +
            "on AppUsers.id = UserRoles.UserId" + "\r\n" +
            "where UserName = @UserName";
            migrationBuilder.Sql(sp);

            sp = "create proc GetUserSetting" + "\r\n" +
                 "as" + "\r\n" +
                 "select * from UserSettings";
            migrationBuilder.Sql(sp);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
