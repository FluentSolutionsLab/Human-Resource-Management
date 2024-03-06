using CSharpFunctionalExtensions;
using HRManagement.BuildingBlocks.Contracts;
using HRManagement.BuildingBlocks.Models;

namespace HRManagement.Modules.Staff.Features.Roles.Get;

public class GetRoleByIdQuery : IQuery<Result<RoleDto, Error>>
{
    public byte Id { get; set; }
}