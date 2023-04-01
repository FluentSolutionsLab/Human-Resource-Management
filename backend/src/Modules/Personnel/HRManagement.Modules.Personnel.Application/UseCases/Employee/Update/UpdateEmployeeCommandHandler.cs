﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using HRManagement.Common.Domain.Models;
using HRManagement.Modules.Personnel.Application.Contracts;
using HRManagement.Modules.Personnel.Application.Contracts.Handlers;
using HRManagement.Modules.Personnel.Domain;

namespace HRManagement.Modules.Personnel.Application.UseCases;

public class UpdateEmployeeCommandHandler : ICommandHandler<UpdateEmployeeCommand, UnitResult<List<Error>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateEmployeeCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UnitResult<List<Error>>> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.EmployeeId, out var employeeId))
            return new List<Error> {DomainErrors.NotFound(nameof(Employee), request.EmployeeId)};

        Maybe<Employee> employeeOrNot = await _unitOfWork.Employees.GetByIdAsync(employeeId);
        if (employeeOrNot.HasNoValue)
            return new List<Error> {DomainErrors.NotFound(nameof(Employee), employeeId)};

        var errors = CheckForErrors(request, out var nameCreation, out var emailCreation, out var dateOfBirthCreation);
        if (errors.Any()) return errors;

        Maybe<Role> maybeRole = await _unitOfWork.Roles.GetByIdAsync(request.RoleId);
        if (maybeRole.HasNoValue) return new List<Error>{DomainErrors.NotFound(nameof(Role), request.RoleId)};

        if (!Guid.TryParse(request.ReportsToId, out var reportsToId))
            return new List<Error> {DomainErrors.NotFound(nameof(Employee), request.ReportsToId)};

        Maybe<Employee> maybeManager = await _unitOfWork.Employees.GetByIdAsync(reportsToId);
        if (maybeManager.HasNoValue) return new List<Error>{DomainErrors.NotFound(nameof(Employee), request.ReportsToId)};

        var employee = employeeOrNot.Value;
        
        var employeeUpdate = employee.Update(nameCreation.Value, emailCreation.Value, dateOfBirthCreation.Value, maybeRole.Value, maybeManager.Value);
        if (employeeUpdate.IsFailure) return new List<Error>{employeeUpdate.Error};
        
        _unitOfWork.Employees.Update(employee);
        await _unitOfWork.SaveChangesAsync();

        return UnitResult.Success<List<Error>>();
    }

    private List<Error> CheckForErrors(UpdateEmployeeCommand request, out Result<Name, List<Error>> nameCreation, out Result<EmailAddress, List<Error>> emailCreation, out Result<DateOfBirth, List<Error>> dateOfBirthCreation)
    {
        var errors = new List<Error>();

        nameCreation = Name.Create(request.FirstName, request.LastName);
        if (nameCreation.IsFailure) errors.AddRange(nameCreation.Error);

        emailCreation = EmailAddress.Create(request.EmailAddress);
        if (emailCreation.IsFailure) errors.AddRange(emailCreation.Error);

        dateOfBirthCreation = DateOfBirth.Create(request.DateOfBirth);
        if (dateOfBirthCreation.IsFailure) errors.AddRange(dateOfBirthCreation.Error);

        return errors;
    }
}