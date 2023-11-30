using HRManagement.Modules.Personnel.Application.UseCases.Services;
using HRManagement.Modules.Personnel.Domain;

namespace HRManagement.Modules.Personnel.Application.UseCases;

public class HireEmployeeCommandHandler : ICommandHandler<HireEmployeeCommand, Result<EmployeeDto, Error>>
{
    private readonly ICacheService _cacheService;
    private readonly IEmployeeService _employeeService;
    private readonly IRoleService _roleService;

    public HireEmployeeCommandHandler(
        ICacheService cacheService,
        IEmployeeService employeeService,
        IRoleService roleService)
    {
        _cacheService = cacheService;
        _employeeService = employeeService;
        _roleService = roleService;
    }

    public async Task<Result<EmployeeDto, Error>> Handle(HireEmployeeCommand request,
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