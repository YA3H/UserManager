using Microsoft.EntityFrameworkCore.Migrations;

namespace UserManager.Data.Migrations
{
    public partial class mig_UpdateRoleRank : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserWorks_Users_UserId1",
                table: "UserWorks");

            migrationBuilder.DropForeignKey(
                name: "FK_UserWorks_Work_WorkId1",
                table: "UserWorks");

            migrationBuilder.DropIndex(
                name: "IX_UserWorks_UserId1",
                table: "UserWorks");

            migrationBuilder.DropIndex(
                name: "IX_UserWorks_WorkId1",
                table: "UserWorks");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "UserWorks");

            migrationBuilder.DropColumn(
                name: "UserId2",
                table: "UserWorks");

            migrationBuilder.DropColumn(
                name: "WorkId1",
                table: "UserWorks");

            migrationBuilder.DropColumn(
                name: "WorkId2",
                table: "UserWorks");

            migrationBuilder.AddColumn<bool>(
                name: "Supervisor",
                table: "UserWorks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "Roles",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Supervisor",
                table: "UserWorks");

            migrationBuilder.DropColumn(
                name: "Rank",
                table: "Roles");

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "UserWorks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId2",
                table: "UserWorks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WorkId1",
                table: "UserWorks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WorkId2",
                table: "UserWorks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UserWorks_UserId1",
                table: "UserWorks",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_UserWorks_WorkId1",
                table: "UserWorks",
                column: "WorkId1");

            migrationBuilder.AddForeignKey(
                name: "FK_UserWorks_Users_UserId1",
                table: "UserWorks",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserWorks_Work_WorkId1",
                table: "UserWorks",
                column: "WorkId1",
                principalTable: "Work",
                principalColumn: "WorkId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
