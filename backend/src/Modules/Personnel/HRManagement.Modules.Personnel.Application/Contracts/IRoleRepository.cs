using HRManagement.Modules.Personnel.Domain.Role;

namespace HRManagement.Modules.Personnel.Application.Contracts;

public interface IRoleRepository : IGenericRepository<Role, byte>
{
}