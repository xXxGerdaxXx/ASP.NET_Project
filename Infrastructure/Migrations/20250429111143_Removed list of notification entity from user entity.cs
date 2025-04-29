using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Removedlistofnotificationentityfromuserentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_AspNetUsers_UserEntityId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_UserEntityId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "UserEntityId",
                table: "Notifications");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserEntityId",
                table: "Notifications",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserEntityId",
                table: "Notifications",
                column: "UserEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_AspNetUsers_UserEntityId",
                table: "Notifications",
                column: "UserEntityId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
