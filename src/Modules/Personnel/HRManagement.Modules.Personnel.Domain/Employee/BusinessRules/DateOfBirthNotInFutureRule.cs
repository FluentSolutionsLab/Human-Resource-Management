using System;
using HRManagement.Common.Domain.Contracts;
using HRManagement.Common.Domain.Models;

namespace HRManagement.Modules.Personnel.Domain.BusinessRules;

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