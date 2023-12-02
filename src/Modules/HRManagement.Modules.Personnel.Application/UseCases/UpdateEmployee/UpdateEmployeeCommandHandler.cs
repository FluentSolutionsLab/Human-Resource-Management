using System;
using HRManagement.Modules.Personnel.Domain;

namespace HRManagement.Modules.Personnel.Application.UseCases;

public class UpdateEmployeeCommandHandler : ICommandHandler<UpdateEmployeeCommand, UnitResult<Error>>
{
    private readonly ICacheService _cacheService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateEmployeeCommandHandler(IUnitOfWork unitOfWork, ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<UnitResult<Error>> Handle(UpdateEmployeeCommand request,
        CancellationToken cancellationToken)
    {
        return await ValidateRequest(request)
            .Ensure(dto => CheckIfEmployeeExists(dto.EmployeeId), DomainErrors.NotFound(nameof(Employee), request.EmployeeId))
            .Map(GetEmployee)
            .Ensure(dto => CheckIfRoleExists(dto.RoleId), DomainErrors.NotFound(nameof(Role), request.RoleId))
            .Map(GetRole)
            .Ensure(dto => CheckIfEmployeeExists(dto.ManagerId), DomainErrors.NotFound(nameof(Employee), request.ReportsToId))
            .Map(GetManager)
            .Map(UpdateEmployeeData)
            .Tap(async result =>
            {
                var (_, _, dto) = result;
                _unitOfWork.GetRepository<Employee, Guid>().Update(dto.Employee);
                await _unitOfWork.SaveChangesAsync();
            })
            .Tap(() => _cacheService.RemoveAll(k => k.Contains("GetEmployeesQuery")))
            .Map(_ => UnitResult.Success<Error>());
    }

    private static Result<UpdatingDto, Error> ValidateRequest(UpdateEmployeeCommand request)
    {
        var nameCreation = Name.Create(request.FirstName, request.LastName);
        if (nameCreation.IsFailure) return nameCreation.Error;

        var emailCreation = EmailAddress.Create(request.EmailAddress);
        if (emailCreation.IsFailure) return emailCreation.Error;

        var dateOfBirthCreation = ValueDate.Create(request.DateOfBirth);
        if (dateOfBirthCreation.IsFailure) return dateOfBirthCreation.Error;

        var hiringDateCreation = ValueDate.Create(request.HiringDate);
        if (hiringDateCreation.IsFailure) return hiringDateCreation.Error;

        if (!Guid.TryParse(request.EmployeeId, out var employeeId))
            return DomainErrors.NotFound(nameof(Employee), request.EmployeeId);

        if (!Guid.TryParse(request.ReportsToId, out var reportsToId))
            return DomainErrors.NotFound(nameof(Employee), request.ReportsToId);

        return new UpdatingDto
        {
            Name = nameCreation.Value,
            EmailAddress = emailCreation.Value,
            DateOfBirth = dateOfBirthCreation.Value,
            HiringDate = hiringDateCreation.Value,
            RoleId = request.RoleId,
            EmployeeId = employeeId,
            ManagerId = reportsToId
        };
    }

    private async Task<bool> CheckIfEmployeeExists(Guid employeeId)
    {
        var queryCacheKey = $"GetEmployeeQuery/{employeeId}";
        var employeeOrNothing = _cacheService.Get<Maybe<Employee>>(queryCacheKey);
        if (employeeOrNothing.HasNoValue)
        {
            employeeOrNothing = await _unitOfWork.GetRepository<Employee, Guid>().GetByIdAsync(employeeId);
            if (employeeOrNothing.HasValue)
                _cacheService.Set(queryCacheKey, employeeOrNothing);
        }

        return employeeOrNothing.HasValue;
    }

    private UpdatingDto GetEmployee(UpdatingDto request)
    {
        var queryCacheKey = $"GetEmployeeQuery/{request.EmployeeId}";
        request.Employee = _cacheService.Get<Maybe<Employee>>(queryCacheKey).Value;

        return request;
    }

    private UpdatingDto GetManager(UpdatingDto request)
    {
        var queryCacheKey = $"GetEmployeeQuery/{request.EmployeeId}";
        request.ManagerOrNothing = _cacheService.Get<Maybe<Employee>>(queryCacheKey).Value;

        return request;
    }

    private static Result<UpdatingDto, Error> UpdateEmployeeData(UpdatingDto dto)
    {
        var result = dto.Employee.Update(
            dto.Name,
            dto.EmailAddress,
            dto.DateOfBirth,
            dto.HiringDate,
            dto.RoleOrNothing.Value,
            dto.ManagerOrNothing);

        if (result.IsFailure) return result.Error;

        return dto;
    }

    private async Task<bool> CheckIfRoleExists(byte? roleId)
    {
        if (!roleId.HasValue) return true;

        var queryCacheKey = $"GetRoleQuery/{roleId}";
        var roleOrNothing = _cacheService.Get<Maybe<Role>>(queryCacheKey);
        if (roleOrNothing.HasNoValue)
        {
            roleOrNothing = await _unitOfWork.GetRepository<Role, byte>().GetByIdAsync(roleId.Value);
            if (roleOrNothing.HasValue) _cacheService.Set(queryCacheKey, roleOrNothing);
        }

        return roleOrNothing.HasValue;
    }

    private UpdatingDto GetRole(UpdatingDto request)
    {
        var queryCacheKey = $"GetRoleQuery/{request.RoleId}";
        request.RoleOrNothing = _cacheService.Get<Maybe<Role>>(queryCacheKey).Value;

        return request;
    }

    private class UpdatingDto
    {
        public Guid EmployeeId { get; set; }
        public Name Name { get; init; }
        public EmailAddress EmailAddress { get; init; }
        public ValueDate DateOfBirth { get; init; }
        public ValueDate HiringDate { get; init; }
        public byte RoleId { get; init; }
        public Maybe<Role> RoleOrNothing { get; set; } = Maybe<Role>.None;
        public Guid ManagerId { get; init; }
        public Maybe<Employee> ManagerOrNothing { get; set; } = Maybe<Employee>.None;
        public Employee Employee { get; set; }
    }
}