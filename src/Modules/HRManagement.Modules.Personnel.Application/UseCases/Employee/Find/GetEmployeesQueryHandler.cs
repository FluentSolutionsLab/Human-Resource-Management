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
        var array = new[] {nameof(Employee.Role), nameof(Employee.Manager), nameof(Employee.ManagedEmployees)};
        var includedProperties = string.Join(',', array);
        var employees = await _unitOfWork.Employees.GetAsync(
            pageNumber: request.PageNumber,
            pageSize: request.PageSize,
            includeProperties: includedProperties);

        var dtos = employees.Select(x => x.ToResponseDto()).ToList();

        return new PagedList<EmployeeDto>(dtos, employees.TotalCount, request.PageNumber, request.PageSize);
    }
}