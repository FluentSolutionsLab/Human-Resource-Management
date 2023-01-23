namespace HRManagement.Modules.Personnel.Application.Contracts;

public interface IUnitOfWork
{
    IRoleRepository Roles { get; }
    IEmployeeRepository Employees { get; }
    Task<bool> SaveChangesAsync();
}