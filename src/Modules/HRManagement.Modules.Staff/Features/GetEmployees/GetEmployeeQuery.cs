using CSharpFunctionalExtensions;
using HRManagement.Common.Application.Contracts;
using HRManagement.Common.Domain.Models;

namespace HRManagement.Modules.Staff.Features.FindEmployees;

public class GetEmployeeQuery : IQuery<Result<EmployeeDto, Error>>
{
    public string EmployeeId { get; set; }
}