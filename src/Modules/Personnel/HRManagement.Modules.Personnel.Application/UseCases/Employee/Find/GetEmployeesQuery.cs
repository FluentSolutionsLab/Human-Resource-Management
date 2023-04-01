using CSharpFunctionalExtensions;
using HRManagement.Common.Application.Contracts;
using HRManagement.Common.Application.Models;

namespace HRManagement.Modules.Personnel.Application.UseCases;

public class GetEmployeesQuery : IQuery<Result<PagedList<EmployeeDto>>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}