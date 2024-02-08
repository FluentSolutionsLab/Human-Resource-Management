using System;

namespace HRManagement.Modules.Staff.Application.UseCases.Services;

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