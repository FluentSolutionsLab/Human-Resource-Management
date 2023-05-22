using HRManagement.Common.Application.Utilities;
using HRManagement.Modules.Personnel.Domain;

namespace HRManagement.Modules.Personnel.Application.UseCases;

public static partial class Mapping
{
    public static EmployeeDto ToResponseDto(this Employee employee)
    {
        var manager = employee.Manager != null
            ? new EmployeeManagerDto
            {
                Id = employee.Manager.Id.ToString(),
                FirstName = employee.Manager.Name.FirstName,
                LastName = employee.Manager.Name.LastName,
                Role = employee.Manager.Role.ToString()
            }
            : null;

        return new EmployeeDto
        {
            Id = employee.Id.ToString(),
            FirstName = employee.Name.FirstName,
            LastName = employee.Name.LastName,
            EmailAddress = employee.EmailAddress.Email,
            DateOfBirth = employee.DateOfBirth.Date.ToISO8601String(),
            HireDate = employee.HireDate.ToISO8601String(),
            Role = employee.Role.ToString(),
            Manager = manager
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

    public static PagedList<EmployeeDto> ToResponseDto(this PagedList<Employee> employees)
    {
        var dtos = employees.Select(x => x.ToResponseDto()).ToList();
        return new PagedList<EmployeeDto>(dtos, employees.TotalCount, employees.CurrentPage, employees.PageSize);
    } 
}