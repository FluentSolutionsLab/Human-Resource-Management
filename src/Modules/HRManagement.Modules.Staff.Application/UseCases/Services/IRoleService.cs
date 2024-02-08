namespace HRManagement.Modules.Staff.Application.UseCases.Services;

public interface IRoleService
{
    Task<bool> CheckIfRoleExists(byte? roleId);
    EmployeeCreateOrUpdateDto GetRole(EmployeeCreateOrUpdateDto request);
}