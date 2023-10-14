using System;
using HRManagement.Modules.Personnel.Domain;

namespace HRManagement.Modules.Personnel.Application.UseCases;

public class GetEmployeeQueryHandler : IQueryHandler<GetEmployeeQuery, Result<EmployeeDto, Error>>
{
    private readonly ICacheService _cacheService;
    private readonly IUnitOfWork _unitOfWork;

    public GetEmployeeQueryHandler(IUnitOfWork unitOfWork, ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<Result<EmployeeDto, Error>> Handle(GetEmployeeQuery request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.EmployeeId, out var employeeId))
            return DomainErrors.NotFound(nameof(Employee), request.EmployeeId);

        var queryCacheKey = $"GetEmployeeQuery/{employeeId}";
        var employee = _cacheService.Get<Employee>(queryCacheKey);
        if (employee != null)
            return employee.ToResponseDto();

        employee = await _unitOfWork.GetRepository<Employee, Guid>()
            .GetByIdAsync(employeeId, "Role,Manager,Manager.Role");
        if (employee == null)
            return DomainErrors.NotFound(nameof(Employee), request.EmployeeId);

        _cacheService.Set(queryCacheKey, employee);

        return employee.ToResponseDto();
    }
}