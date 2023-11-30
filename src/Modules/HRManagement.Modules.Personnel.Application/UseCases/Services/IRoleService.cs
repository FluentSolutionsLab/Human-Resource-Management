namespace HRManagement.Modules.Personnel.Application.UseCases.Services;

public interface IRoleService
{
    Task<bool> CheckIfRoleExists(byte? roleId);
    EmployeeCreateOrUpdateDto GetRole(EmployeeCreateOrUpdateDto request);
}