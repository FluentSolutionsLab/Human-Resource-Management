using Bogus;
using HRManagement.Modules.Staff.Features.CreateEmployee;
using Moq;

namespace HRManagement.Staff.Tests.Features.Builders;

public class HireEmployeeCommandBuilder
{
    private readonly HireEmployeeCommand _command = new();
    private readonly Person _fakePerson = new Faker().Person;

    public HireEmployeeCommandBuilder WithFixture()
    {
        _command.EmailAddress = _fakePerson.Email;
        _command.FirstName = _fakePerson.FirstName;
        _command.LastName = _fakePerson.LastName;
        _command.DateOfBirth = _fakePerson.DateOfBirth.Date.ToString("d");
        _command.HiringDate = new Faker().Date.Recent(60).ToString("d");
        _command.ReportsToId = new Faker().Random.Guid().ToString();
        _command.RoleId = It.IsAny<byte>();

        return this;
    }

    public HireEmployeeCommand Build()
    {
        return _command;
    }
}