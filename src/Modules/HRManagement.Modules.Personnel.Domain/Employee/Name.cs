using System.Collections.Generic;
using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using HRManagement.Common.Domain.Models;
using ValueObject = HRManagement.Common.Domain.Models.ValueObject;

namespace HRManagement.Modules.Personnel.Domain;

public class Name : ValueObject
{
    private static readonly Regex NameRegex =
        new("^[a-z ,.'-]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    protected Name()
    {
    }

    private Name(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public string FirstName { get; }
    public string LastName { get; }

    public static Result<Name, Error> Create(Maybe<string> firstNameOrNoting, Maybe<string> lastNameOrNothing)
    {
        var firstNameResult = ValidateValue(firstNameOrNoting, nameof(FirstName));
        var lastNameResult = ValidateValue(lastNameOrNothing, nameof(LastName));

        var result = Result
            .Combine(firstNameResult, lastNameResult);

        if (result.IsFailure)
            return result.Error;

        return new Name(firstNameOrNoting.Value, lastNameOrNothing.Value);
    }

    private static Result<string, Error> ValidateValue(Maybe<string> name, string fieldName)
    {
        var currentName = string.Empty;
        return name
            .ToResult(DomainErrors.InvalidInput(fieldName))
            .Map(x => x.Trim())
            .Ensure(x => x != string.Empty, DomainErrors.InvalidInput(fieldName))
            .Ensure(x =>
            {
                currentName = x;
                return NameRegex.IsMatch(x);
            }, DomainErrors.InvalidInput(currentName));
    }

    public override string ToString()
    {
        return $"{LastName}, {FirstName}";
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return FirstName;
        yield return LastName;
    }
}