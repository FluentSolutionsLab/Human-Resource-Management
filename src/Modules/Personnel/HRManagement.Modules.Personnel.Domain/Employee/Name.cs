using CSharpFunctionalExtensions;
using HRManagement.Modules.Personnel.Domain.Employee.BusinessRules;

namespace HRManagement.Modules.Personnel.Domain.Employee;

public class Name : ValueObject
{
    public string FirstName { get; }
    public string LastName { get; }

    private Name(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public static Result<Name, Error> Create(string firstName, string lastName)
    {
        var firstNameNullOrEmptyRule = CheckRule(new NotNullOrEmptyNameRule(firstName, nameof(FirstName)));
        if (firstNameNullOrEmptyRule.IsFailure)
            return Error.Deserialize(firstNameNullOrEmptyRule.Error);

        var lastNameNullOrEmptyRule = CheckRule(new NotNullOrEmptyNameRule(lastName, nameof(LastName)));
        if (lastNameNullOrEmptyRule.IsFailure)
            return Error.Deserialize(lastNameNullOrEmptyRule.Error);

        var firstNameRule = CheckRule(new ValidNameRule(firstName));
        if (firstNameRule.IsFailure)
            return Error.Deserialize(firstNameRule.Error);
        
        var lastNameRule = CheckRule(new ValidNameRule(lastName));
        if (lastNameRule.IsFailure)
            return Error.Deserialize(lastNameRule.Error);
        
        return new Name(firstName, lastName);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return FirstName;
        yield return LastName;
    }
}