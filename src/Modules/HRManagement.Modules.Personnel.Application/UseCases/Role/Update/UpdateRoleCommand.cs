using System.Collections.Generic;

namespace HRManagement.Modules.Personnel.Application.UseCases;

public class UpdateRoleCommand : ICommand<UnitResult<List<Error>>>
{
    public byte Id { get; set; }
    public string Name { get; set; }
    public byte? ReportsToId { get; set; }
}