using HRManagement.Modules.Personnel.Domain;

namespace HRManagement.Modules.Personnel.Application.UseCases;

public static partial class Mapping
{
    public static CreateRoleCommand ToCreateRoleCommand(this CreateRoleDto dto)
    {
        return new CreateRoleCommand
        {
            Name = dto.Name,
            ReportsToId = dto.ReportsToId
        };
    }

    public static RoleDto ToResponseDto(this Role role)
    {
        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            ReportsTo = role.ReportsTo?.Name
        };
    }
    
    public static UpdateRoleCommand ToUpdateRoleCommand(this UpdateRoleDto dto, byte id)
    {
        return new UpdateRoleCommand
        {
            Id = id,
            Name = dto.Name,
            ReportsToId = dto.ReportsToId
        };
    }
}