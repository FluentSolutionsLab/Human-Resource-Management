using CSharpFunctionalExtensions;
using HRManagement.Modules.Staff.Models;
using HRManagement.Modules.Staff.Models.ValueObjects;

namespace HRManagement.Modules.Staff.Features.Services;

public class RoleCreateOrUpdateDto
{
    public int Id { get; init; }

    public RoleName Name { get; init; }

    public Role RoleToUpdate { get; set; }

    public int? ManagerRoleId { get; init; }
    public Maybe<Role> ManagerRoleOrNothing { get; set; } = Maybe<Role>.None;
}