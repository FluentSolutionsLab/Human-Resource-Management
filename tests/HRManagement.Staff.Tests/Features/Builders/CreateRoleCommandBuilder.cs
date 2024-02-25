using HRManagement.Modules.Staff.Features.CreateRole;

namespace HRManagement.Staff.Tests.Features.Builders;

public class CreateRoleCommandBuilder
{
    private readonly CreateRoleCommand _command = new();

    public CreateRoleCommandBuilder WithName(string name)
    {
        _command.Name = name;
        return this;
    }

    public CreateRoleCommandBuilder WithManagerId(byte id)
    {
        _command.ReportsToId = id;
        return this;
    }

    public CreateRoleCommand Build()
    {
        return _command;
    }
}