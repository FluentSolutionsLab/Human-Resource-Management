namespace HRManagement.Modules.Personnel.Application.UseCases;

public class GetEmployeeQuery : IQuery<Result<EmployeeExtendedDto, Error>>
{
    public string EmployeeId { get; set; }
}