using HRManagement.Common.Application.Contracts;
using HRManagement.Modules.Personnel.Domain;

namespace HRManagement.Modules.Personnel.Application.Contracts;

public interface IRoleRepository : IGenericRepository<Role, byte>
{
}