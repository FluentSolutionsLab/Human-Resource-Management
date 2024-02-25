using CSharpFunctionalExtensions;
using HRManagement.Common.Application.Contracts;
using HRManagement.Common.Domain.Models;

namespace HRManagement.Modules.Staff.Features.TerminateEmployee;

public class TerminateEmployeeCommand : ICommand<UnitResult<Error>>
{
    public string EmployeeId { get; set; }
    public string TerminationDate { get; set; }
}