using AutoFixture;
using AutoFixture.AutoMoq;
using Bogus;
using HRManagement.Modules.Personnel.Application.Contracts;
using HRManagement.Modules.Personnel.Application.Features.Employee;
using HRManagement.Modules.Personnel.Domain;
using HRManagement.Modules.Personnel.Domain.Employee;
using MediatR;
using Moq;
using Shouldly;
using Xunit;

namespace HRManagement.Modules.Personnel.Application.UnitTests;

public class UpdateEmployeeCommandHandlerShould
{
    [Theory]
    [ClassData(typeof(InvalidNameOnUpdateTestData))]
    public async Task ReturnError_WhenNameInvalid(string firstName, string lastName)
    {
        var fixture = SetFixture(out var mockEmployeeRepo);
        var person = new Person();
        mockEmployeeRepo.Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(BuildFakeEmployee(person));
        var command = BuildFakeCommand(person);
        command.FirstName = firstName;
        command.LastName = lastName;
        var sut = fixture.Create<UpdateEmployeeCommandHandler>();

        var result = await sut.Handle(command, CancellationToken.None);

        result.Error.Count.ShouldBeGreaterThan(0);
        result.Error.All(error => error.Code == DomainErrors.InvalidName(It.IsNotNull<string>()).Code).ShouldBeTrue();;
    }

    [Theory]
    [ClassData(typeof(InvalidEmailOnUpdateTestData))]
    public async Task ReturnError_WhenEmailInvalid(string emailAddress)
    {
        var fixture = SetFixture(out var mockEmployeeRepo);
        var person = new Person();
        mockEmployeeRepo.Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(BuildFakeEmployee(person));
        var command = BuildFakeCommand(person);
        command.EmailAddress = emailAddress;
        var sut = fixture.Create<UpdateEmployeeCommandHandler>();

        var result = await sut.Handle(command, CancellationToken.None);

        result.Error.ShouldNotBeNull();
    }

    [Theory]
    [ClassData(typeof(InvalidDateOfBirthOnUpdateTestData))]
    public async Task ReturnError_WhenDateOfBirthInvalid(string dateOfBirth)
    {
        var fixture = SetFixture(out var mockEmployeeRepo);
        var person = new Person();
        mockEmployeeRepo.Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(BuildFakeEmployee(person));
        var command = BuildFakeCommand(person);
        command.DateOfBirth = dateOfBirth;
        var sut = fixture.Create<UpdateEmployeeCommandHandler>();

        var result = await sut.Handle(command, CancellationToken.None);

        result.Error.ShouldNotBeNull();
    }

    [Fact]
    public async Task ReturnNotFoundError_WhenProvidedKeyInvalid()
    {
        var fixture = SetFixture(out _);
        var person = new Person();
        var command = BuildFakeCommand(person);
        var invalidEmployeeId = new Faker().Random.AlphaNumeric(9);
        command.EmployeeId = invalidEmployeeId;
        var sut = fixture.Create<UpdateEmployeeCommandHandler>();

        var result = await sut.Handle(command, CancellationToken.None);

        result.Error.Count.ShouldBe(1);
        result.Error.First().ShouldBeEquivalentTo(DomainErrors.NotFound(nameof(Employee), invalidEmployeeId));
    }

    [Fact]
    public async Task ReturnError_WhenEmployeeNotFound()
    {
        var fixture = SetFixture(out var mockEmployeeRepo);
        var person = new Faker().Person;
        var updateEmployee = BuildFakeCommand(person);
        mockEmployeeRepo
            .Setup(d => d.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => null!);
        var sut = fixture.Create<UpdateEmployeeCommandHandler>();

        var result = await sut.Handle(updateEmployee, CancellationToken.None);

        result.Error.Count.ShouldBe(1);
        result.Error.First().Code.ShouldBe(DomainErrors.NotFound(nameof(Employee), updateEmployee.EmployeeId).Code);
    }

    [Fact]
    public async Task UpdateEmployee_WhenEmployeeExists()
    {
        var fixture = SetFixture(out var mockEmployeeRepo);
        var person = new Faker().Person;
        var employee = BuildFakeEmployee(person);
        var updateEmployee = BuildFakeCommand(person);
        mockEmployeeRepo
            .Setup(d => d.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => employee);
        mockEmployeeRepo
            .Setup(d => d.CommitAsync())
            .Callback(() =>
            {
                var name = Name.Create($"{updateEmployee.FirstName} Updated", updateEmployee.LastName).Value;
                var email = EmailAddress.Create(updateEmployee.EmailAddress).Value;
                var dateOfBirth = DateOfBirth.Create(updateEmployee.DateOfBirth).Value;
                employee.Update(name, email, dateOfBirth);
            });
        var sut = fixture.Create<UpdateEmployeeCommandHandler>();

        var result = await sut.Handle(updateEmployee, CancellationToken.None);

        result.Value.ShouldBe(Unit.Value);
        employee.Name.FirstName.ShouldEndWith(" Updated");
    }

    private static IFixture SetFixture(out Mock<IEmployeeRepository> mockEmployeeRepo)
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        mockEmployeeRepo = fixture.Freeze<Mock<IEmployeeRepository>>();
        return fixture;
    }

    private static Employee BuildFakeEmployee(Person person)
    {
        var employee = Employee.Create(
            Name.Create(person.FirstName, person.LastName).Value,
            EmailAddress.Create(person.Email).Value,
            DateOfBirth.Create(person.DateOfBirth.ToString("d")).Value).Value;
        return employee;
    }

    private static UpdateEmployeeCommand BuildFakeCommand(Person person)
    {
        var hireEmployee = new UpdateEmployeeCommand
        {
            EmployeeId = Guid.NewGuid().ToString(),
            EmailAddress = person.Email,
            FirstName = person.FirstName,
            LastName = person.LastName,
            DateOfBirth = person.DateOfBirth.ToString("d")
        };
        return hireEmployee;
    }
}

public class InvalidNameOnUpdateTestData : TheoryData<string, string>
{
    public InvalidNameOnUpdateTestData()
    {
        var person = new Faker().Person;

        Add(null!, person.LastName);
        Add(string.Empty, person.LastName);
        Add(person.FirstName, null!);
        Add(person.FirstName, string.Empty);
        Add(string.Empty, string.Empty);
        Add(null!, null!);
    }
}

public class InvalidEmailOnUpdateTestData : TheoryData<string>
{
    public InvalidEmailOnUpdateTestData()
    {
        Add(null!);
        Add(string.Empty);
        Add(new Faker().Random.AlphaNumeric(9));
    }
}

public class InvalidDateOfBirthOnUpdateTestData : TheoryData<string>
{
    public InvalidDateOfBirthOnUpdateTestData()
    {
        Add(null!);
        Add(string.Empty);
        Add(new Faker().Random.AlphaNumeric(9));
    }
}