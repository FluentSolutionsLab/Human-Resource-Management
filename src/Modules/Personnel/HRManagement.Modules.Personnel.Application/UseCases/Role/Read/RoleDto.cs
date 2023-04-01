namespace HRManagement.Modules.Personnel.Application.UseCases;

public class RoleDto
{
    public byte Id { get; set; }
    public string Name { get; set; }
    public string ReportsTo { get; set; }
}