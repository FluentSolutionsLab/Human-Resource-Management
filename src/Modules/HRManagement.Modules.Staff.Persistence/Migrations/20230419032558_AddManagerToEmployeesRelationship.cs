using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRManagement.Modules.Staff.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddManagerToEmployeesRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employee_Employee_ReportsToId",
                schema: "PersonnelManagement",
                table: "Employee");

            migrationBuilder.RenameColumn(
                name: "ReportsToId",
                schema: "PersonnelManagement",
                table: "Employee",
                newName: "ManagerId");

            migrationBuilder.RenameIndex(
                name: "IX_Employee_ReportsToId",
                schema: "PersonnelManagement",
                table: "Employee",
                newName: "IX_Employee_ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_Employee_ManagerId",
                schema: "PersonnelManagement",
                table: "Employee",
                column: "ManagerId",
                principalSchema: "PersonnelManagement",
                principalTable: "Employee",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employee_Employee_ManagerId",
                schema: "PersonnelManagement",
                table: "Employee");

            migrationBuilder.RenameColumn(
                name: "ManagerId",
                schema: "PersonnelManagement",
                table: "Employee",
                newName: "ReportsToId");

            migrationBuilder.RenameIndex(
                name: "IX_Employee_ManagerId",
                schema: "PersonnelManagement",
                table: "Employee",
                newName: "IX_Employee_ReportsToId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_Employee_ReportsToId",
                schema: "PersonnelManagement",
                table: "Employee",
                column: "ReportsToId",
                principalSchema: "PersonnelManagement",
                principalTable: "Employee",
                principalColumn: "Id");
        }
    }
}
