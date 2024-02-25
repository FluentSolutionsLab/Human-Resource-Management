using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRManagement.Modules.Staff.Persistence.Migrations
{
    public partial class UpdateEmployeeTable2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employee_Role_RoleId",
                table: "Employee");

            migrationBuilder.AlterColumn<byte>(
                name: "RoleId",
                table: "Employee",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReportsToId",
                table: "Employee",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Employee_ReportsToId",
                table: "Employee",
                column: "ReportsToId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_Employee_ReportsToId",
                table: "Employee",
                column: "ReportsToId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_Role_RoleId",
                table: "Employee",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employee_Employee_ReportsToId",
                table: "Employee");

            migrationBuilder.DropForeignKey(
                name: "FK_Employee_Role_RoleId",
                table: "Employee");

            migrationBuilder.DropIndex(
                name: "IX_Employee_ReportsToId",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "ReportsToId",
                table: "Employee");

            migrationBuilder.AlterColumn<byte>(
                name: "RoleId",
                table: "Employee",
                type: "tinyint",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_Role_RoleId",
                table: "Employee",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "Id");
        }
    }
}
