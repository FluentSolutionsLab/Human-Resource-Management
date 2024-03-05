namespace HRManagement.Modules.Staff.Features.Services;

public interface IRoleService
{
    Task<bool> CheckIfRoleExists(int? roleId);
    EmployeeCreateOrUpdateDto GetRole(EmployeeCreateOrUpdateDto request);
}