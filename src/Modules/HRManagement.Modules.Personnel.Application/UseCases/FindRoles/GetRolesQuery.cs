using System.Collections.Generic;

namespace HRManagement.Modules.Personnel.Application.UseCases;

public class GetRolesQuery : IQuery<Result<List<RoleDto>>>
{
    public int PageSize { get; set; }
}