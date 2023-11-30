using HRManagement.Modules.Personnel.Domain;

namespace HRManagement.Modules.Personnel.Application.UseCases.Services;

public class RoleCreateOrUpdateDto
{
    public byte Id { get; init; }

    public RoleName Name { get; init; }

    public Role RoleToUpdate { get; set; }

    public byte? ManagerRoleId { get; init; }
    public Maybe<Role> ManagerRoleOrNothing { get; set; } = Maybe<Role>.None;
}