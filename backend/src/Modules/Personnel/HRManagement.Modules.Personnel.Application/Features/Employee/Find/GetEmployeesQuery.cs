using CSharpFunctionalExtensions;
using HRManagement.Common.Domain.Models;
using HRManagement.Modules.Personnel.Application.Contracts.Handlers;
using HRManagement.Modules.Personnel.Application.DTOs;

namespace HRManagement.Modules.Personnel.Application.Features.Employee;

public class GetEmployeesQuery : IQuery<Result<PagedList<EmployeeDto>>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}