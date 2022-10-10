using Microsoft.EntityFrameworkCore.Migrations;

namespace WebCoreTutorial.Migrations
{
    public partial class alterPaymentBill : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Payments",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "BillingAddresses",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_UserId",
                table: "Payments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BillingAddresses_UserId",
                table: "BillingAddresses",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BillingAddresses_AppUsers_UserId",
                table: "BillingAddresses",
                column: "UserId",
                principalTable: "AppUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_AppUsers_UserId",
                table: "Payments",
                column: "UserId",
                principalTable: "AppUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BillingAddresses_AppUsers_UserId",
                table: "BillingAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_AppUsers_UserId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_UserId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_BillingAddresses_UserId",
                table: "BillingAddresses");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "BillingAddresses");
        }
    }
}
