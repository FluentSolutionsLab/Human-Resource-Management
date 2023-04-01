namespace HRManagement.Modules.Personnel.Application.UseCases;

public class UpdateRoleDto
{
    public string Name { get; set; }
    public byte? ReportsToId { get; set; }
}