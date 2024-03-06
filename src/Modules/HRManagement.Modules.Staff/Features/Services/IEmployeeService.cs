using CSharpFunctionalExtensions;
using HRManagement.BuildingBlocks.Models;
using HRManagement.Modules.Staff.Features.Employees.Create;
using HRManagement.Modules.Staff.Features.Employees.Update;

namespace HRManagement.Modules.Staff.Features.Services;

public interface IEmployeeService
{
    Result<EmployeeCreateOrUpdateDto, Error> ValidateRequest(UpdateEmployeeCommand command);
    Result<EmployeeCreateOrUpdateDto, Error> ValidateRequest(CreateEmployeeCommand command);
    Task<bool> CheckIfEmployeeExists(Guid? managerId);
    EmployeeCreateOrUpdateDto GetEmployee(EmployeeCreateOrUpdateDto dto);
    EmployeeCreateOrUpdateDto GetManager(EmployeeCreateOrUpdateDto dto);
    Task<bool> CheckIfEmployeeIsUnique(EmployeeCreateOrUpdateDto dto);
    Result<EmployeeCreateOrUpdateDto, Error> CreateEmployee(EmployeeCreateOrUpdateDto dto);
    Result<EmployeeCreateOrUpdateDto, Error> UpdateEmployee(EmployeeCreateOrUpdateDto dto);
    Task StoreCreatedEmployee(Result<EmployeeCreateOrUpdateDto, Error> result);
    Task StoreUpdatedEmployee(Result<EmployeeCreateOrUpdateDto, Error> result);
}