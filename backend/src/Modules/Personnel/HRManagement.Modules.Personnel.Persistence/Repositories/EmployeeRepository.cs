using HRManagement.Modules.Personnel.Application.Contracts;
using HRManagement.Modules.Personnel.Domain.Employee;

namespace HRManagement.Modules.Personnel.Persistence.Repositories;

public class EmployeeRepository : GenericRepository<Employee, Guid>, IEmployeeRepository
{
    public EmployeeRepository(PersonnelDbContext dbContext) : base(dbContext)
    {
    }
}