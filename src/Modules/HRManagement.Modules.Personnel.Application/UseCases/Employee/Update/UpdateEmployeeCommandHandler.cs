using System;
using System.Collections.Generic;
using HRManagement.Modules.Personnel.Domain;

namespace HRManagement.Modules.Personnel.Application.UseCases;

public class UpdateEmployeeCommandHandler : ICommandHandler<UpdateEmployeeCommand, UnitResult<List<Error>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

    public UpdateEmployeeCommandHandler(IUnitOfWork unitOfWork, ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<UnitResult<List<Error>>> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.EmployeeId, out var employeeId))
            return new List<Error> {DomainErrors.NotFound(nameof(Employee), request.EmployeeId)};

        Maybe<Employee> employeeOrNot = await _unitOfWork.GetRepository<Employee, Guid>().GetByIdAsync(employeeId);
        if (employeeOrNot.HasNoValue)
            return new List<Error> {DomainErrors.NotFound(nameof(Employee), employeeId)};

        var errors = CheckForErrors(
            request,
            out var nameCreation,
            out var emailCreation,
            out var dateOfBirthCreation,
            out var hiringDateCreation);
        if (errors.Any()) return errors;

        Maybe<Role> maybeRole = await _unitOfWork.GetRepository<Role, byte>().GetByIdAsync(request.RoleId);
        if (maybeRole.HasNoValue) return new List<Error>{DomainErrors.NotFound(nameof(Role), request.RoleId)};

        if (!Guid.TryParse(request.ReportsToId, out var reportsToId))
            return new List<Error> {DomainErrors.NotFound(nameof(Employee), request.ReportsToId)};

        Maybe<Employee> maybeManager = await _unitOfWork.GetRepository<Employee, Guid>().GetByIdAsync(reportsToId);
        if (maybeManager.HasNoValue) return new List<Error>{DomainErrors.NotFound(nameof(Employee), request.ReportsToId)};

        var employee = employeeOrNot.Value;
        
        var employeeUpdate = employee.Update(
            nameCreation.Value,
            emailCreation.Value,
            dateOfBirthCreation.Value,
            hiringDateCreation.Value,
            maybeRole.Value,
            maybeManager.Value);
        if (employeeUpdate.IsFailure) return new List<Error>{employeeUpdate.Error};
        
        _unitOfWork.GetRepository<Employee, Guid>().Update(employee);
        await _unitOfWork.SaveChangesAsync();
        
        _cacheService.Remove($"GetEmployeeQuery/{employeeId}");

        return UnitResult.Success<List<Error>>();
    }

    private List<Error> CheckForErrors(
        UpdateEmployeeCommand request,
        out Result<Name, List<Error>> nameCreation,
        out Result<EmailAddress, List<Error>> emailCreation,
        out Result<ValueDate, List<Error>> dateOfBirthCreation,
        out Result<ValueDate, List<Error>> hiringDateCreation
        )
    {
        var errors = new List<Error>();

        nameCreation = Name.Create(request.FirstName, request.LastName);
        if (nameCreation.IsFailure) errors.AddRange(nameCreation.Error);

        emailCreation = EmailAddress.Create(request.EmailAddress);
        if (emailCreation.IsFailure) errors.AddRange(emailCreation.Error);

        dateOfBirthCreation = ValueDate.Create(request.DateOfBirth);
        if (dateOfBirthCreation.IsFailure) errors.AddRange(dateOfBirthCreation.Error);

        hiringDateCreation = ValueDate.Create(request.HiringDate);
        if (hiringDateCreation.IsFailure) errors.AddRange(hiringDateCreation.Error);

        return errors;
    }
}