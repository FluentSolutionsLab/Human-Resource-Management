using HRManagement.Common.Domain.Models;

namespace HRManagement.Common.Domain.Contracts;

public interface IBusinessRule
{
    Error Error { get; }
    bool IsBroken();
}