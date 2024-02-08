namespace HRManagement.Modules.Staff.Application.UseCases;

public class GetEmployeesQuery : IQuery<Result<PagedList<EmployeeDto>>>
{
    public FilterParameters FilterParameters { get; set; }
}