using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Editeddismissednotificationentityagain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "DismissedNotifications",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_DismissedNotifications_UserId",
                table: "DismissedNotifications",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_DismissedNotifications_AspNetUsers_UserId",
                table: "DismissedNotifications",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DismissedNotifications_AspNetUsers_UserId",
                table: "DismissedNotifications");

            migrationBuilder.DropIndex(
                name: "IX_DismissedNotifications_UserId",
                table: "DismissedNotifications");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "DismissedNotifications",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

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
    }
}
