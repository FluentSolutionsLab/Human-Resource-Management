namespace HRManagement.Modules.Personnel.Application.UseCases;

public class TerminateEmployeeCommand : ICommand<UnitResult<Error>>
{
    public string EmployeeId { get; set; }
    public string TerminationDate { get; set; }
}