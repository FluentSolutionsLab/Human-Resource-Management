using CSharpFunctionalExtensions;
using HRManagement.Common.Domain.Models;
using HRManagement.Modules.Personnel.Application.Contracts.Handlers;
using MediatR;

namespace HRManagement.Modules.Personnel.Application.Features.Employee;

public class UpdateEmployeeCommand : ICommand<Result<Unit, List<Error>>>
{
    public string EmployeeId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public string DateOfBirth { get; set; }
}