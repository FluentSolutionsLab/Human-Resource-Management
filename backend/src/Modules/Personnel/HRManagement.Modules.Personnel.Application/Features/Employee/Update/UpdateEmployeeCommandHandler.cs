using CSharpFunctionalExtensions;
using HRManagement.Common.Domain.Models;
using HRManagement.Modules.Personnel.Application.Contracts;
using HRManagement.Modules.Personnel.Application.Contracts.Handlers;
using HRManagement.Modules.Personnel.Domain;
using HRManagement.Modules.Personnel.Domain.Employee;
using MediatR;

namespace HRManagement.Modules.Personnel.Application.Features.Employee;

public class UpdateEmployeeCommandHandler : ICommandHandler<UpdateEmployeeCommand, Result<Unit, List<Error>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateEmployeeCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Unit, List<Error>>> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.EmployeeId, out var employeeId))
            return new List<Error> {DomainErrors.NotFound(nameof(Domain.Employee.Employee), request.EmployeeId)};

        Maybe<Domain.Employee.Employee> employeeOrNot = await _unitOfWork.Employees.GetByIdAsync(employeeId);
        if (employeeOrNot.HasNoValue)
            return new List<Error> {DomainErrors.NotFound(nameof(Domain.Employee.Employee), employeeId)};

        var errors = CheckForErrors(request, out var nameCreation, out var emailCreation, out var dateOfBirthCreation);
        if (errors.Any()) return errors;

        Maybe<Domain.Role.Role> maybeRole = await _unitOfWork.Roles.GetByIdAsync(request.RoleId);
        if (maybeRole.HasNoValue) return new List<Error>{DomainErrors.NotFound(nameof(Domain.Role.Role), request.RoleId)};

        if (!Guid.TryParse(request.ReportsToId, out var reportsToId))
            return new List<Error> {DomainErrors.NotFound(nameof(Domain.Employee.Employee), request.ReportsToId)};

        Maybe<Domain.Employee.Employee> maybeManager = await _unitOfWork.Employees.GetByIdAsync(reportsToId);
        if (maybeManager.HasNoValue) return new List<Error>{DomainErrors.NotFound(nameof(Domain.Employee.Employee), request.ReportsToId)};

        var employee = employeeOrNot.Value;
        
        var employeeUpdate = employee.Update(nameCreation.Value, emailCreation.Value, dateOfBirthCreation.Value, maybeRole.Value, maybeManager.Value);
        if (employeeUpdate.IsFailure) return new List<Error>{employeeUpdate.Error};
        
        _unitOfWork.Employees.Update(employee);
        await _unitOfWork.SaveChangesAsync();

        return Unit.Value;
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