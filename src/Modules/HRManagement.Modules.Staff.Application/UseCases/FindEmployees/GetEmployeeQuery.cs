namespace HRManagement.Modules.Staff.Application.UseCases;

public class GetEmployeeQuery : IQuery<Result<EmployeeDto, Error>>
{
    public string EmployeeId { get; set; }
}