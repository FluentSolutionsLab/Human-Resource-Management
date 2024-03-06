using CSharpFunctionalExtensions;
using HRManagement.BuildingBlocks.Contracts;

namespace HRManagement.Modules.Staff.Features.Roles.Get;

public class GetRolesQuery : IQuery<Result<List<RoleDto>>>
{
    public int PageSize { get; set; }
}