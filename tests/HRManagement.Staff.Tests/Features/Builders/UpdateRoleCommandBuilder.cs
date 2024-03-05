using HRManagement.Modules.Staff.Features.UpdateRole;

namespace HRManagement.Staff.Tests.Features.Builders;

public class UpdateRoleCommandBuilder
{
    private readonly UpdateRoleCommand _command = new();

    public UpdateRoleCommandBuilder WithId(byte id)
    {
        _command.Id = id;
        return this;
    }

    public UpdateRoleCommandBuilder WithName(string name)
    {
        _command.Name = name;
        return this;
    }

    public UpdateRoleCommandBuilder WithManagerId(int id)
    {
        _command.ReportsToId = id;
        return this;
    }

    public UpdateRoleCommand Build()
    {
        return _command;
    }
}