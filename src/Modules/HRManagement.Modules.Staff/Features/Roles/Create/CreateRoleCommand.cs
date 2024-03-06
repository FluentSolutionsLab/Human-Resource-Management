using CSharpFunctionalExtensions;
using HRManagement.BuildingBlocks.Contracts;
using HRManagement.BuildingBlocks.Models;
using HRManagement.Modules.Staff.Features.Roles.Get;

namespace HRManagement.Modules.Staff.Features.Roles.Create;

public class CreateRoleCommand : ICommand<Result<RoleDto, Error>>
{
    public string Name { get; set; }
    public int? ReportsToId { get; set; }
}