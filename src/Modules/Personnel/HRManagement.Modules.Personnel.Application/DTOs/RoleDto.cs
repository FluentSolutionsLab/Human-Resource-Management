namespace HRManagement.Modules.Personnel.Application.DTOs;

public class RoleDto
{
    public byte Id { get; set; }
    public string Name { get; set; }
    public byte? ReportsToId { get; set; }

    public static RoleDto MapFromEntity(Domain.Role.Role role)
    {
        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            ReportsToId = role.ReportsTo?.Id
        };
    }
}