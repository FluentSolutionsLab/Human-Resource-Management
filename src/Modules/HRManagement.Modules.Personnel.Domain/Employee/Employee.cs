using System;
using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;
using HRManagement.Common.Domain.Models;

namespace HRManagement.Modules.Personnel.Domain;

public class Employee : Common.Domain.Models.Entity<Guid>
{
    private readonly List<Employee> _managedEmployees = new();
    
    protected Employee()
    {
    }

    private Employee(Name name, EmailAddress emailAddress, ValueDate birthDate, ValueDate hiringDate, Role role, Employee manager)
    {
        Id = Guid.NewGuid();
        Name = name;
        EmailAddress = emailAddress;
        BirthDate = birthDate;
        HireDate = hiringDate;
        Role = role;
        Manager = manager;
    }

    public Name Name { get; private set; }
    public EmailAddress EmailAddress { get; private set; }
    public ValueDate BirthDate { get; private set; }
    public ValueDate HireDate { get; private set; }
    public ValueDate TerminationDate { get; private set; }
    public virtual Role Role { get; private set; }
    public virtual Employee Manager { get; private set; }
    public virtual IReadOnlyList<Employee> ManagedEmployees => _managedEmployees.ToList();

    public static Result<Employee, Error> Create(Name name, EmailAddress emailAddress, ValueDate birthDate, ValueDate hiringDate, Role role, Employee reportsTo)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(emailAddress);
        ArgumentNullException.ThrowIfNull(birthDate);
        ArgumentNullException.ThrowIfNull(hiringDate);

        var error = CheckHierarchyRules(role, reportsTo);

        return error != null ? error : new Employee(name, emailAddress, birthDate, hiringDate, role, reportsTo);
    }

    public Result<Employee, Error> Update(Name name, EmailAddress emailAddress, ValueDate birthDate, ValueDate hiringDate, Role role, Employee reportsTo)
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
        Manager = reportsTo;

        return error != null ? error : this;
    }

    public void Terminate(ValueDate terminationDate)
    {
        ArgumentNullException.ThrowIfNull(terminationDate);

        TerminationDate = terminationDate;
    }
    
    private static Error CheckHierarchyRules(Role role, Employee reportsTo)
    {
        if (reportsTo == null || role == null) return default;

        var mustReportToIntendedRoleRule = CheckRule(new ManagerRoleMustComplyWithOrganizationRule(reportsTo.Role.Name, role.ReportsTo.Name));
        return mustReportToIntendedRoleRule.IsFailure ? Error.Deserialize(mustReportToIntendedRoleRule.Error) : default;
    }

    public override string ToString()
    {
        return Name + " - " + Role.Name;
    }
}