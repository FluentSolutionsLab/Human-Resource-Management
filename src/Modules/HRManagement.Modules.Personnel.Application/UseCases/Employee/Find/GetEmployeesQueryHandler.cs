using System;
using System.Linq.Expressions;
using HRManagement.Modules.Personnel.Application.Contracts;
using HRManagement.Modules.Personnel.Domain;

namespace HRManagement.Modules.Personnel.Application.UseCases;

public class GetEmployeesQueryHandler : IQueryHandler<GetEmployeesQuery, Result<PagedList<EmployeeDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetEmployeesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PagedList<EmployeeDto>>> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = request.FilterParameters.PageNumber;
        var pageSize = request.FilterParameters.PageSize;

        Expression<Func<Employee, bool>> filter = null;
        if (!string.IsNullOrWhiteSpace(request.FilterParameters.SearchQuery))
        {
            var searchQuery = request.FilterParameters.SearchQuery.Trim();
            filter = employee => employee.Name.FirstName.Contains(searchQuery)
                                 || employee.Name.LastName.Contains(searchQuery)
                                 || employee.EmailAddress.Email.Contains(searchQuery)
                                 || employee.Role.Name.Contains(searchQuery);
        }

        var employees = await _unitOfWork.Employees.GetAsync(
            filter: filter,
            pageNumber: pageNumber,
            pageSize: pageSize);

        var dtos = employees.Select(x => x.ToResponseDto()).ToList();

        return new PagedList<EmployeeDto>(dtos, employees.TotalCount, pageNumber, pageSize);
    }
}