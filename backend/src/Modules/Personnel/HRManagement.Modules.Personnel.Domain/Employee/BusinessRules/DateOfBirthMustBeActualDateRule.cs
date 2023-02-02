using HRManagement.Common.Domain.Contracts;
using HRManagement.Common.Domain.Models;

namespace HRManagement.Modules.Personnel.Domain.Employee.BusinessRules;

public class DateOfBirthMustBeActualDateRule : IBusinessRule
{
    private readonly string _date;

    public DateOfBirthMustBeActualDateRule(string date)
    {
        _date = date;
    }

    public bool IsBroken()
    {
        return !DateTime.TryParse(_date, out _);
    }

    public Error Error => DomainErrors.InvalidDate(_date);
}