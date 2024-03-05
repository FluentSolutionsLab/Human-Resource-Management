using CSharpFunctionalExtensions;
using HRManagement.BuildingBlocks.Models;
using HRManagement.Modules.Staff.Models.ValueObjects;

namespace HRManagement.Modules.Staff.Models;

public class Employee : BuildingBlocks.Models.Entity<Guid>
{
    private readonly List<Employee> _managedEmployees = new();

    protected Employee()
    {
    }

    private Employee(Name name, EmailAddress emailAddress, ValueDate birthDate, ValueDate hiringDate, Role role,
        Maybe<Employee> manager)
    {
        Id = Guid.NewGuid();
        Name = name;
        EmailAddress = emailAddress;
        BirthDate = birthDate;
        HireDate = hiringDate;
        Role = role;
        Manager = manager.HasValue ? manager.Value : null;
    }

    public Name Name { get; private set; }
    public EmailAddress EmailAddress { get; private set; }
    public ValueDate BirthDate { get; private set; }
    public ValueDate HireDate { get; private set; }
    public ValueDate TerminationDate { get; private set; }
    public virtual Role Role { get; private set; }
    public virtual Employee Manager { get; private set; }
    public virtual IReadOnlyList<Employee> ManagedEmployees => _managedEmployees.ToList();

    public static Result<Employee, Error> Create(Name name, EmailAddress emailAddress, ValueDate birthDate,
        ValueDate hiringDate, Role role, Maybe<Employee> reportsTo)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(emailAddress);
        ArgumentNullException.ThrowIfNull(birthDate);
        ArgumentNullException.ThrowIfNull(hiringDate);

        var error = CheckHierarchyRules(role, reportsTo);

        return error != null ? error : new Employee(name, emailAddress, birthDate, hiringDate, role, reportsTo);
    }

    public Result<Employee, Error> Update(Name name, EmailAddress emailAddress, ValueDate birthDate,
        ValueDate hiringDate, Role role, Maybe<Employee> reportsTo)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(emailAddress);
        ArgumentNullException.ThrowIfNull(birthDate);
        ArgumentNullException.ThrowIfNull(hiringDate);

        var error = CheckHierarchyRules(role, reportsTo);

        Name = name;
        EmailAddress = emailAddress;
        BirthDate = birthDate;
        HireDate = hiringDate;
        Role = role;
        Manager = reportsTo.Value;

        return error != null ? error : this;
    }

    public void Terminate(ValueDate terminationDate)
    {
        ArgumentNullException.ThrowIfNull(terminationDate);

        TerminationDate = terminationDate;
    }

    private static Error CheckHierarchyRules(Maybe<Role> roleOrNothing, Maybe<Employee> reportsToOrNothing)
    {
        if (reportsToOrNothing.HasNoValue || roleOrNothing.HasNoValue) return default;

        var expectedManagerRole = reportsToOrNothing.Value;
        var assignedManagerRole = roleOrNothing.Value;
        return !string.Equals(expectedManagerRole.Role.Name.Value, assignedManagerRole.ReportsTo.Name.Value,
            StringComparison.InvariantCultureIgnoreCase)
            ? DomainErrors.ManagerRoleMustComplyWithOrganization()
            : default;
    }

    public override string ToString()
    {
        return Name + " - " + Role.Name;
    }
}