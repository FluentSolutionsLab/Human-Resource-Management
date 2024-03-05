using CSharpFunctionalExtensions;
using HRManagement.BuildingBlocks.Contracts;
using HRManagement.BuildingBlocks.Models;

namespace HRManagement.Modules.Staff.Features.GetEmployees;

public class GetEmployeesQuery : IQuery<Result<PagedList<EmployeeDto>>>
{
    public FilterParameters FilterParameters { get; set; }
}