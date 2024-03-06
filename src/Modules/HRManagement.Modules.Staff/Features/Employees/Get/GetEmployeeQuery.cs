using CSharpFunctionalExtensions;
using HRManagement.BuildingBlocks.Contracts;
using HRManagement.BuildingBlocks.Models;

namespace HRManagement.Modules.Staff.Features.Employees.Get;

public class GetEmployeeQuery : IQuery<Result<EmployeeDto, Error>>
{
    public string EmployeeId { get; set; }
}