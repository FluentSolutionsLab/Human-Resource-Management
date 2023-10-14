using System.Text.RegularExpressions;
using HRManagement.Common.Domain.Contracts;
using HRManagement.Common.Domain.Models;

namespace HRManagement.Modules.Personnel.Domain;

public class ValidEmailAddressRule : IBusinessRule
{
    private static readonly Regex EmailRegex =
        new("^[\\w!#$%&’*+/=?`{|}~^-]+(?:\\.[\\w!#$%&’*+/=?`{|}~^-]+)*@(?:[a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private readonly string _email;

    public ValidEmailAddressRule(string email)
    {
        _email = email;
    }

    public bool IsBroken()
    {
        return !string.IsNullOrEmpty(_email) && !EmailRegex.IsMatch(_email);
    }

    public Error Error => DomainErrors.InvalidEmailAddress(_email);
}