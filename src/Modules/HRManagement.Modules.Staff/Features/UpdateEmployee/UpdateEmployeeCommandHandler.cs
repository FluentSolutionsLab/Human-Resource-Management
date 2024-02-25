using CSharpFunctionalExtensions;
using HRManagement.Common.Application.Contracts;
using HRManagement.Common.Domain.Models;
using HRManagement.Modules.Staff.Features.Services;
using HRManagement.Modules.Staff.Models;

namespace HRManagement.Modules.Staff.Features.UpdateEmployee;

public class UpdateEmployeeCommandHandler : ICommandHandler<UpdateEmployeeCommand, UnitResult<Error>>
{
    private readonly ICacheService _cacheService;
    private readonly IEmployeeService _employeeService;
    private readonly IRoleService _roleService;

    public UpdateEmployeeCommandHandler(
        ICacheService cacheService,
        IEmployeeService employeeService,
        IRoleService roleService)
    {
        _cacheService = cacheService;
        _employeeService = employeeService;
        _roleService = roleService;
    }

    public async Task<UnitResult<Error>> Handle(UpdateEmployeeCommand request,
        CancellationToken cancellationToken)
    {
        return await _employeeService.ValidateRequest(request)
            .Ensure(dto => _employeeService.CheckIfEmployeeExists(dto.EmployeeId.Value),
                DomainErrors.NotFound(nameof(Employee), request.EmployeeId))
            .Map(dto => _employeeService.GetEmployee(dto))
            .Ensure(dto => _roleService.CheckIfRoleExists(dto.RoleId),
                DomainErrors.NotFound(nameof(Role), request.RoleId))
            .Map(dto => _roleService.GetRole(dto))
            .Ensure(dto => _employeeService.CheckIfEmployeeExists(dto.ManagerId),
                DomainErrors.NotFound(nameof(Employee), request.ReportsToId))
            .Map(dto => _employeeService.GetManager(dto))
            .Map(dto => _employeeService.UpdateEmployee(dto))
            .Tap(async result => await _employeeService.StoreUpdatedEmployee(result))
            .Tap(() => _cacheService.RemoveAll(k =>
                k.Contains("GetEmployeesQuery") || k.Contains("GetEmployeeQuery") || k.Contains("GetRoleQuery")))
            .Map(_ => UnitResult.Success<Error>());
    }
}