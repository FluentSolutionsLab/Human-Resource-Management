using System.Linq.Expressions;
using AutoMapper;
using CSharpFunctionalExtensions;
using HRManagement.Modules.Personnel.Application.Contracts;
using HRManagement.Modules.Personnel.Application.Contracts.Handlers;
using HRManagement.Modules.Personnel.Application.DTOs;
using HRManagement.Modules.Personnel.Domain;
using HRManagement.Modules.Personnel.Domain.Employee;

namespace HRManagement.Modules.Personnel.Application.Features.Employee;

public class HireEmployeeCommandHandler : ICommandHandler<HireEmployeeCommand, Result<EmployeeDto, List<Error>>>
{
    private readonly IMapper _mapper;
    private readonly IEmployeeRepository _repository;

    public HireEmployeeCommandHandler(IMapper mapper, IEmployeeRepository repository)
    {
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<Result<EmployeeDto, List<Error>>> Handle(HireEmployeeCommand request, CancellationToken cancellationToken)
    {
        var errors = CheckForErrors(request, out var nameCreation, out var emailCreation, out var dateOfBirthCreation);
        if (errors.Any()) return errors;

        var actualDate = DateOnly.FromDateTime(DateTime.Parse(request.DateOfBirth));
        Expression<Func<Domain.Employee.Employee, bool>> existingEmployeeCondition =
            e => e.Name.FirstName == request.FirstName
                 && e.Name.LastName == request.LastName
                 && e.DateOfBirth.Date == actualDate;

        var existingEmployees = await _repository.GetAsync(existingEmployeeCondition);
        if (existingEmployees.Any()) return new List<Error> {DomainErrors.ResourceAlreadyExists()};

        var employeeCreation = Domain.Employee.Employee.Create(nameCreation.Value, emailCreation.Value, dateOfBirthCreation.Value);
        if (employeeCreation.IsFailure) return new List<Error> {employeeCreation.Error};

        var employee = employeeCreation.Value;
        await _repository.AddAsync(employee);
        await _repository.CommitAsync();

        return _mapper.Map<EmployeeDto>(employee);
    }

    private List<Error> CheckForErrors(HireEmployeeCommand request, out Result<Name, List<Error>> nameCreation,
        out Result<EmailAddress, List<Error>> emailCreation, out Result<DateOfBirth, List<Error>> dateOfBirthCreation)
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