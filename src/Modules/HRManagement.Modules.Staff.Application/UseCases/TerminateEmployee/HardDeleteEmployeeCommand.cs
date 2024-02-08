namespace HRManagement.Modules.Staff.Application.UseCases;

public class HardDeleteEmployeeCommand : ICommand<UnitResult<Error>>
{
    public string EmployeeId { get; set; }
}