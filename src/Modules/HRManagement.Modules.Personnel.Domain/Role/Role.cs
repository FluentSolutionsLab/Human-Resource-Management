using CSharpFunctionalExtensions;
using HRManagement.Common.Domain.Models;

namespace HRManagement.Modules.Personnel.Domain;

public class Role : Common.Domain.Models.Entity<byte>
{
    protected Role()
    {
    }

    private Role(string name, Role reportsTo)
    {
        Name = name;
        ReportsTo = reportsTo;
    }

    public string Name { get; private set; }
    public virtual Role ReportsTo { get; private set; }

    public static Result<Role, Error> Create(Maybe<string> roleNameOrNothing, Role reportsTo)
    {
        return roleNameOrNothing
            .ToResult(DomainErrors.InvalidName(nameof(Name)))
            .Map(roleName => roleName.Trim())
            .Ensure(roleName => roleName != string.Empty, DomainErrors.InvalidName(nameof(Name)))
            .Map(roleName => new Role(roleName, reportsTo));
    }

    public Result<Role, Error> Update(Maybe<string> roleNameOrNothing, Role reportsTo)
    {
        var result = roleNameOrNothing
            .ToResult(DomainErrors.InvalidName(nameof(Name)))
            .Map(roleName => roleName.Trim())
            .Ensure(roleName => roleName != string.Empty, DomainErrors.InvalidName(nameof(Name)))
            .Tap(roleName =>
            {
                Name = roleName;
                ReportsTo = reportsTo;
            });

        return result.IsSuccess ? this : result.Error;
    }

    public override string ToString()
    {
        return Name;
    }

    // USED ONLY FOR INTEGRATION TEST
    public void SetId(byte id)
    {
        Id = id;
    }
}