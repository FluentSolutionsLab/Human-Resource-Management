using CSharpFunctionalExtensions;
using HRManagement.Common.Domain.Models;
using HRManagement.Modules.Staff.Features.CreateEmployee;
using HRManagement.Modules.Staff.Features.UpdateEmployee;

namespace HRManagement.Modules.Staff.Features.Services;

public interface IEmployeeService
{
    Result<EmployeeCreateOrUpdateDto, Error> ValidateRequest(UpdateEmployeeCommand command);
    Result<EmployeeCreateOrUpdateDto, Error> ValidateRequest(HireEmployeeCommand command);
    Task<bool> CheckIfEmployeeExists(Guid? managerId);
    EmployeeCreateOrUpdateDto GetEmployee(EmployeeCreateOrUpdateDto dto);
    EmployeeCreateOrUpdateDto GetManager(EmployeeCreateOrUpdateDto dto);
    Task<bool> CheckIfEmployeeIsUnique(EmployeeCreateOrUpdateDto dto);
    Result<EmployeeCreateOrUpdateDto, Error> CreateEmployee(EmployeeCreateOrUpdateDto dto);
    Result<EmployeeCreateOrUpdateDto, Error> UpdateEmployee(EmployeeCreateOrUpdateDto dto);
    Task StoreCreatedEmployee(Result<EmployeeCreateOrUpdateDto, Error> result);
    Task StoreUpdatedEmployee(Result<EmployeeCreateOrUpdateDto, Error> result);
}