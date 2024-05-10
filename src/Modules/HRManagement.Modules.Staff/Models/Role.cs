using CSharpFunctionalExtensions;
using HRManagement.BuildingBlocks.Models;
using HRManagement.Modules.Staff.Models.ValueObjects;

namespace HRManagement.Modules.Staff.Models;

public class Role : BuildingBlocks.Models.Entity<int>
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

    /// <summary>
    /// USED ONLY FOR INTEGRATION TEST 
    /// </summary>
    /// <param name="id">Hardcoded ID for testing purposes</param>
    public void SetId(byte id)
    {
        Id = id;
    }
}