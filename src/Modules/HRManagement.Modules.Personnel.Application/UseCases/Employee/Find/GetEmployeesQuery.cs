namespace HRManagement.Modules.Personnel.Application.UseCases;

public class GetEmployeesQuery : IQuery<Result<PagedList<EmployeeDto>>>
{
    public FilterParameters FilterParameters { get; set; }
}