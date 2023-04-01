using HRManagement.Modules.Personnel.Application.Contracts;

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
        var employees = await _unitOfWork.Employees.GetAsync(pageNumber: request.PageNumber, pageSize: request.PageSize);

        var dtos = employees.Select(x => x.ToResponseDto()).ToList();
        
        return new PagedList<EmployeeDto>(dtos, employees.TotalCount, request.PageNumber, request.PageSize);
    }
}