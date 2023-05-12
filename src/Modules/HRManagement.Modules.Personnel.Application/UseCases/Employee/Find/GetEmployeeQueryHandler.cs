using System;
using HRManagement.Modules.Personnel.Application.Contracts;
using HRManagement.Modules.Personnel.Domain;
using Microsoft.Extensions.Caching.Memory;

namespace HRManagement.Modules.Personnel.Application.UseCases;

public class GetEmployeeQueryHandler : IQueryHandler<GetEmployeeQuery, Result<EmployeeDto, Error>>
{
    private readonly IGenericUnitOfWork _unitOfWork;
    private readonly IMemoryCache _cache;

    public GetEmployeeQueryHandler(IGenericUnitOfWork unitOfWork, IMemoryCache cache)
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    public async Task<Result<EmployeeDto, Error>> Handle(GetEmployeeQuery request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.EmployeeId, out var employeeId))
            return DomainErrors.NotFound(nameof(Employee), request.EmployeeId);

        var queryCacheKey = $"GetEmployeeQuery/{employeeId}";
        if (!_cache.TryGetValue(queryCacheKey, out Employee employee))
        {
            employee = await _unitOfWork.GetRepository<Employee, Guid>().GetByIdAsync(employeeId);
            if (employee == null)
                return DomainErrors.NotFound(nameof(Employee), request.EmployeeId);

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(60))
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(3600))
                .SetPriority(CacheItemPriority.Normal)
                .SetSize(1024);
            _cache.Set(queryCacheKey, employee, cacheEntryOptions);
        }
        
        return employee.ToResponseDto();
    }
}