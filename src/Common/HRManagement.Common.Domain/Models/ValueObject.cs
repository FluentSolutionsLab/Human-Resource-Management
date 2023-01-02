using CSharpFunctionalExtensions;
using HRManagement.Common.Domain.Contracts;

namespace HRManagement.Common.Domain.Models;

public abstract class ValueObject : CSharpFunctionalExtensions.ValueObject
{
    protected static Result<Error> CheckRule(IBusinessRule rule)
    {
        if (rule.IsBroken())
            return Result.Failure<Error>(rule.Error.Serialize());

        return default;
    }
}