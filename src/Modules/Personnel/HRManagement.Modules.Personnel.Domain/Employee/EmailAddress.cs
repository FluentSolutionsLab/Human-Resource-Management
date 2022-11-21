using CSharpFunctionalExtensions;
using HRManagement.Modules.Personnel.Domain.Employee.BusinessRules;

namespace HRManagement.Modules.Personnel.Domain.Employee;

public class EmailAddress : ValueObject
{
    public string Email { get; }

    private EmailAddress(string email)
    {
        Email = email;
    }

    public static Result<EmailAddress, Error> Create(string email)
    {
        var notNullOrEmptyRule = CheckRule(new NotNullOrEmptyEmailAddressRule(email, nameof(Email)));
        if (notNullOrEmptyRule.IsFailure)
            return Error.Deserialize(notNullOrEmptyRule.Error);

        var validEmailRule = CheckRule(new ValidEmailAddressRule(email));
        if (validEmailRule.IsFailure)
            return Error.Deserialize(validEmailRule.Error);

        return new EmailAddress(email);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Email;
    }
}