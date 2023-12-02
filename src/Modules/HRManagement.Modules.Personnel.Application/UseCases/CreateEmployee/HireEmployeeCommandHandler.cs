using System;
using System.Linq.Expressions;
using HRManagement.Modules.Personnel.Domain;

namespace HRManagement.Modules.Personnel.Application.UseCases;

public class HireEmployeeCommandHandler : ICommandHandler<HireEmployeeCommand, Result<EmployeeDto, Error>>
{
    private readonly ICacheService _cacheService;
    private readonly IUnitOfWork _unitOfWork;

    public HireEmployeeCommandHandler(IUnitOfWork unitOfWork, ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<Result<EmployeeDto, Error>> Handle(HireEmployeeCommand request,
        CancellationToken cancellationToken)
    {
        return await ValidateRequest(request)
            .Ensure(dto => CheckIfRoleExists(dto.RoleId), DomainErrors.NotFound(nameof(Role), request.RoleId))
            .Map(GetRole)
            .Ensure(dto => CheckIfManagerExists(dto.ManagerId),
                DomainErrors.NotFound(nameof(Employee), request.ReportsToId))
            .Map(GetManagerRole)
            .Ensure(CheckIfEmployeeIsUnique, DomainErrors.ResourceAlreadyExists())
            .Map(CreateEmployee)
            .Tap(async result =>
            {
                var employee = result.Value.Employee;
                await _unitOfWork.GetRepository<Employee, Guid>().AddAsync(employee);
                await _unitOfWork.SaveChangesAsync();
            })
            .Tap(() => _cacheService.RemoveAll(k => k.Contains("GetEmployeesQuery")))
            .Map(result => result.Value.Employee.ToResponseDto());
    }

    private static Result<HiringDto, Error> ValidateRequest(HireEmployeeCommand request)
    {
        var nameCreation = Name.Create(request.FirstName, request.LastName);
        if (nameCreation.IsFailure) return nameCreation.Error;

        var emailCreation = EmailAddress.Create(request.EmailAddress);
        if (emailCreation.IsFailure) return emailCreation.Error;

        var dateOfBirthCreation = ValueDate.Create(request.DateOfBirth);
        if (dateOfBirthCreation.IsFailure) return dateOfBirthCreation.Error;

        var hiringDateCreation = ValueDate.Create(request.HiringDate);
        if (hiringDateCreation.IsFailure) return hiringDateCreation.Error;

        if (!Guid.TryParse(request.ReportsToId, out var reportsToId))
            return DomainErrors.InvalidInput(nameof(request.ReportsToId));

        return new HiringDto
        {
            Name = nameCreation.Value,
            EmailAddress = emailCreation.Value,
            DateOfBirth = dateOfBirthCreation.Value,
            HiringDate = hiringDateCreation.Value,
            RoleId = request.RoleId,
            ManagerId = reportsToId
        };
    }

    private async Task<bool> CheckIfRoleExists(byte? roleId)
    {
        if (!roleId.HasValue) return true;

        var queryCacheKey = $"GetRoleQuery/{roleId}";
        var roleOrNothing = _cacheService.Get<Maybe<Role>>(queryCacheKey);
        if (roleOrNothing.HasNoValue)
        {
            roleOrNothing =
                await _unitOfWork.GetRepository<Role, byte>().GetByIdAsync(roleId.Value);
            if (roleOrNothing.HasValue)
                _cacheService.Set(queryCacheKey, roleOrNothing);
        }

        return roleOrNothing.HasValue;
    }

    private async Task<bool> CheckIfManagerExists(Guid? managerId)
    {
        if (!managerId.HasValue) return true;

        var queryCacheKey = $"GetRoleQuery/{managerId}";
        var managerOrNothing = _cacheService.Get<Maybe<Employee>>(queryCacheKey);
        if (managerOrNothing.HasNoValue)
        {
            managerOrNothing =
                await _unitOfWork.GetRepository<Employee, Guid>().GetByIdAsync(managerId.Value);
            if (managerOrNothing.HasValue)
                _cacheService.Set(queryCacheKey, managerOrNothing);
        }

        return managerOrNothing.HasValue;
    }

    private HiringDto GetRole(HiringDto request)
    {
        var queryCacheKey = $"GetRoleQuery/{request.RoleId}";
        request.RoleOrNothing = _cacheService.Get<Maybe<Role>>(queryCacheKey).Value;

        return request;
    }

    private HiringDto GetManagerRole(HiringDto request)
    {
        var queryCacheKey = $"GetRoleQuery/{request.ManagerId}";
        request.ManagerOrNothing = _cacheService.Get<Maybe<Employee>>(queryCacheKey).Value;

        return request;
    }

    private async Task<bool> CheckIfEmployeeIsUnique(HiringDto request)
    {
        Expression<Func<Employee, bool>> existingEmployeeCondition =
            e => e.Name.FirstName == request.Name.FirstName
                 && e.Name.LastName == request.Name.LastName
                 && e.BirthDate.Date == request.DateOfBirth.Date;
        var (_, isFailure) = await _unitOfWork.GetRepository<Employee, Guid>().HasMatches(existingEmployeeCondition);

        return isFailure;
    }

    private static Result<HiringDto, Error> CreateEmployee(HiringDto dto)
    {
        var (_, isFailure, employee, error) = Employee.Create(
            dto.Name,
            dto.EmailAddress,
            dto.DateOfBirth,
            dto.HiringDate,
            dto.RoleOrNothing.Value,
            dto.ManagerOrNothing.Value);

        if (isFailure) return error;

        dto.Employee = employee;

        return dto;
    }

    private class HiringDto
    {
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