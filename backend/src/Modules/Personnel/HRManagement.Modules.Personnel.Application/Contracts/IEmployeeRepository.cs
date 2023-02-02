using HRManagement.Modules.Personnel.Domain.Employee;

namespace HRManagement.Modules.Personnel.Application.Contracts;

public interface IEmployeeRepository : IGenericRepository<Employee, Guid>
{
}