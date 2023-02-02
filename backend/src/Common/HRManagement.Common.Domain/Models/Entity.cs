using CSharpFunctionalExtensions;
using HRManagement.Common.Domain.Contracts;

namespace HRManagement.Common.Domain.Models;

public abstract class Entity<TId> : CSharpFunctionalExtensions.Entity<TId>
{
    protected static Result<Error> CheckRule(IBusinessRule rule)
    {
        if (rule.IsBroken())
            return Result.Failure<Error>(rule.Error.Serialize());

        return default;
    }
}