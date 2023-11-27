namespace HRManagement.Modules.Personnel.Application.UseCases;

public class CreateRoleCommand : ICommand<Result<RoleDto, Error>>
{
    public string Name { get; set; }
    public byte? ReportsToId { get; set; }
}