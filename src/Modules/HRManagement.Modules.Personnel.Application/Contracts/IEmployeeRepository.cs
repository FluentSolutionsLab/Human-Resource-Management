using System;
using HRManagement.Modules.Personnel.Domain;

namespace HRManagement.Modules.Personnel.Application.Contracts;

public interface IEmployeeRepository : IGenericRepository<Employee, Guid>
{
}