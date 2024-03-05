using HRManagement.BuildingBlocks.Models;

namespace HRManagement.BuildingBlocks.Contracts;

public interface IBusinessRule
{
    Error Error { get; }
    bool IsBroken();
}