﻿using System.Linq.Expressions;
using CSharpFunctionalExtensions;
using HRManagement.Common.Domain.Models;
using HRManagement.Modules.Personnel.Application.Contracts;
using HRManagement.Modules.Personnel.Application.Contracts.Handlers;
using HRManagement.Modules.Personnel.Application.DTOs;
using HRManagement.Modules.Personnel.Domain;
using HRManagement.Modules.Personnel.Domain.Employee;

namespace HRManagement.Modules.Personnel.Application.Features.Employee;

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
            return new List<Error> {DomainErrors.NotFound(nameof(Domain.Employee.Employee), request.ReportsToId)};

        Maybe<Domain.Role.Role> maybeRole = await _unitOfWork.Roles.GetByIdAsync(request.RoleId);
        if (maybeRole.HasNoValue) return new List<Error>{DomainErrors.NotFound(nameof(Domain.Role.Role), request.RoleId)};

        Maybe<Domain.Employee.Employee> maybeManager = await _unitOfWork.Employees.GetByIdAsync(reportsToId);
        if (maybeManager.HasNoValue) return new List<Error>{DomainErrors.NotFound(nameof(Domain.Employee.Employee), request.ReportsToId)};

        Expression<Func<Domain.Employee.Employee, bool>> existingEmployeeCondition =
            e => e.Name.FirstName == request.FirstName
                 && e.Name.LastName == request.LastName
                 && e.DateOfBirth.Date == dateOfBirthCreation.Value.Date;

        var existingEmployees = await _unitOfWork.Employees.GetAsync(existingEmployeeCondition);
        if (existingEmployees.Any()) return new List<Error> {DomainErrors.ResourceAlreadyExists()};

        var employeeCreation = Domain.Employee.Employee.Create(nameCreation.Value, emailCreation.Value, dateOfBirthCreation.Value, maybeRole.Value, maybeManager.Value);
        if (employeeCreation.IsFailure) return new List<Error>{employeeCreation.Error};

        var employee = employeeCreation.Value;
        await _unitOfWork.Employees.AddAsync(employee);
        await _unitOfWork.SaveChangesAsync();

        return EmployeeDto.MapFromEntity(employee);
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