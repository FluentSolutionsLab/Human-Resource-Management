﻿using System;
using HRManagement.Modules.Personnel.Application.Contracts;
using HRManagement.Modules.Personnel.Domain;

namespace HRManagement.Modules.Personnel.Application.UseCases;

public class TerminateEmployeeCommandHandler : ICommandHandler<TerminateEmployeeCommand, UnitResult<Error>>
{
    private readonly IUnitOfWork _unitOfWork;

    public TerminateEmployeeCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UnitResult<Error>> Handle(TerminateEmployeeCommand request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.EmployeeId, out var employeeId))
            return DomainErrors.NotFound(nameof(Employee), request.EmployeeId);

        Maybe<Employee> employeeOrNot = await _unitOfWork.GetRepository<Employee, Guid>().GetByIdAsync(employeeId);
        if (employeeOrNot.HasNoValue) return DomainErrors.NotFound(nameof(Employee), employeeId);

        var employee = employeeOrNot.Value;
        employee.Terminate();
        _unitOfWork.GetRepository<Employee, Guid>().Update(employee);
        await _unitOfWork.SaveChangesAsync();

        return UnitResult.Success<Error>();
    }
}