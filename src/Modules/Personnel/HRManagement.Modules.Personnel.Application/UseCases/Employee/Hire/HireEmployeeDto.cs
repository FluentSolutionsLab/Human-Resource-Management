namespace HRManagement.Modules.Personnel.Application.UseCases;

public class HireEmployeeDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public string DateOfBirth { get; set; }
    public byte RoleId { get; set; }
    public string ManagerId { get; set; }
}