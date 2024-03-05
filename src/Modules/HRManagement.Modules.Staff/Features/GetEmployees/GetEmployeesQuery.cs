using CSharpFunctionalExtensions;
using HRManagement.Common.Application.Contracts;
using HRManagement.Common.Application.Models;

namespace HRManagement.Modules.Staff.Features.GetEmployees;

public class GetEmployeesQuery : IQuery<Result<PagedList<EmployeeDto>>>
{
    public FilterParameters FilterParameters { get; set; }
}