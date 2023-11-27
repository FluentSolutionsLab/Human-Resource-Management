using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using HRManagement.Modules.Personnel.Domain;

namespace HRManagement.Modules.Personnel.Application.UseCases;

public class HireEmployeeCommandHandler : ICommandHandler<HireEmployeeCommand, Result<EmployeeDto, List<Error>>>
{
    private readonly ICacheService _cacheService;
    private readonly IUnitOfWork _unitOfWork;

    public HireEmployeeCommandHandler(IUnitOfWork unitOfWork, ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<Result<EmployeeDto, List<Error>>> Handle(HireEmployeeCommand request,
        CancellationToken cancellationToken)
    {
        var errors = CheckForErrors(
            request,
            out var nameCreation,
            out var emailCreation,
            out var dateOfBirthCreation,
            out var hiringDateCreation);
        if (errors.Any()) return errors;

        if (!Guid.TryParse(request.ReportsToId, out var reportsToId))
            return new List<Error> {DomainErrors.NotFound(nameof(Employee), request.ReportsToId)};

        Maybe<Role> maybeRole = await _unitOfWork.GetRepository<Role, byte>().GetByIdAsync(request.RoleId);
        if (maybeRole.HasNoValue) return new List<Error> {DomainErrors.NotFound(nameof(Role), request.RoleId)};
        
        //TODO: Need to deal with CEO case

        Maybe<Employee> maybeManager = await _unitOfWork.GetRepository<Employee, Guid>().GetByIdAsync(reportsToId);
        if (maybeManager.HasNoValue)
            return new List<Error> {DomainErrors.NotFound(nameof(Employee), request.ReportsToId)};

        Expression<Func<Employee, bool>> existingEmployeeCondition =
            e => e.Name.FirstName == request.FirstName
                 && e.Name.LastName == request.LastName
                 && e.BirthDate.Date == dateOfBirthCreation.Value.Date;

        var existingEmployees = await _unitOfWork.GetRepository<Employee, Guid>().GetAsync(existingEmployeeCondition);
        if (existingEmployees.Any()) return new List<Error> {DomainErrors.ResourceAlreadyExists()};

        var employeeCreation = Employee.Create(
            nameCreation.Value,
            emailCreation.Value,
            dateOfBirthCreation.Value,
            hiringDateCreation.Value,
            maybeRole.Value,
            maybeManager.Value);
        if (employeeCreation.IsFailure) return new List<Error> {employeeCreation.Error};

        var employee = employeeCreation.Value;
        await _unitOfWork.GetRepository<Employee, Guid>().AddAsync(employee);
        await _unitOfWork.SaveChangesAsync();

        // Clear the memory cache of keys related to the same context
        var cacheKeys = _cacheService.GetAllKeys();
        foreach (var key in cacheKeys.Where(k => k.Contains("GetEmployeesQuery")))
            _cacheService.Remove(key);

        return employee.ToResponseDto();
    }

    private static List<Error> CheckForErrors(
        HireEmployeeCommand request,
        out Result<Name, Error> nameCreation,
        out Result<EmailAddress, Error> emailCreation,
        out Result<ValueDate, Error> dateOfBirthCreation,
        out Result<ValueDate, Error> hiringDateCreation)
    {
        var errors = new List<Error>();

        nameCreation = Name.Create(request.FirstName, request.LastName);
        if (nameCreation.IsFailure) errors.Add(nameCreation.Error);

        emailCreation = EmailAddress.Create(request.EmailAddress);
        if (emailCreation.IsFailure) errors.Add(emailCreation.Error);

        dateOfBirthCreation = ValueDate.Create(request.DateOfBirth);
        if (dateOfBirthCreation.IsFailure) errors.Add(dateOfBirthCreation.Error);

        hiringDateCreation = ValueDate.Create(request.HiringDate);
        if (hiringDateCreation.IsFailure) errors.Add(hiringDateCreation.Error);

        return errors;
    }
}