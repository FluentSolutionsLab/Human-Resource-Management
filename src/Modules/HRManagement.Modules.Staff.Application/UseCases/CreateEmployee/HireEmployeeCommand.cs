namespace HRManagement.Modules.Staff.Application.UseCases;

public class HireEmployeeCommand : ICommand<Result<EmployeeDto, Error>>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public string DateOfBirth { get; set; }
    public string HiringDate { get; set; }
    public byte RoleId { get; set; }
    public string ReportsToId { get; set; }
}