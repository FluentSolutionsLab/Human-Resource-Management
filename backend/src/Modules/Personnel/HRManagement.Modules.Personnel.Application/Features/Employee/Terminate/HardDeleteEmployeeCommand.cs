using CSharpFunctionalExtensions;
using HRManagement.Common.Domain.Models;
using HRManagement.Modules.Personnel.Application.Contracts.Handlers;
using MediatR;

namespace HRManagement.Modules.Personnel.Application.Features.Employee;

public class HardDeleteEmployeeCommand : ICommand<Result<Unit, Error>>
{
    public string EmployeeId { get; set; }
}