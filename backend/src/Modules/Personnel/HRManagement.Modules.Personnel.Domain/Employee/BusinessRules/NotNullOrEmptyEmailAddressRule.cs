using HRManagement.Common.Domain.Contracts;
using HRManagement.Common.Domain.Models;

namespace HRManagement.Modules.Personnel.Domain.BusinessRules;

public class NotNullOrEmptyEmailAddressRule : IBusinessRule
{
    private readonly string _email;
    private readonly string _propertyName;

    public NotNullOrEmptyEmailAddressRule(string email, string propertyName)
    {
        _email = email;
        _propertyName = propertyName;
    }

    public bool IsBroken()
    {
        return string.IsNullOrEmpty(_email);
    }

    public Error Error => DomainErrors.NullOrEmptyName(_propertyName);
}