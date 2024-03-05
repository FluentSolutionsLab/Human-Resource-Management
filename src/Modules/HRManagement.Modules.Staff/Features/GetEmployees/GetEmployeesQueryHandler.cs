using System.Linq.Expressions;
using System.Text;
using CSharpFunctionalExtensions;
using HRManagement.BuildingBlocks.Contracts;
using HRManagement.BuildingBlocks.Models;
using HRManagement.Modules.Staff.Models;

namespace HRManagement.Modules.Staff.Features.GetEmployees;

public class GetEmployeesQueryHandler : IQueryHandler<GetEmployeesQuery, Result<PagedList<EmployeeDto>>>
{
    private readonly ICacheService _cacheService;
    private readonly IUnitOfWork _unitOfWork;

    public GetEmployeesQueryHandler(IUnitOfWork unitOfWork, ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<Result<PagedList<EmployeeDto>>> Handle(GetEmployeesQuery request,
        CancellationToken cancellationToken)
    {
        var queryData = ProcessRequestData(request);
        var employees = _cacheService.Get<Maybe<PagedList<Employee>>>(queryData.CacheKey);
        if (employees.HasValue) return employees.Value.ToResponseDto();
        employees = await _unitOfWork.GetRepository<Employee, Guid>().GetAsync(
            queryData.Filter,
            pageNumber: request.FilterParameters.PageNumber,
            pageSize: request.FilterParameters.PageSize,
            orderBy: queryData.OrderBy,
            includeProperties: "Role,Manager,Manager.Role");
        _cacheService.Set(queryData.CacheKey, employees);

        return employees.Value.ToResponseDto();
    }

    private static QueryData ProcessRequestData(GetEmployeesQuery request)
    {
        var pageNumber = request.FilterParameters.PageNumber;
        var pageSize = request.FilterParameters.PageSize;
        var cacheKeyBuilder = new StringBuilder();
        cacheKeyBuilder.Append($"GetEmployeesQuery?pageNumber={pageNumber}&pageSize={pageSize}");

        Expression<Func<Employee, bool>> filter = null;
        if (!string.IsNullOrWhiteSpace(request.FilterParameters.KeyWord))
        {
            var searchQuery = request.FilterParameters.KeyWord.Trim();
            filter = employee => employee.Name.FirstName.Contains(searchQuery)
                                 || employee.Name.LastName.Contains(searchQuery)
                                 || employee.EmailAddress.Email.Contains(searchQuery)
                                 || employee.Role.Name.Value.Contains(searchQuery);
            cacheKeyBuilder.Append($"&searchQuery={searchQuery}");
        }

        Func<IQueryable<Employee>, IOrderedQueryable<Employee>> orderBy = queryable =>
            queryable
                .OrderBy(e => e.Name.LastName)
                .ThenBy(e => e.Name.FirstName);

        return new QueryData(cacheKeyBuilder.ToString(), filter, orderBy);
    }

    private record QueryData(
        string CacheKey,
        Expression<Func<Employee, bool>> Filter,
        Func<IQueryable<Employee>, IOrderedQueryable<Employee>> OrderBy);
}