using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Adjustednotificationentities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserEntityId",
                table: "DismissedNotifications",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DismissedNotifications_UserEntityId",
                table: "DismissedNotifications",
                column: "UserEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_DismissedNotifications_AspNetUsers_UserEntityId",
                table: "DismissedNotifications",
                column: "UserEntityId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DismissedNotifications_AspNetUsers_UserEntityId",
                table: "DismissedNotifications");

            migrationBuilder.DropIndex(
                name: "IX_DismissedNotifications_UserEntityId",
                table: "DismissedNotifications");

            migrationBuilder.DropColumn(
                name: "UserEntityId",
                table: "DismissedNotifications");
        }
    }
}
