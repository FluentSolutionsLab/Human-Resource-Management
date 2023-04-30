﻿using System;
using System.Linq.Expressions;
using System.Text;
using HRManagement.Modules.Personnel.Application.Contracts;
using HRManagement.Modules.Personnel.Domain;
using Microsoft.Extensions.Caching.Memory;

namespace HRManagement.Modules.Personnel.Application.UseCases;

public class GetEmployeesQueryHandler : IQueryHandler<GetEmployeesQuery, Result<PagedList<EmployeeDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMemoryCache _cache;

    public GetEmployeesQueryHandler(IUnitOfWork unitOfWork, IMemoryCache cache)
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    public async Task<Result<PagedList<EmployeeDto>>> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = request.FilterParameters.PageNumber;
        var pageSize = request.FilterParameters.PageSize;

        var sb = new StringBuilder();
        sb.Append($"employeeList?pageNumber={pageNumber}&pageSize={pageSize}");
        
        Expression<Func<Employee, bool>> filter = null;
        if (!string.IsNullOrWhiteSpace(request.FilterParameters.SearchQuery))
        {
            var searchQuery = request.FilterParameters.SearchQuery.Trim();
            filter = employee => employee.Name.FirstName.Contains(searchQuery)
                                 || employee.Name.LastName.Contains(searchQuery)
                                 || employee.EmailAddress.Email.Contains(searchQuery)
                                 || employee.Role.Name.Contains(searchQuery);
            sb.Append($"&searchQuery={searchQuery}");
        }

        var employeeListCacheKey = sb.ToString();
        if (!_cache.TryGetValue(employeeListCacheKey, out PagedList<Employee> employees))
        {
            employees = await _unitOfWork.Employees.GetAsync(
                filter: filter,
                pageNumber: pageNumber,
                pageSize: pageSize);

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(60))
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(3600))
                .SetPriority(CacheItemPriority.Normal)
                .SetSize(1024);
            _cache.Set(employeeListCacheKey, employees, cacheEntryOptions);
        }

        var dtos = employees.Select(x => x.ToResponseDto()).ToList();

        return new PagedList<EmployeeDto>(dtos, employees.TotalCount, pageNumber, pageSize);
    }
}