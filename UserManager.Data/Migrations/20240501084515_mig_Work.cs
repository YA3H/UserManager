using Microsoft.EntityFrameworkCore.Migrations;

namespace UserManager.Data.Migrations
{
    public partial class mig_Work : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_UserWorks_UserId1",
                table: "UserWorks",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_UserWorks_Users_UserId1",
                table: "UserWorks",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserWorks_Users_UserId1",
                table: "UserWorks");

            migrationBuilder.DropIndex(
                name: "IX_UserWorks_UserId1",
                table: "UserWorks");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "UserWorks");

            migrationBuilder.DropColumn(
                name: "UserId2",
                table: "UserWorks");
        }
    }
}
