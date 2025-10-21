using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogsManagment.Data.Migrations
{
    /// <inheritdoc />
    public partial class ensure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserLog_AspNetUsers_UserId",
                table: "UserLog");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLog_Logs_LogId",
                table: "UserLog");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserLog",
                table: "UserLog");

            migrationBuilder.RenameTable(
                name: "UserLog",
                newName: "UserLogs");

            migrationBuilder.RenameIndex(
                name: "IX_UserLog_LogId",
                table: "UserLogs",
                newName: "IX_UserLogs_LogId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserLogs",
                table: "UserLogs",
                columns: new[] { "UserId", "LogId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserLogs_AspNetUsers_UserId",
                table: "UserLogs",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLogs_Logs_LogId",
                table: "UserLogs",
                column: "LogId",
                principalTable: "Logs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserLogs_AspNetUsers_UserId",
                table: "UserLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLogs_Logs_LogId",
                table: "UserLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserLogs",
                table: "UserLogs");

            migrationBuilder.RenameTable(
                name: "UserLogs",
                newName: "UserLog");

            migrationBuilder.RenameIndex(
                name: "IX_UserLogs_LogId",
                table: "UserLog",
                newName: "IX_UserLog_LogId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserLog",
                table: "UserLog",
                columns: new[] { "UserId", "LogId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserLog_AspNetUsers_UserId",
                table: "UserLog",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLog_Logs_LogId",
                table: "UserLog",
                column: "LogId",
                principalTable: "Logs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
