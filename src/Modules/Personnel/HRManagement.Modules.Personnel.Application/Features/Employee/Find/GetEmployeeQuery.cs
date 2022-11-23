using CSharpFunctionalExtensions;
using HRManagement.Modules.Personnel.Application.Contracts.Handlers;
using HRManagement.Modules.Personnel.Application.DTOs;
using HRManagement.Modules.Personnel.Domain;

namespace HRManagement.Modules.Personnel.Application.Features.Employee;

public class GetEmployeeQuery : IQuery<Result<EmployeeDto, Error>>
{
    public string EmployeeId { get; set; }
}