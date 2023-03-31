using CSharpFunctionalExtensions;
using HRManagement.Common.Domain.Models;
using HRManagement.Modules.Personnel.Application.Contracts.Handlers;

namespace HRManagement.Modules.Personnel.Application.UseCases;

public class GetEmployeesQuery : IQuery<Result<PagedList<EmployeeDto>>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}