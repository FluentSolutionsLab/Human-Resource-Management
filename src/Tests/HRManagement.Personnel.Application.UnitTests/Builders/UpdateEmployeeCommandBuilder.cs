namespace HRManagement.Personnel.Application.UnitTests.Builders;

public class UpdateEmployeeCommandBuilder
{
    private readonly UpdateEmployeeCommand _command = new();
    private readonly Person _person = new Faker().Person;

    public UpdateEmployeeCommandBuilder WithEmailAddress(string email)
    {
        _command.EmailAddress = email;
        return this;
    }

    public UpdateEmployeeCommandBuilder WithFirstName(string firstName)
    {
        _command.FirstName = firstName;
        return this;
    }

    public UpdateEmployeeCommandBuilder WithLastName(string lastName)
    {
        _command.LastName = lastName;
        return this;
    }

    public UpdateEmployeeCommandBuilder WithDateOfBirth(string dateOfBirth)
    {
        _command.DateOfBirth = dateOfBirth;
        return this;
    }

    public UpdateEmployeeCommandBuilder WithManagerId(string reportsToId)
    {
        _command.ReportsToId = reportsToId;
        return this;
    }

    public UpdateEmployeeCommandBuilder WithEmployeeId(string id)
    {
        _command.EmployeeId = id;
        return this;
    }

    public UpdateEmployeeCommandBuilder WithFixture()
    {
        _command.EmployeeId = It.IsNotNull<Guid>().ToString();
        _command.EmailAddress = _person.Email;
        _command.FirstName = _person.FirstName;
        _command.LastName = _person.LastName;
        _command.DateOfBirth = _person.DateOfBirth.Date.ToString("d");
        _command.HiringDate = new Faker().Date.Recent(60).ToString("d");
        _command.ReportsToId = new Faker().Random.Guid().ToString();
        _command.RoleId = It.IsAny<byte>();

        return this;
    }

    public UpdateEmployeeCommandBuilder WithFixture(Employee employee)
    {
        _command.EmployeeId = It.IsNotNull<Guid>().ToString();
        _command.EmailAddress = employee.EmailAddress.Email;
        _command.FirstName = employee.Name.FirstName;
        _command.LastName = employee.Name.LastName;
        _command.DateOfBirth = employee.BirthDate.Date.ToString("d");
        _command.HiringDate = employee.HireDate.Date.ToString();
        _command.ReportsToId = employee.Manager.Id.ToString();
        _command.RoleId = employee.Role.Id;

        return this;
    }

    public UpdateEmployeeCommand Build()
    {
        return _command;
    }
}