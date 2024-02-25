using CSharpFunctionalExtensions;
using HRManagement.Common.Domain.Models;
using ValueObject = HRManagement.Common.Domain.Models.ValueObject;

namespace HRManagement.Modules.Staff.Models.ValueObjects;

public class RoleName : ValueObject
{
    protected RoleName()
    {
    }

    private RoleName(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<RoleName, Error> Create(Maybe<string> nameOrNothing)
    {
        return nameOrNothing
            .ToResult(DomainErrors.InvalidInput(nameof(Value)))
            .Map(roleName => roleName.Trim())
            .Ensure(roleName => roleName != string.Empty, DomainErrors.NullOrEmptyName("Role Name"))
            .Map(roleName => new RoleName(roleName));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}