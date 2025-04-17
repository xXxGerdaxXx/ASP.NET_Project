using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Tryingtofixnotificationentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_NotificationTargetGroups_TargetGroupId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_TargetGroupId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "TargetGroupId",
                table: "Notifications");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_NotificationTargetGroupId",
                table: "Notifications",
                column: "NotificationTargetGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_NotificationTargetGroups_NotificationTargetGroupId",
                table: "Notifications",
                column: "NotificationTargetGroupId",
                principalTable: "NotificationTargetGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_NotificationTargetGroups_NotificationTargetGroupId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_NotificationTargetGroupId",
                table: "Notifications");

            migrationBuilder.AddColumn<int>(
                name: "TargetGroupId",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_TargetGroupId",
                table: "Notifications",
                column: "TargetGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_NotificationTargetGroups_TargetGroupId",
                table: "Notifications",
                column: "TargetGroupId",
                principalTable: "NotificationTargetGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
