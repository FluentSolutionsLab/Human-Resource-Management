using System.Linq.Expressions;
using AutoFixture;
using AutoFixture.AutoMoq;
using Bogus;
using HRManagement.Modules.Personnel.Application.Contracts;
using HRManagement.Modules.Personnel.Application.DTOs;
using HRManagement.Modules.Personnel.Application.Features.Employee;
using HRManagement.Modules.Personnel.Domain;
using HRManagement.Modules.Personnel.Domain.Employee;
using HRManagement.Modules.Personnel.Domain.Role;
using Moq;
using Shouldly;
using Xunit;

namespace HRManagement.Modules.Personnel.Application.UnitTests;

public class HireEmployeeCommandHandlerShould
{
    [Theory]
    [ClassData(typeof(InvalidNameOnCreationTestData))]
    public async Task ReturnError_WhenNameInvalid(string firstName, string lastName)
    {
        var fixture = SetFixture(out _);
        var person = new Faker().Person;
        var command = BuildFakeCommand(person);
        command.FirstName = firstName;
        command.LastName = lastName;
        var sut = fixture.Create<HireEmployeeCommandHandler>();

        var result = await sut.Handle(command, CancellationToken.None);

        result.Error.Count.ShouldBeGreaterThan(0);
        result.Error.All(error => error.Code == DomainErrors.InvalidName(It.IsNotNull<string>()).Code).ShouldBeTrue();
    }

    [Theory]
    [ClassData(typeof(InvalidEmailOnCreationTestData))]
    public async Task ReturnError_WhenEmailInvalid(string email)
    {
        var fixture = SetFixture(out _);
        var person = new Faker().Person;
        var command = BuildFakeCommand(person);
        command.EmailAddress = email;
        var sut = fixture.Create<HireEmployeeCommandHandler>();

        var result = await sut.Handle(command, CancellationToken.None);

        result.Error.Count.ShouldBeGreaterThan(0);
        result.Error.All(error => error.Code == DomainErrors.InvalidEmailAddress(It.IsNotNull<string>()).Code).ShouldBeTrue();
    }

    [Theory]
    [ClassData(typeof(InvalidDateOfBirthTestData))]
    public async Task ReturnError_WhenDateOfBirthInvalid(string dateOfBirth)
    {
        var fixture = SetFixture(out _);
        var person = new Faker().Person;
        var command = BuildFakeCommand(person);
        command.DateOfBirth = dateOfBirth;
        var sut = fixture.Create<HireEmployeeCommandHandler>();

        var result = await sut.Handle(command, CancellationToken.None);
        
        result.Error.Count.ShouldBeGreaterThan(0);
        result.Error.All(error => error.Code == DomainErrors.InvalidDate(It.IsNotNull<string>()).Code || error.Code == DomainErrors.DateOfBirthInFuture().Code).ShouldBeTrue();
    }

    [Fact]
    public async Task ReturnError_WhenEmployeeAlreadyExists()
    {
        var fixture = SetFixture(out var mockUnitOfWork);
        var person = new Faker().Person;
        mockUnitOfWork
            .Setup(d => d.Employees.GetAsync(It.IsAny<Expression<Func<Employee,bool>>>(), It.IsAny<Func<IQueryable<Employee>, IOrderedQueryable<Employee>>>(), It.IsNotNull<string>()))
            .ReturnsAsync(new List<Employee> {BuildFakeEmployee(person)});
        var sut = fixture.Create<HireEmployeeCommandHandler>();

        var result = await sut.Handle(BuildFakeCommand(person), CancellationToken.None);

        result.Error.Count.ShouldBe(1);
        result.Error.First().Code.ShouldBe(DomainErrors.ResourceAlreadyExists().Code);
    }

    [Fact]
    public async Task ReturnError_WhenManagerIdIsInvalid()
    {
        var fixture = SetFixture(out var _);
        var person = new Faker().Person;
        var sut = fixture.Create<HireEmployeeCommandHandler>();
        var hireEmployeeCommand = BuildFakeCommand(person);
        hireEmployeeCommand.ReportsToId = string.Empty;
        
        var result = await sut.Handle(hireEmployeeCommand, CancellationToken.None);
        result.Error.Count.ShouldBe(1);
        result.Error.First().Code.ShouldBe(DomainErrors.NotFound(nameof(Role), hireEmployeeCommand.ReportsToId).Code);
    }
    
    private static IFixture SetFixture(out Mock<IUnitOfWork> mockUnitOfWork)
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        mockUnitOfWork = fixture.Freeze<Mock<IUnitOfWork>>();
        return fixture;
    }

    private static Employee BuildFakeEmployee(Person person)
    {
        return Employee.Create(
            Name.Create(person.FirstName, person.LastName).Value,
            EmailAddress.Create(person.Email).Value,
            DateOfBirth.Create(person.DateOfBirth.ToString("d")).Value, 
            Role.Create("ceo", null).Value,
            null).Value;
    }

    private static HireEmployeeCommand BuildFakeCommand(Person person)
    {
        return new HireEmployeeCommand
        {
            EmailAddress = person.Email,
            FirstName = person.FirstName,
            LastName = person.LastName,
            DateOfBirth = person.DateOfBirth.ToString("d"),
            ReportsToId = It.IsNotNull<Guid>().ToString(),
            RoleId = It.IsAny<byte>()
        };
    }
}

public class InvalidDateOfBirthTestData : TheoryData<string>
{
    public InvalidDateOfBirthTestData()
    {
        Add(null!);
        Add(string.Empty);
        Add(new Faker().Random.AlphaNumeric(9));
        Add(DateTime.Now.AddDays(1).ToString());
    }
}

public class InvalidEmailOnCreationTestData : TheoryData<string>
{
    public InvalidEmailOnCreationTestData()
    {
        Add(null!);
        Add(string.Empty);
        Add(new Faker().Random.AlphaNumeric(9));
    }
}

public class InvalidNameOnCreationTestData : TheoryData<string, string>
{
    public InvalidNameOnCreationTestData()
    {
        var person = new Faker().Person;
        
        Add(null!, person.LastName);
        Add(string.Empty, person.LastName);
        Add(person.FirstName, null!);
        Add(person.FirstName, string.Empty);
    }
}