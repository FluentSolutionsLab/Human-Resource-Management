using HRManagement.Modules.Personnel.Application.Contracts;
using HRManagement.Modules.Personnel.Domain;

namespace HRManagement.Modules.Personnel.Persistence.Repositories;

public class RoleRepository : GenericRepository<Role, byte>, IRoleRepository
{
    public RoleRepository(PersonnelDbContext dbContext) : base(dbContext)
    {
    }
}