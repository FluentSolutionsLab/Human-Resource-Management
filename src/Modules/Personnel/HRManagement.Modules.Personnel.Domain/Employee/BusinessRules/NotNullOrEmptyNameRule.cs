namespace HRManagement.Modules.Personnel.Domain.Employee.BusinessRules;

public class NotNullOrEmptyNameRule : IBusinessRule
{
    private readonly string _name;
    private readonly string _propertyName;

    public NotNullOrEmptyNameRule(string name, string propertyName)
    {
        _name = name;
        _propertyName = propertyName;
    }

    public bool IsBroken()
    {
        return string.IsNullOrEmpty(_name);
    }

    public Error Error => DomainErrors.NullOrEmptyName(_propertyName);
}