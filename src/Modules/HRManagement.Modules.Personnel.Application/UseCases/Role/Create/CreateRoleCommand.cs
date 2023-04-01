using System.Collections.Generic;

namespace HRManagement.Modules.Personnel.Application.UseCases;

public class CreateRoleCommand : ICommand<Result<RoleDto, List<Error>>>
{
    public string Name { get; set; }
    public byte? ReportsToId { get; set; }
}