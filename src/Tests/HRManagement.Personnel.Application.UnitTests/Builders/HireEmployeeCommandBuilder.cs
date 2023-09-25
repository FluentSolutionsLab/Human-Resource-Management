namespace HRManagement.Personnel.Application.UnitTests.Builders;

public class HireEmployeeCommandBuilder
{
    private readonly HireEmployeeCommand _command = new();
    private readonly Person _fakePerson = new Faker().Person;

    public HireEmployeeCommandBuilder WithEmailAddress(string email)
    {
        _command.EmailAddress = email;
        return this;
    }

    public HireEmployeeCommandBuilder WithFirstName(string firstName)
    {
        _command.FirstName = firstName;
        return this;
    }

    public HireEmployeeCommandBuilder WithLastName(string lastName)
    {
        _command.LastName = lastName;
        return this;
    }

    public HireEmployeeCommandBuilder WithDateOfBirth(string dateOfBirth)
    {
        _command.DateOfBirth = dateOfBirth;
        return this;
    }

    public HireEmployeeCommandBuilder WithManagerId(string reportsToId)
    {
        _command.ReportsToId = reportsToId;
        return this;
    }

    public HireEmployeeCommandBuilder WithFixture()
    {
        _command.EmailAddress = _fakePerson.Email;
        _command.FirstName = _fakePerson.FirstName;
        _command.LastName = _fakePerson.LastName;
        _command.DateOfBirth = _fakePerson.DateOfBirth.Date.ToString("d");
        _command.HiringDate = new Faker().Date.Recent(60).ToString("d");
        _command.ReportsToId = It.IsNotNull<Guid>().ToString();
        _command.RoleId = It.IsAny<byte>();

        return this;
    }

    public HireEmployeeCommand Build()
    {
        return _command;
    }
}