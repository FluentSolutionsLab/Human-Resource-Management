using CSharpFunctionalExtensions;
using HRManagement.Common.Domain.Models;

namespace HRManagement.Modules.Personnel.Domain.Employee;

public class Employee : Common.Domain.Models.Entity<Guid>
{
    protected Employee()
    {
    }

    private Employee(Name name, EmailAddress emailAddress, DateOfBirth dateOfBirth, Role.Role role, Employee reportsTo)
    {
        Id = Guid.NewGuid();
        Name = name;
        EmailAddress = emailAddress;
        DateOfBirth = dateOfBirth;
        HireDate = DateOnly.FromDateTime(DateTime.Now);
        Role = role;
        ReportsTo = reportsTo;
    }

    public Name Name { get; private set; } = null!;
    public EmailAddress EmailAddress { get; private set; } = null!;
    public DateOfBirth DateOfBirth { get; private set; } = null!;
    public DateOnly HireDate { get; }
    public DateOnly? TerminationDate { get; private set; }
    public virtual Role.Role Role { get; private set; }
    public virtual Employee ReportsTo { get; private set; }

    public static Result<Employee, Error> Create(Name name, EmailAddress emailAddress, DateOfBirth dateOfBirth, Role.Role role, Employee reportsTo)
    {
        if (name == null) throw new ArgumentNullException(nameof(name));
        if (emailAddress == null) throw new ArgumentNullException(nameof(emailAddress));
        if (dateOfBirth == null) throw new ArgumentNullException(nameof(dateOfBirth));

        var error = CheckHierarchyRules(role, reportsTo);

        return error != null ? error : new Employee(name, emailAddress, dateOfBirth, role, reportsTo);
    }

    public Result<Employee, Error> Update(Name name, EmailAddress emailAddress, DateOfBirth dateOfBirth, Role.Role role, Employee reportsTo)
    {
        if (name == null) throw new ArgumentNullException(nameof(name));
        if (emailAddress == null) throw new ArgumentNullException(nameof(emailAddress));
        if (dateOfBirth == null) throw new ArgumentNullException(nameof(dateOfBirth));

        var error = CheckHierarchyRules(role, reportsTo);

        Name = name;
        EmailAddress = emailAddress;
        DateOfBirth = dateOfBirth;
        Role = role;
        ReportsTo = reportsTo;

        return error != null ? error : this;
    }

    public void Terminate()
    {
        TerminationDate = DateOnly.FromDateTime(DateTime.Now);
    }
    
    private static Error CheckHierarchyRules(Role.Role role, Employee reportsTo)
    {
        if (reportsTo == null || role == null) return default;

        var mustReportToIntendedRoleRule = CheckRule(new ManagerRoleMustComplyWithOrganizationRule(reportsTo.Role.Name, role.ReportsTo.Name));
        return mustReportToIntendedRoleRule.IsFailure ? Error.Deserialize(mustReportToIntendedRoleRule.Error) : default;
    }
}