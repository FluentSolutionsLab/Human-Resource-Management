using CSharpFunctionalExtensions;
using HRManagement.Common.Application.Contracts;

namespace HRManagement.Modules.Staff.Features.GetRoles;

public class GetRolesQuery : IQuery<Result<List<RoleDto>>>
{
    public int PageSize { get; set; }
}