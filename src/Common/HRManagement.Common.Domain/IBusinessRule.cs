namespace HRManagement.Common.Domain;

public interface IBusinessRule
{
    bool IsBroken();

    Error Error { get; }
}