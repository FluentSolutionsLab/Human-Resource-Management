namespace HRManagement.Modules.Personnel.Application.UseCases;

public class CreateRoleDto
{
    public string Name { get; set; }
    public byte? ReportsToId { get; set; }
}