namespace HRManagement.Modules.Personnel.Application.UseCases;

public class GetEmployeeQuery : IQuery<Result<EmployeeDto, Error>>
{
    public string EmployeeId { get; set; }
}