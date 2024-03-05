using CSharpFunctionalExtensions;
using HRManagement.BuildingBlocks.Contracts;
using HRManagement.BuildingBlocks.Models;

namespace HRManagement.Modules.Staff.Features.TerminateEmployee;

public class HardDeleteEmployeeCommand : ICommand<UnitResult<Error>>
{
    public string EmployeeId { get; set; }
}