using CSharpFunctionalExtensions;

namespace HRManagement.Staff.Application.UnitTests.Builders;

public class EmployeeBuilder
{
    private readonly Faker _faker = new();
    private readonly Person _person = new();
    private ValueDate _birthDate;
    private EmailAddress _emailAddress;
    private ValueDate _hiringDate;
    private Maybe<Employee> _manager = Maybe<Employee>.None;
    private Name _name;
    private Role _role;

    public EmployeeBuilder WithFixture()
    {
        _name = Name.Create(_person.FirstName, _person.LastName).Value;
        _emailAddress = EmailAddress.Create(_person.Email).Value;
        _birthDate = ValueDate.Create(_person.DateOfBirth.ToString("d")).Value;
        _hiringDate = ValueDate.Create(_faker.Date.Past(15).ToString("d")).Value;
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