﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using HRManagement.Modules.Personnel.Application.Contracts;
using HRManagement.Modules.Personnel.Domain;

namespace HRManagement.Modules.Personnel.Application.UseCases;

public class HireEmployeeCommandHandler : ICommandHandler<HireEmployeeCommand, Result<EmployeeDto, List<Error>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public HireEmployeeCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<EmployeeDto, List<Error>>> Handle(HireEmployeeCommand request, CancellationToken cancellationToken)
    {
        var errors = CheckForErrors(request, out var nameCreation, out var emailCreation, out var dateOfBirthCreation);
        if (errors.Any()) return errors;

        if (!Guid.TryParse(request.ReportsToId, out var reportsToId))
            return new List<Error> {DomainErrors.NotFound(nameof(Employee), request.ReportsToId)};

        Maybe<Role> maybeRole = await _unitOfWork.Roles.GetByIdAsync(request.RoleId);
        if (maybeRole.HasNoValue) return new List<Error>{DomainErrors.NotFound(nameof(Role), request.RoleId)};

        Maybe<Employee> maybeManager = await _unitOfWork.Employees.GetByIdAsync(reportsToId);
        if (maybeManager.HasNoValue) return new List<Error>{DomainErrors.NotFound(nameof(Employee), request.ReportsToId)};

        Expression<Func<Employee, bool>> existingEmployeeCondition =
            e => e.Name.FirstName == request.FirstName
                 && e.Name.LastName == request.LastName
                 && e.DateOfBirth.Date == dateOfBirthCreation.Value.Date;

        var existingEmployees = await _unitOfWork.Employees.GetAsync(existingEmployeeCondition);
        if (existingEmployees.Any()) return new List<Error> {DomainErrors.ResourceAlreadyExists()};

        var employeeCreation = Employee.Create(nameCreation.Value, emailCreation.Value, dateOfBirthCreation.Value, maybeRole.Value, maybeManager.Value);
        if (employeeCreation.IsFailure) return new List<Error>{employeeCreation.Error};

        var employee = employeeCreation.Value;
        await _unitOfWork.Employees.AddAsync(employee);
        await _unitOfWork.SaveChangesAsync();

        return employee.ToResponseDto();
    }

    private List<Error> CheckForErrors(
        HireEmployeeCommand request, 
        out Result<Name, List<Error>> nameCreation, 
        out Result<EmailAddress, List<Error>> emailCreation, 
        out Result<DateOfBirth, List<Error>> dateOfBirthCreation)
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