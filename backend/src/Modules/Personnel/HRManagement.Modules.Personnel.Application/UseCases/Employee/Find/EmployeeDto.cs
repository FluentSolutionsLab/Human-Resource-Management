namespace HRManagement.Modules.Personnel.Application.UseCases;

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
}