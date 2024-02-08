using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRManagement.Modules.Staff.Persistence.Migrations
{
    public partial class AddPersonnelManagementSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "PersonnelManagement");

            migrationBuilder.RenameTable(
                name: "Role",
                newName: "Role",
                newSchema: "PersonnelManagement");

            migrationBuilder.RenameTable(
                name: "Employee",
                newName: "Employee",
                newSchema: "PersonnelManagement");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Role",
                schema: "PersonnelManagement",
                newName: "Role");

            migrationBuilder.RenameTable(
                name: "Employee",
                schema: "PersonnelManagement",
                newName: "Employee");
        }
    }
}
