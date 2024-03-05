using System.Linq.Expressions;
using CSharpFunctionalExtensions;
using HRManagement.BuildingBlocks.Contracts;
using HRManagement.BuildingBlocks.Models;
using HRManagement.Modules.Staff.Features.CreateEmployee;
using HRManagement.Modules.Staff.Features.UpdateEmployee;
using HRManagement.Modules.Staff.Models;
using HRManagement.Modules.Staff.Models.ValueObjects;

namespace HRManagement.Modules.Staff.Features.Services;

public class EmployeeService : IEmployeeService
{
    private readonly ICacheService _cacheService;
    private readonly IUnitOfWork _unitOfWork;

    public EmployeeService(ICacheService cacheService, IUnitOfWork unitOfWork)
    {
        _cacheService = cacheService;
        _unitOfWork = unitOfWork;
    }

    public Result<EmployeeCreateOrUpdateDto, Error> ValidateRequest(UpdateEmployeeCommand command)
    {
        return ValidateRequest(command.FirstName, command.LastName, command.EmailAddress, command.DateOfBirth,
            command.HiringDate, command.ReportsToId, command.RoleId, command.EmployeeId);
    }

    public Result<EmployeeCreateOrUpdateDto, Error> ValidateRequest(CreateEmployeeCommand command)
    {
        return ValidateRequest(command.FirstName, command.LastName, command.EmailAddress, command.DateOfBirth,
            command.HiringDate, command.ReportsToId, command.RoleId);
    }

    public async Task<bool> CheckIfEmployeeExists(Guid? managerId)
    {
        if (!managerId.HasValue) return true;

        var queryCacheKey = $"GetEmployeeQuery/{managerId}";
        var managerOrNothing = _cacheService.Get<Maybe<Employee>>(queryCacheKey);
        if (managerOrNothing.HasNoValue)
        {
            managerOrNothing =
                await _unitOfWork.GetRepository<Employee, Guid>().GetByIdAsync(managerId.Value, "Role");
            if (managerOrNothing.HasValue)
                _cacheService.Set(queryCacheKey, managerOrNothing);
        }

        return managerOrNothing.HasValue;
    }

    public EmployeeCreateOrUpdateDto GetEmployee(EmployeeCreateOrUpdateDto dto)
    {
        var queryCacheKey = $"GetEmployeeQuery/{dto.EmployeeId}";
        dto.Employee = _cacheService.Get<Maybe<Employee>>(queryCacheKey).Value;

        return dto;
    }

    public EmployeeCreateOrUpdateDto GetManager(EmployeeCreateOrUpdateDto dto)
    {
        var queryCacheKey = $"GetEmployeeQuery/{dto.ManagerId}";
        dto.ManagerOrNothing = _cacheService.Get<Maybe<Employee>>(queryCacheKey).Value;

        return dto;
    }

    public async Task<bool> CheckIfEmployeeIsUnique(EmployeeCreateOrUpdateDto dto)
    {
        Expression<Func<Employee, bool>> existingEmployeeCondition =
            e => e.Name.FirstName == dto.Name.FirstName
                 && e.Name.LastName == dto.Name.LastName
                 && e.BirthDate.Date == dto.DateOfBirth.Date;
        var (_, isFailure) = await _unitOfWork.GetRepository<Employee, Guid>().HasMatches(existingEmployeeCondition);

        return isFailure;
    }

    public Result<EmployeeCreateOrUpdateDto, Error> CreateEmployee(EmployeeCreateOrUpdateDto dto)
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

    public Result<EmployeeCreateOrUpdateDto, Error> UpdateEmployee(EmployeeCreateOrUpdateDto dto)
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

    public async Task StoreCreatedEmployee(Result<EmployeeCreateOrUpdateDto, Error> result)
    {
        var employee = result.Value.Employee;
        await _unitOfWork.GetRepository<Employee, Guid>().AddAsync(employee);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task StoreUpdatedEmployee(Result<EmployeeCreateOrUpdateDto, Error> result)
    {
        var (_, _, dto) = result;
        _unitOfWork.GetRepository<Employee, Guid>().Update(dto.Employee);
        await _unitOfWork.SaveChangesAsync();
    }

    private static Result<EmployeeCreateOrUpdateDto, Error> ValidateRequest(
        string firstName, string lastName, string emailAddress, string dateOfBirth, string hiringDate,
        string reportsToId, int roleId, string employeeId = null)
    {
        var nameCreation = Name.Create(firstName, lastName);
        if (nameCreation.IsFailure) return nameCreation.Error;

        var emailCreation = EmailAddress.Create(emailAddress);
        if (emailCreation.IsFailure) return emailCreation.Error;

        var dateOfBirthCreation = ValueDate.Create(dateOfBirth);
        if (dateOfBirthCreation.IsFailure) return dateOfBirthCreation.Error;

        var hiringDateCreation = ValueDate.Create(hiringDate);
        if (hiringDateCreation.IsFailure) return hiringDateCreation.Error;

        if (!Guid.TryParse(reportsToId, out var managerId))
            return DomainErrors.InvalidInput(nameof(reportsToId));

        var id = Guid.Empty;
        if (employeeId != null && !Guid.TryParse(employeeId, out id))
            return DomainErrors.NotFound(nameof(Employee), employeeId);

        return new EmployeeCreateOrUpdateDto
        {
            EmployeeId = id,
            Name = nameCreation.Value,
            EmailAddress = emailCreation.Value,
            DateOfBirth = dateOfBirthCreation.Value,
            HiringDate = hiringDateCreation.Value,
            RoleId = roleId,
            ManagerId = managerId
        };
    }
}