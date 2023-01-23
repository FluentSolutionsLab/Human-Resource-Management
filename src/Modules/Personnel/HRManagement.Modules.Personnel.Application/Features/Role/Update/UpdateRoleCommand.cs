using CSharpFunctionalExtensions;
using HRManagement.Common.Domain.Models;
using HRManagement.Modules.Personnel.Application.Contracts.Handlers;
using HRManagement.Modules.Personnel.Application.DTOs;
using MediatR;

namespace HRManagement.Modules.Personnel.Application.Features.Role;

public class UpdateRoleCommand : ICommand<Result<Unit, List<Error>>>
{
    public byte Id { get; set; }
    public string Name { get; set; }
    public byte? ReportsToId { get; set; }

    public static UpdateRoleCommand MapFromDto(byte id, CreateOrUpdateRoleDto dto)
    {
        return new UpdateRoleCommand
        {
            Id = id,
            Name = dto.Name,
            ReportsToId = dto.ReportsToId
        };
    }
}