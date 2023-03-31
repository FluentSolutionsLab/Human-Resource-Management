using CSharpFunctionalExtensions;
using HRManagement.Common.Domain.Models;
using HRManagement.Modules.Personnel.Application.Contracts.Handlers;
using MediatR;

namespace HRManagement.Modules.Personnel.Application.UseCases;

public class TerminateEmployeeCommand : ICommand<UnitResult<Error>>
{
    public string EmployeeId { get; set; }
}