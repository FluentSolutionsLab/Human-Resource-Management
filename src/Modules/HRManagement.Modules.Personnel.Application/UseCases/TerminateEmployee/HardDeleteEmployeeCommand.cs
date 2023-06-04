namespace HRManagement.Modules.Personnel.Application.UseCases;

public class HardDeleteEmployeeCommand : ICommand<UnitResult<Error>>
{
    public string EmployeeId { get; set; }
}