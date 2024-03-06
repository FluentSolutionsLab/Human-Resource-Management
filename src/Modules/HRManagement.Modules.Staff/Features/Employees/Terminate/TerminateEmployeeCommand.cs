using CSharpFunctionalExtensions;
using HRManagement.BuildingBlocks.Contracts;
using HRManagement.BuildingBlocks.Models;

namespace HRManagement.Modules.Staff.Features.Employees.Terminate;

public class TerminateEmployeeCommand : ICommand<UnitResult<Error>>
{
    public string EmployeeId { get; set; }
    public string TerminationDate { get; set; }
}