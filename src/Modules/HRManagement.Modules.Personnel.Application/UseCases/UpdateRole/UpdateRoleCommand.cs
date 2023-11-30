namespace HRManagement.Modules.Personnel.Application.UseCases;

public class UpdateRoleCommand : ICommand<UnitResult<Error>>
{
    public byte Id { get; set; }
    public string Name { get; set; }
    public byte? ReportsToId { get; set; }
}