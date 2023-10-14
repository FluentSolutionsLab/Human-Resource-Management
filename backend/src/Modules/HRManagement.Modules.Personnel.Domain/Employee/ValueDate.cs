using System;
using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;
using HRManagement.Common.Domain.Models;
using ValueObject = HRManagement.Common.Domain.Models.ValueObject;

namespace HRManagement.Modules.Personnel.Domain;

public class ValueDate : ValueObject
{
    protected ValueDate()
    {
    }

    private ValueDate(DateOnly date)
    {
        Date = date;
    }

    public DateOnly Date { get; }

    public static Result<ValueDate, List<Error>> Create(string date)
    {
        var errors = ValidateBusinessRules(date, out var actualDate);
        if (errors.Any()) return errors;

        return new ValueDate(actualDate);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Date;
    }

    private static List<Error> ValidateBusinessRules(string date, out DateOnly actualDate)
    {
        var actualDateRule = CheckRule(new DateMustBeValidRule(date));
        if (actualDateRule.IsFailure)
        {
            actualDate = default;
            return new List<Error> {Error.Deserialize(actualDateRule.Error)};
        }

        actualDate = DateOnly.FromDateTime(DateTime.Parse(date));

        var rule = CheckRule(new DateNotInFutureRule(actualDate));
        return rule.IsFailure ? new List<Error> {Error.Deserialize(rule.Error)} : new List<Error>();
    }
}