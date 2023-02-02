namespace HRManagement.Modules.Personnel.Application.DTOs;

public class CreateOrUpdateRoleDto
{
    public string Name { get; set; }
    public byte? ReportsToId { get; set; }
}