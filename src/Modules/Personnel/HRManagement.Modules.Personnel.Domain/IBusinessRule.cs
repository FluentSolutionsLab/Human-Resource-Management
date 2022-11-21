namespace HRManagement.Modules.Personnel.Domain;

public interface IBusinessRule
{
    bool IsBroken();

    Error Error { get; }
}