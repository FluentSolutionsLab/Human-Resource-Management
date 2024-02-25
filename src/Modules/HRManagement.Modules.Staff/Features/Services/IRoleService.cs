namespace HRManagement.Modules.Staff.Features.Services;

public interface IRoleService
{
    Task<bool> CheckIfRoleExists(byte? roleId);
    EmployeeCreateOrUpdateDto GetRole(EmployeeCreateOrUpdateDto request);
}