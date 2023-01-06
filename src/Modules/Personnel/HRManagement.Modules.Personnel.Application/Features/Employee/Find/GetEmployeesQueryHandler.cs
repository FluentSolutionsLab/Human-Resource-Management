using CSharpFunctionalExtensions;
using HRManagement.Common.Domain.Models;
using HRManagement.Modules.Personnel.Application.Contracts;
using HRManagement.Modules.Personnel.Application.Contracts.Handlers;
using HRManagement.Modules.Personnel.Application.DTOs;

namespace HRManagement.Modules.Personnel.Application.Features.Employee;

public class GetEmployeesQueryHandler : IQueryHandler<GetEmployeesQuery, Result<List<EmployeeDto>, Error>>
{
    private readonly IEmployeeRepository _repository;

    public GetEmployeesQueryHandler(IEmployeeRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<EmployeeDto>, Error>> Handle(GetEmployeesQuery request,
        CancellationToken cancellationToken)
    {
        var employees = await _repository.GetAsync();

        return employees.Select(EmployeeDto.MapFromEntity).ToList();
    }
}