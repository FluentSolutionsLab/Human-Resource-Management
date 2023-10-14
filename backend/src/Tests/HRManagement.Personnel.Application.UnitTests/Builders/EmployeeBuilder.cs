namespace HRManagement.Personnel.Application.UnitTests.Builders;

public class EmployeeBuilder
{
    private readonly Person _fakePerson = new Faker().Person;
    private Employee _employee;

    public EmployeeBuilder WithFixture()
    {
        var hiringDate = new Faker().Date.Past(15);
        _employee = Employee.Create(
            Name.Create(_fakePerson.FirstName, _fakePerson.LastName).Value,
            EmailAddress.Create(_fakePerson.Email).Value,
            ValueDate.Create(_fakePerson.DateOfBirth.ToString("d")).Value,
            ValueDate.Create(hiringDate.ToString("d")).Value,
            Role.Create("ceo", null).Value,
            null).Value;
        return this;
    }

    public Employee Build()
    {
        return _employee;
    }
}