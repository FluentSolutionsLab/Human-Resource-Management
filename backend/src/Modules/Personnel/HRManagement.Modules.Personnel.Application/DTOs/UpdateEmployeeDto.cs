namespace HRManagement.Modules.Personnel.Application.DTOs;

public class UpdateEmployeeDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public string DateOfBirth { get; set; }
    public byte RoleId { get; set; }
    public string ManagerId { get; set; }
}