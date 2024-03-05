using CSharpFunctionalExtensions;
using HRManagement.Common.Application.Contracts;
using HRManagement.Common.Domain.Models;
using HRManagement.Modules.Staff.Models;

namespace HRManagement.Modules.Staff.Features.GetEmployees;

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
        var employeeOrNothing = _cacheService.Get<Maybe<Employee>>(queryCacheKey);
        if (employeeOrNothing.HasValue)
            return employeeOrNothing.Value.ToResponseDto();

        employeeOrNothing = await _unitOfWork.GetRepository<Employee, Guid>()
            .GetByIdAsync(employeeId, "Role,Manager,Manager.Role");
        if (employeeOrNothing.HasNoValue)
            return DomainErrors.NotFound(nameof(Employee), request.EmployeeId);

        _cacheService.Set(queryCacheKey, employeeOrNothing);

        return employeeOrNothing.Value.ToResponseDto();
    }
}