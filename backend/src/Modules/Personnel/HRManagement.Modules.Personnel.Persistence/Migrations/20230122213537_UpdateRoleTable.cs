using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRManagement.Modules.Personnel.Persistence.Migrations
{
    public partial class UpdateRoleTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Role_Role_ReportsToId",
                table: "Role");

            migrationBuilder.AlterColumn<byte>(
                name: "ReportsToId",
                table: "Role",
                type: "tinyint",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AddForeignKey(
                name: "FK_Role_Role_ReportsToId",
                table: "Role",
                column: "ReportsToId",
                principalTable: "Role",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Role_Role_ReportsToId",
                table: "Role");

            migrationBuilder.AlterColumn<byte>(
                name: "ReportsToId",
                table: "Role",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Role_Role_ReportsToId",
                table: "Role",
                column: "ReportsToId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
