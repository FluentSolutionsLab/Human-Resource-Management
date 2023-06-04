using System;
using HRManagement.Common.Domain.Contracts;
using HRManagement.Common.Domain.Models;

namespace HRManagement.Modules.Personnel.Domain;

public class DateMustBeValidRule : IBusinessRule
{
    private readonly string _date;

    public DateMustBeValidRule(string date)
    {
        _date = date;
    }

    public bool IsBroken()
    {
        return !DateTime.TryParse(_date, out _);
    }

    public Error Error => DomainErrors.InvalidDate(_date);
}