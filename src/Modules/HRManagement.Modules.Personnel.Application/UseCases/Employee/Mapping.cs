using HRManagement.Common.Application.Utilities;
using HRManagement.Modules.Personnel.Domain;

namespace HRManagement.Modules.Personnel.Application.UseCases;

public static partial class Mapping
{
    public static EmployeeDto ToResponseDto(this Employee employee)
    {
        return new EmployeeDto
        {
            Id = employee.Id.ToString(),
            FirstName = employee.Name.FirstName,
            LastName = employee.Name.LastName,
            EmailAddress = employee.EmailAddress.Email,
            DateOfBirth = employee.DateOfBirth.Date.ToISO8601String(),
            HireDate = employee.HireDate.ToISO8601String(),
            Role = employee.Role.ToString(),
            Manager = GetManager(employee)
        };
    }

    public static EmployeeExtendedDto ToResponseExtendedDto(this Employee employee)
    {
        var managedEmployees = employee
            .ManagedEmployees
            .OrderBy(e => e.Name.LastName)
            .ThenBy(e => e.Name.LastName)
            .Select(e => new ManagedEmployeeDto
            {
                Id = e.Id.ToString(),
                FirstName = e.Name.FirstName,
                LastName = e.Name.LastName,
                Role = e.Role.ToString()
            }).ToList();

        return new EmployeeExtendedDto
        {
            Id = employee.Id.ToString(),
            FirstName = employee.Name.FirstName,
            LastName = employee.Name.LastName,
            EmailAddress = employee.EmailAddress.Email,
            DateOfBirth = employee.DateOfBirth.Date.ToISO8601String(),
            HireDate = employee.HireDate.ToISO8601String(),
            Role = employee.Role.ToString(),
            Manager = GetManager(employee),
            ManagedEmployees = managedEmployees
        };
    }

    public static HireEmployeeCommand ToHireEmployeeCommand(this HireEmployeeDto dto)
    {
        return new HireEmployeeCommand
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            EmailAddress = dto.EmailAddress,
            DateOfBirth = dto.DateOfBirth,
            RoleId = dto.RoleId,
            ReportsToId = dto.ManagerId
        };
    }

    public static UpdateEmployeeCommand ToUpdateEmployeeCommand(this UpdateEmployeeDto dto, string id)
    {
        return new UpdateEmployeeCommand
        {
            EmployeeId = id,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            EmailAddress = dto.EmailAddress,
            DateOfBirth = dto.DateOfBirth,
            RoleId = dto.RoleId,
            ReportsToId = dto.ManagerId
        };
    }

    private static EmployeeManagerDto GetManager(Employee employee)
    {
        return employee.Manager != null
            ? new EmployeeManagerDto
            {
                Id = employee.Manager.Id.ToString(),
                FirstName = employee.Manager.Name.FirstName,
                LastName = employee.Manager.Name.LastName,
                Role = employee.Manager.Role.ToString()
            }
            : null;
    }
}