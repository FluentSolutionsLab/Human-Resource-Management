using CSharpFunctionalExtensions;
using HRManagement.Common.Application.Contracts;
using HRManagement.Common.Domain.Models;
using HRManagement.Modules.Staff.Features.GetEmployees;
using HRManagement.Modules.Staff.Features.Services;
using HRManagement.Modules.Staff.Models;

namespace HRManagement.Modules.Staff.Features.CreateEmployee;

public class CreateEmployeeCommandHandler : ICommandHandler<CreateEmployeeCommand, Result<EmployeeDto, Error>>
{
    private readonly ICacheService _cacheService;
    private readonly IEmployeeService _employeeService;
    private readonly IRoleService _roleService;

    public CreateEmployeeCommandHandler(
        ICacheService cacheService,
        IEmployeeService employeeService,
        IRoleService roleService)
    {
        _cacheService = cacheService;
        _employeeService = employeeService;
        _roleService = roleService;
    }

    public async Task<Result<EmployeeDto, Error>> Handle(CreateEmployeeCommand request,
        CancellationToken cancellationToken)
    {
        return await _employeeService.ValidateRequest(request)
            .Ensure(dto => _roleService.CheckIfRoleExists(dto.RoleId),
                DomainErrors.NotFound(nameof(Role), request.RoleId))
            .Map(dto => _roleService.GetRole(dto))
            .Ensure(async dto => await _employeeService.CheckIfEmployeeExists(dto.ManagerId),
                DomainErrors.NotFound(nameof(Employee), request.ReportsToId))
            .Map(dto => _employeeService.GetManager(dto))
            .Ensure(dto => _employeeService.CheckIfEmployeeIsUnique(dto), DomainErrors.ResourceAlreadyExists())
            .Map(_employeeService.CreateEmployee)
            .Tap(async result => await _employeeService.StoreCreatedEmployee(result))
            .Tap(() => _cacheService.RemoveAll(k =>
                k.Contains("GetEmployeesQuery") || k.Contains("GetEmployeeQuery") || k.Contains("GetRoleQuery")))
            .Map(result => result.Value.Employee.ToResponseDto());
    }
}