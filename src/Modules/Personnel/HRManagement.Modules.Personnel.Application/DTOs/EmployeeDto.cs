using HRManagement.Common.Domain.Utilities;
using HRManagement.Modules.Personnel.Domain.Employee;

namespace HRManagement.Modules.Personnel.Application.DTOs;

public class EmployeeDto
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public string DateOfBirth { get; set; }
    public string HireDate { get; set; }
    public string Role { get; set; }
    public string Manager { get; set; }

    public static EmployeeDto MapFromEntity(Employee employee)
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
}