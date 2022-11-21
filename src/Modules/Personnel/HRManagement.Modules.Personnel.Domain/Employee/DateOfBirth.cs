using CSharpFunctionalExtensions;
using HRManagement.Modules.Personnel.Domain.Employee.BusinessRules;

namespace HRManagement.Modules.Personnel.Domain.Employee;

public class DateOfBirth : ValueObject
{
    public DateOnly Date { get; }

    private DateOfBirth(DateOnly date)
    {
        Date = date;
    }

    public static Result<DateOfBirth, Error> Create(DateOnly date)
    {
        var rule = CheckRule(new DateOfBirthNotInFutureRule(date));
        if (rule.IsFailure)
            return Error.Deserialize(rule.Error);
        return new DateOfBirth(date);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Date;
    }
}