using CSharpFunctionalExtensions;
using HRManagement.BuildingBlocks.Contracts;
using HRManagement.BuildingBlocks.Models;

namespace HRManagement.Modules.Staff.Features.Employees.Get;

public class GetEmployeesQuery : IQuery<Result<PagedList<EmployeeDto>>>
{
    public FilterParameters FilterParameters { get; set; }
}