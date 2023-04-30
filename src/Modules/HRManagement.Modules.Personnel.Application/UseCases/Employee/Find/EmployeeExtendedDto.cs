using System.Collections.Generic;

namespace HRManagement.Modules.Personnel.Application.UseCases;

public class EmployeeExtendedDto
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public string DateOfBirth { get; set; }
    public string HireDate { get; set; }
    public string Role { get; set; }
    public EmployeeManagerDto Manager { get; set; }
    public List<ManagedEmployeeDto> ManagedEmployees { get; set; }
}