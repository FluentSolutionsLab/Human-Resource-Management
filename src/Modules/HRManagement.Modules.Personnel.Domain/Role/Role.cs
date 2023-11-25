using System;
using CSharpFunctionalExtensions;
using HRManagement.Common.Domain.Models;

namespace HRManagement.Modules.Personnel.Domain;

public class Role : Common.Domain.Models.Entity<byte>
{
    protected Role()
    {
    }

    private Role(RoleName name, Role reportsTo)
    {
        Name = name;
        ReportsTo = reportsTo;
    }

    public RoleName Name { get; private set; }
    public virtual Role ReportsTo { get; private set; }

    public static Result<Role, Error> Create(Maybe<RoleName> roleNameOrNothing, Maybe<Role> reportsTo)
    {
        if (roleNameOrNothing.HasNoValue) throw new ArgumentNullException();

        return new Role(roleNameOrNothing.Value, reportsTo.HasValue ? reportsTo.Value : null);
    }

    public void Update(Maybe<RoleName> roleNameOrNothing, Maybe<Role> reportsTo)
    {
        if (roleNameOrNothing.HasNoValue) throw new ArgumentNullException();

        Name = roleNameOrNothing.Value;
        ReportsTo = reportsTo.HasValue ? reportsTo.Value : null;
    }

    public override string ToString()
    {
        return Name.Value;
    }

    // USED ONLY FOR INTEGRATION TEST
    public void SetId(byte id)
    {
        Id = id;
    }
}