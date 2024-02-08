namespace HRManagement.Modules.Staff.Application.UseCases;

public class GetRoleByIdQuery : IQuery<Result<RoleDto, Error>>
{
    public byte Id { get; set; }
}