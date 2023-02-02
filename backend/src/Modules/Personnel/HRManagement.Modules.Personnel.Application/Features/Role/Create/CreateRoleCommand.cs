using CSharpFunctionalExtensions;
using HRManagement.Common.Domain.Models;
using HRManagement.Modules.Personnel.Application.Contracts.Handlers;
using HRManagement.Modules.Personnel.Application.DTOs;

namespace HRManagement.Modules.Personnel.Application.Features.Role;

public class CreateRoleCommand : ICommand<Result<RoleDto, List<Error>>>
{
    public string Name { get; set; }
    public byte? ReportsToId { get; set; }

    public static CreateRoleCommand MapFromDto(CreateOrUpdateRoleDto dto)
    {
        return new CreateRoleCommand
        {
            Name = dto.Name,
            ReportsToId = dto.ReportsToId
        };
    }
}