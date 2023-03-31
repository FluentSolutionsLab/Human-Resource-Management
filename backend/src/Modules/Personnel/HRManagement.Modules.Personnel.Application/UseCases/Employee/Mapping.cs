using HRManagement.Common.Domain.Utilities;

namespace HRManagement.Modules.Personnel.Application.UseCases;

public static partial class Mapping
{
    public static EmployeeDto ToResponseDto(this Domain.Employee employee)
    {
        return new EmployeeDto
        {
            Id = employee.Id.ToString(),
            FirstName = employee.Name.FirstName,
            LastName = employee.Name.LastName,
            EmailAddress = employee.EmailAddress.Email,
            DateOfBirth = employee.DateOfBirth.Date.ToISO8601String(),
            HireDate = employee.HireDate.ToISO8601String(),
            Role = employee.Role?.ToString(),
            Manager = employee.ReportsTo?.Name.ToString()
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
}