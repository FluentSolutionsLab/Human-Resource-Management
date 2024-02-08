using System.Collections.Generic;

namespace HRManagement.Modules.Staff.Application.UseCases;

public class GetRolesQuery : IQuery<Result<List<RoleDto>>>
{
    public int PageSize { get; set; }
}