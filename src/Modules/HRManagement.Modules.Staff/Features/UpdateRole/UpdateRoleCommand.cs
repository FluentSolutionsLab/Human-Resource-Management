using CSharpFunctionalExtensions;
using HRManagement.Common.Application.Contracts;
using HRManagement.Common.Domain.Models;

namespace HRManagement.Modules.Staff.Features.UpdateRole;

public class UpdateRoleCommand : ICommand<UnitResult<Error>>
{
    public byte Id { get; set; }
    public string Name { get; set; }
    public byte? ReportsToId { get; set; }
}