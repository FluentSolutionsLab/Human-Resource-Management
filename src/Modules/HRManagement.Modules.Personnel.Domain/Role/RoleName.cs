using System.Collections.Generic;
using CSharpFunctionalExtensions;
using HRManagement.Common.Domain.Models;
using ValueObject = HRManagement.Common.Domain.Models.ValueObject;

namespace HRManagement.Modules.Personnel.Domain;

public class RoleName : ValueObject
{
    public string Value { get; }

    protected RoleName()
    {
    }

    private RoleName(string value)
    {
        Value = value;
    }

    public static Result<RoleName, Error> Create(Maybe<string> nameOrNothing)
    {
        return nameOrNothing
            .ToResult(DomainErrors.InvalidName(nameof(Value)))
            .Map(roleName => roleName.Trim())
            .Ensure(roleName => roleName != string.Empty, DomainErrors.NullOrEmptyName("Role Name"))
            .Map(roleName => new RoleName(roleName));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}