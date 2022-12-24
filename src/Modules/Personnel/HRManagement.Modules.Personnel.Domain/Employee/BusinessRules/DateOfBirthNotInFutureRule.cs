using HRManagement.Common.Domain;

namespace HRManagement.Modules.Personnel.Domain.Employee.BusinessRules;

public class DateOfBirthNotInFutureRule : IBusinessRule
{
    private readonly DateOnly _date;

    public DateOfBirthNotInFutureRule(DateOnly date)
    {
        _date = date;
    }

    public bool IsBroken()
    {
        return _date > DateOnly.FromDateTime(DateTime.Now);
    }

    public Error Error => DomainErrors.DateOfBirthInFuture();
}