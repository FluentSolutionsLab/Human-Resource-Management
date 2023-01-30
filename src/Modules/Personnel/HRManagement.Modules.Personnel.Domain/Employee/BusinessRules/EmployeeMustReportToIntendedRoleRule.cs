using HRManagement.Common.Domain.Contracts;
using HRManagement.Common.Domain.Models;

namespace HRManagement.Modules.Personnel.Domain.Employee;

public class EmployeeMustReportToIntendedRoleRule : IBusinessRule
{
    private readonly string _expectedManagerRole;
    private readonly string _assignedManagerRole;

    public EmployeeMustReportToIntendedRoleRule(string expectedManagerRole, string assignedManagerRole)
    {
        _expectedManagerRole = expectedManagerRole;
        _assignedManagerRole = assignedManagerRole;
    }

    public bool IsBroken()
    {
        return !string.Equals(_expectedManagerRole, _assignedManagerRole, StringComparison.InvariantCultureIgnoreCase);
    }

    public Error Error => DomainErrors.EmployeeMustReportToIntendedRole();
}