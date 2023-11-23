using System.Collections.Generic;
using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using HRManagement.Common.Domain.Models;
using ValueObject = HRManagement.Common.Domain.Models.ValueObject;

namespace HRManagement.Modules.Personnel.Domain;

public class EmailAddress : ValueObject
{
    private static readonly Regex EmailRegex =
        new("^[\\w!#$%&’*+/=?`{|}~^-]+(?:\\.[\\w!#$%&’*+/=?`{|}~^-]+)*@(?:[a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static string _email;

    protected EmailAddress()
    {
    }

    private EmailAddress(string email)
    {
        Email = email;
    }

    public string Email { get; }

    public static Result<EmailAddress, Error> Create(Maybe<string> emailOrNothing)
    {
        return emailOrNothing
            .ToResult(DomainErrors.NullOrEmptyName(nameof(Email)))
            .Tap(email => email.Trim())
            .Ensure(email => email != string.Empty, DomainErrors.NullOrEmptyName(nameof(Email)))
            .Ensure(email =>
            {
                _email = email;
                return EmailRegex.IsMatch(email);
            }, DomainErrors.InvalidEmailAddress(_email))
            .Map(email => new EmailAddress(email));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Email;
    }
}