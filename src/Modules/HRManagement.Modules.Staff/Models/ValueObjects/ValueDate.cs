using CSharpFunctionalExtensions;
using HRManagement.Common.Domain.Models;
using ValueObject = HRManagement.Common.Domain.Models.ValueObject;

namespace HRManagement.Modules.Staff.Models.ValueObjects;

public class ValueDate : ValueObject
{
    private static string _date;

    protected ValueDate()
    {
    }

    private ValueDate(DateOnly date)
    {
        Date = date;
    }

    public DateOnly Date { get; }

    public static Result<ValueDate, Error> Create(Maybe<string> dateOrNothing, string field = null)
    {
        return dateOrNothing
            .ToResult(DomainErrors.NullOrEmptyName(nameof(field)))
            .Map(date => date.Trim())
            .Ensure(date => date != string.Empty, DomainErrors.NullOrEmptyName(nameof(field)))
            .Ensure(date =>
            {
                _date = date;
                return DateTime.TryParse(_date, out _);
            }, DomainErrors.InvalidDate(_date))
            .Ensure(date => DateOnly.FromDateTime(DateTime.Parse(date)) <= DateOnly.FromDateTime(DateTime.Now),
                DomainErrors.DateInFuture())
            .Map(date => new ValueDate(DateOnly.FromDateTime(DateTime.Parse(date))));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Date;
    }
}