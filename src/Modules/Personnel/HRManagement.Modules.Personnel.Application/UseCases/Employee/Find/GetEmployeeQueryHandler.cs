using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using HRManagement.Common.Application.Handlers;
using HRManagement.Common.Domain.Models;
using HRManagement.Modules.Personnel.Application.Contracts;
using HRManagement.Modules.Personnel.Domain;

namespace HRManagement.Modules.Personnel.Application.UseCases;

public class GetEmployeeQueryHandler : IQueryHandler<GetEmployeeQuery, Result<EmployeeDto, Error>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetEmployeeQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<EmployeeDto, Error>> Handle(GetEmployeeQuery request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.EmployeeId, out var employeeId))
            return DomainErrors.NotFound(nameof(Employee), request.EmployeeId);

        Maybe<Employee> employee = await _unitOfWork.Employees.GetByIdAsync(employeeId);
        if (employee.HasNoValue)
            return DomainErrors.NotFound(nameof(Employee), request.EmployeeId);

        return employee.Value.ToResponseDto();
    }
}