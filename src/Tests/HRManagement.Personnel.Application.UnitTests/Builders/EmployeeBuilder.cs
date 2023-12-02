using CSharpFunctionalExtensions;

namespace HRManagement.Personnel.Application.UnitTests.Builders;

public class EmployeeBuilder
{
    private static readonly Faker Faker = new();

    private readonly Person _fakePerson = Faker.Person;
    private Name _name;
    private EmailAddress _emailAddress;
    private ValueDate _birthDate;
    private ValueDate _hiringDate;
    private Role _role;
    private Maybe<Employee> _manager = Maybe<Employee>.None;

    public EmployeeBuilder WithFixture()
    {
        _name = Name.Create(_fakePerson.FirstName, _fakePerson.LastName).Value;
        _emailAddress = EmailAddress.Create(_fakePerson.Email).Value;
        _birthDate = ValueDate.Create(_fakePerson.DateOfBirth.ToString("d")).Value;
        _hiringDate = ValueDate.Create(Faker.Date.Past(15).ToString("d")).Value;
        _role = Role.Create(RoleName.Create("ceo").Value, null).Value;
        return this;
    }

    public EmployeeBuilder WithRole(Role role)
    {
        _role = role;
        return this;
    }

    public EmployeeBuilder WithManager(Employee employee)
    {
        _manager = employee;
        return this;
    }

    public Employee Build()
    {
        return Employee.Create(
            _name,
            _emailAddress,
            _birthDate,
            _hiringDate,
            _role,
            _manager).Value;
    }
}