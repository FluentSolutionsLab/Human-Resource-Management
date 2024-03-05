using CSharpFunctionalExtensions;
using HRManagement.BuildingBlocks.Contracts;
using HRManagement.BuildingBlocks.Models;

namespace HRManagement.Modules.Staff.Features.UpdateRole;

public class UpdateRoleCommand : ICommand<UnitResult<Error>>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int? ReportsToId { get; set; }
}