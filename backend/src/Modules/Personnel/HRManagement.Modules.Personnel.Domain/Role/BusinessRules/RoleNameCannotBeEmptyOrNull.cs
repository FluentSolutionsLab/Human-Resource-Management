using HRManagement.Common.Domain.Contracts;
using HRManagement.Common.Domain.Models;

namespace HRManagement.Modules.Personnel.Domain.Role.BusinessRules;

public class RoleNameCannotBeEmptyOrNull : IBusinessRule
{
    private readonly string _name;

    public RoleNameCannotBeEmptyOrNull(string name)
    {
        _name = name;
    }

    public bool IsBroken()
    {
        return string.IsNullOrEmpty(_name) || string.IsNullOrWhiteSpace(_name);
    }

    public Error Error => DomainErrors.InvalidName(_name);
}