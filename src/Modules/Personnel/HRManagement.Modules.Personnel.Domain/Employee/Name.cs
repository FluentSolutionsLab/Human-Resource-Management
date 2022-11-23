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

    public static Result<Name, List<Error>> Create(string firstName, string lastName)
    {
        var errors = ValidateBusinessRules(firstName, lastName);
        if (errors.Any()) return errors;
        
        return new Name(firstName, lastName);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return FirstName;
        yield return LastName;
    }

    private static List<Error> ValidateBusinessRules(string firstName, string lastName)
    {
        var errors = new List<Error>();
        var firstNameNullOrEmptyRule = CheckRule(new NotNullOrEmptyNameRule(firstName, nameof(FirstName)));
        if (firstNameNullOrEmptyRule.IsFailure)
            errors.Add(Error.Deserialize(firstNameNullOrEmptyRule.Error));

        var lastNameNullOrEmptyRule = CheckRule(new NotNullOrEmptyNameRule(lastName, nameof(LastName)));
        if (lastNameNullOrEmptyRule.IsFailure)
            errors.Add(Error.Deserialize(lastNameNullOrEmptyRule.Error));

        var firstNameRule = CheckRule(new ValidNameRule(firstName));
        if (firstNameRule.IsFailure)
            errors.Add(Error.Deserialize(firstNameRule.Error));
        
        var lastNameRule = CheckRule(new ValidNameRule(lastName));
        if (lastNameRule.IsFailure)
            errors.Add(Error.Deserialize(lastNameRule.Error));
        return errors;
    }
}