using System;
using System.Linq.Expressions;
using System.Text;
using HRManagement.Modules.Personnel.Domain;

namespace HRManagement.Modules.Personnel.Application.UseCases;

public class GetEmployeesQueryHandler : IQueryHandler<GetEmployeesQuery, Result<PagedList<EmployeeDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

    public GetEmployeesQueryHandler(IUnitOfWork unitOfWork, ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<Result<PagedList<EmployeeDto>>> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
    {
        var cacheKeyBuilder = BuildCacheKey(request, out var filter, out var orderBy);
        var employeeListCacheKey = cacheKeyBuilder.ToString();
        var employees = _cacheService.Get<PagedList<Employee>>(employeeListCacheKey);
        if (employees != null)
        {
            return employees.ToResponseDto();
        }
        employees = await _unitOfWork.GetRepository<Employee, Guid>().GetAsync(
            filter: filter,
            pageNumber: request.FilterParameters.PageNumber,
            pageSize: request.FilterParameters.PageSize,
            orderBy: orderBy);
        _cacheService.Set(employeeListCacheKey, employees);

        return employees.ToResponseDto();
    }

    private static StringBuilder BuildCacheKey(
        GetEmployeesQuery request, 
        out Expression<Func<Employee, bool>> filter, 
        out Func<IQueryable<Employee>, IOrderedQueryable<Employee>> orderBy)
    {
        var pageNumber = request.FilterParameters.PageNumber;
        var pageSize = request.FilterParameters.PageSize;
        var cacheKeyBuilder = new StringBuilder();
        cacheKeyBuilder.Append($"GetEmployeesQuery?pageNumber={pageNumber}&pageSize={pageSize}");

        filter = null;
        if (!string.IsNullOrWhiteSpace(request.FilterParameters.SearchQuery))
        {
            var searchQuery = request.FilterParameters.SearchQuery.Trim();
            filter = employee => employee.Name.FirstName.Contains(searchQuery)
                                 || employee.Name.LastName.Contains(searchQuery)
                                 || employee.EmailAddress.Email.Contains(searchQuery)
                                 || employee.Role.Name.Contains(searchQuery);
            cacheKeyBuilder.Append($"&searchQuery={searchQuery}");
        }

        orderBy = queryable =>
            queryable
                .OrderBy(e => e.Name.LastName)
                .ThenBy(e => e.Name.FirstName);

        return cacheKeyBuilder;
    }
}