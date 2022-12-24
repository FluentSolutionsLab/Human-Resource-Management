using CSharpFunctionalExtensions;
using HRManagement.Common.Domain;
using HRManagement.Modules.Personnel.Application.Contracts.Handlers;
using HRManagement.Modules.Personnel.Application.DTOs;

namespace HRManagement.Modules.Personnel.Application.Features.Employee;

public class GetEmployeesQuery : IQuery<Result<List<EmployeeDto>, Error>>
{
}