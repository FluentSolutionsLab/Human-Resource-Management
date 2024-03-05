using CSharpFunctionalExtensions;
using HRManagement.Common.Application.Contracts;
using HRManagement.Common.Domain.Models;
using HRManagement.Modules.Staff.Features.FindRoles;

namespace HRManagement.Modules.Staff.Features.CreateRole;

public class CreateRoleCommand : ICommand<Result<RoleDto, Error>>
{
    public string Name { get; set; }
    public int? ReportsToId { get; set; }
}