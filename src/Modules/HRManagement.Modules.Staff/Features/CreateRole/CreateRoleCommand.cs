using CSharpFunctionalExtensions;
using HRManagement.BuildingBlocks.Contracts;
using HRManagement.BuildingBlocks.Models;
using HRManagement.Modules.Staff.Features.GetRoles;

namespace HRManagement.Modules.Staff.Features.CreateRole;

public class CreateRoleCommand : ICommand<Result<RoleDto, Error>>
{
    public string Name { get; set; }
    public int? ReportsToId { get; set; }
}