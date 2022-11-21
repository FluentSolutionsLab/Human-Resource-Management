using System.Text.RegularExpressions;

namespace HRManagement.Modules.Personnel.Domain.Employee.BusinessRules;

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
        return !EmailRegex.IsMatch(_email);
    }

    public Error Error => DomainErrors.InvalidEmailAddress(_email);
}