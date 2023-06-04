using System;
using HRManagement.Common.Domain.Contracts;
using HRManagement.Common.Domain.Models;

namespace HRManagement.Modules.Personnel.Domain;

public class DateNotInFutureRule : IBusinessRule
{
    private readonly DateOnly _date;

    public DateNotInFutureRule(DateOnly date)
    {
        _date = date;
    }

    public bool IsBroken()
    {
        return _date > DateOnly.FromDateTime(DateTime.Now);
    }

    public Error Error => DomainErrors.DateInFuture();
}