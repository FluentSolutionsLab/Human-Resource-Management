﻿using System;
using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;
using HRManagement.Common.Domain.Models;
using HRManagement.Modules.Personnel.Domain.BusinessRules;

namespace HRManagement.Modules.Personnel.Domain;

public class Employee : Common.Domain.Models.Entity<Guid>
{
    private readonly List<Employee> _managedEmployees = new();
    
    protected Employee()
    {
    }

    private Employee(Name name, EmailAddress emailAddress, DateOfBirth dateOfBirth, Role role, Employee manager)
    {
        Id = Guid.NewGuid();
        Name = name;
        EmailAddress = emailAddress;
        DateOfBirth = dateOfBirth;
        HireDate = DateOnly.FromDateTime(DateTime.Now);
        Role = role;
        Manager = manager;
    }

    public Name Name { get; private set; }
    public EmailAddress EmailAddress { get; private set; }
    public DateOfBirth DateOfBirth { get; private set; }
    public DateOnly HireDate { get; }
    public DateOnly? TerminationDate { get; private set; }
    public virtual Role Role { get; private set; }
    public virtual Employee Manager { get; private set; }
    public virtual IReadOnlyList<Employee> ManagedEmployees => _managedEmployees.ToList();

    public static Result<Employee, Error> Create(Name name, EmailAddress emailAddress, DateOfBirth dateOfBirth, Role role, Employee reportsTo)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(emailAddress);
        ArgumentNullException.ThrowIfNull(dateOfBirth);

        var error = CheckHierarchyRules(role, reportsTo);

        return error != null ? error : new Employee(name, emailAddress, dateOfBirth, role, reportsTo);
    }

    public Result<Employee, Error> Update(Name name, EmailAddress emailAddress, DateOfBirth dateOfBirth, Role role, Employee reportsTo)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(emailAddress);
        ArgumentNullException.ThrowIfNull(dateOfBirth);

        var error = CheckHierarchyRules(role, reportsTo);

        Name = name;
        EmailAddress = emailAddress;
        DateOfBirth = dateOfBirth;
        Role = role;
        Manager = reportsTo;

        return error != null ? error : this;
    }

    public void Terminate()
    {
        TerminationDate = DateOnly.FromDateTime(DateTime.Now);
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