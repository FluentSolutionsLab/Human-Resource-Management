using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using HRManagement.Modules.Personnel.Application.Contracts;
using HRManagement.Modules.Personnel.Domain;
using Microsoft.Extensions.Caching.Memory;

namespace HRManagement.Modules.Personnel.Application.UseCases;

public class HireEmployeeCommandHandler : ICommandHandler<HireEmployeeCommand, Result<EmployeeDto, List<Error>>>
{
    private readonly IGenericUnitOfWork _unitOfWork;
    private readonly IMemoryCache _cache;

    public HireEmployeeCommandHandler(IGenericUnitOfWork unitOfWork, IMemoryCache cache)
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    public async Task<Result<EmployeeDto, List<Error>>> Handle(HireEmployeeCommand request, CancellationToken cancellationToken)
    {
        var errors = CheckForErrors(request, out var nameCreation, out var emailCreation, out var dateOfBirthCreation);
        if (errors.Any()) return errors;

        if (!Guid.TryParse(request.ReportsToId, out var reportsToId))
            return new List<Error> {DomainErrors.NotFound(nameof(Employee), request.ReportsToId)};

        Maybe<Role> maybeRole = await _unitOfWork.GetRepository<Role, byte>().GetByIdAsync(request.RoleId);
        if (maybeRole.HasNoValue) return new List<Error>{DomainErrors.NotFound(nameof(Role), request.RoleId)};

        Maybe<Employee> maybeManager = await _unitOfWork.GetRepository<Employee, Guid>().GetByIdAsync(reportsToId);
        if (maybeManager.HasNoValue) return new List<Error>{DomainErrors.NotFound(nameof(Employee), request.ReportsToId)};

        Expression<Func<Employee, bool>> existingEmployeeCondition =
            e => e.Name.FirstName == request.FirstName
                 && e.Name.LastName == request.LastName
                 && e.DateOfBirth.Date == dateOfBirthCreation.Value.Date;

        var existingEmployees = await _unitOfWork.GetRepository<Employee, Guid>().GetAsync(existingEmployeeCondition);
        if (existingEmployees.Any()) return new List<Error> {DomainErrors.ResourceAlreadyExists()};

        var employeeCreation = Employee.Create(nameCreation.Value, emailCreation.Value, dateOfBirthCreation.Value, maybeRole.Value, maybeManager.Value);
        if (employeeCreation.IsFailure) return new List<Error>{employeeCreation.Error};

        var employee = employeeCreation.Value;
        await _unitOfWork.GetRepository<Employee, Guid>().AddAsync(employee);
        await _unitOfWork.SaveChangesAsync();

        // Clear the memory cache of keys related to the same context
        var cacheKeys = GetAllKeysList();
        foreach (var key in cacheKeys.Where(k => k.Contains("GetEmployeesQuery"))) 
            _cache.Remove(key);

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
    
    private List<string> GetAllKeysList()
    {
        var coherentState = typeof(MemoryCache).GetField("_coherentState", BindingFlags.NonPublic | BindingFlags.Instance);
        var coherentStateValue = coherentState.GetValue(_cache);
        var entriesCollection = coherentStateValue.GetType().GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);
        var entriesCollectionValue = entriesCollection.GetValue(coherentStateValue) as ICollection;

        if (entriesCollectionValue == null) return default;

        var keys = new List<string>();
        foreach (var item in entriesCollectionValue)
        {
            var methodInfo = item.GetType().GetProperty("Key");
            var val = methodInfo.GetValue(item);
            keys.Add(val.ToString());
        }

        return keys;
    }
}