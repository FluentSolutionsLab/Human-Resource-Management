using System.Text.RegularExpressions;
using HRManagement.Common.Domain.Contracts;
using HRManagement.Common.Domain.Models;

namespace HRManagement.Modules.Personnel.Domain.BusinessRules;

public class ValidNameRule : IBusinessRule
{
    private static readonly Regex NameRegex =
        new("^[a-z ,.'-]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private readonly string _name;

    public ValidNameRule(string name)
    {
        _name = name;
    }

    public bool IsBroken()
    {
        return !string.IsNullOrEmpty(_name) && !NameRegex.IsMatch(_name);
    }

    public Error Error => DomainErrors.InvalidName(_name);
}