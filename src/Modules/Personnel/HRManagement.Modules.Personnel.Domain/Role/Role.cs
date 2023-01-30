using CSharpFunctionalExtensions;
using HRManagement.Common.Domain.Models;
using HRManagement.Modules.Personnel.Domain.Role.BusinessRules;

namespace HRManagement.Modules.Personnel.Domain.Role;

public class Role : Common.Domain.Models.Entity<byte>
{
    protected Role()
    {
    }

    public Role(string name, Role reportsTo)
    {
        Name = name;
        ReportsTo = reportsTo;
    }

    public string Name { get; private set; }
    public virtual Role ReportsTo { get; private set; }

    public static Result<Role, Error> Create(string name, Role reportsTo)
    {
        var ruleCheck = CheckRule(new RoleNameCannotBeEmptyOrNull(name));
        if (ruleCheck.IsFailure) return Error.Deserialize(ruleCheck.Error);

        return new Role(name, reportsTo);
    }

    public Result<Role, Error> Update(string name, Role reportsTo)
    {
        var ruleCheck = CheckRule(new RoleNameCannotBeEmptyOrNull(name));
        if (ruleCheck.IsFailure) return Error.Deserialize(ruleCheck.Error);

        Name = name;
        ReportsTo = reportsTo;

        return this;
    }

    public override string ToString() => Name;
}