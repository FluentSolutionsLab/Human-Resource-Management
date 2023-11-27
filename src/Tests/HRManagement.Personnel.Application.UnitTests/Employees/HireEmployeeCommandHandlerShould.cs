using System.Globalization;
using CSharpFunctionalExtensions;
using HRManagement.Common.Application.Contracts;
using HRManagement.Personnel.Application.UnitTests.Builders;

namespace HRManagement.Personnel.Application.UnitTests.Employees;

public class HireEmployeeCommandHandlerShould
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly HireEmployeeCommandHandler _sut;

    public HireEmployeeCommandHandlerShould()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        _mockUnitOfWork = fixture.Freeze<Mock<IUnitOfWork>>();
        _sut = fixture.Create<HireEmployeeCommandHandler>();
    }

    [Theory]
    [ClassData(typeof(InvalidNameOnCreationTestData))]
    public async Task ReturnError_WhenNameInvalid(string firstName, string lastName)
    {
        var command = new HireEmployeeCommandBuilder()
            .WithFixture()
            .WithFirstName(firstName)
            .WithLastName(lastName)
            .Build();

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Error.Count.ShouldBeGreaterThan(0);
        result.Error.All(error => error.Code == DomainErrors.InvalidName(It.IsNotNull<string>()).Code).ShouldBeTrue();
    }

    [Theory]
    [ClassData(typeof(InvalidEmailOnCreationTestData))]
    public async Task ReturnError_WhenEmailInvalid(string email)
    {
        var command = new HireEmployeeCommandBuilder().WithFixture().WithEmailAddress(email).Build();

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Error.Count.ShouldBeGreaterThan(0);
        result.Error
            .All(error => error.Code == DomainErrors.InvalidEmailAddress(It.IsNotNull<string>()).Code)
            .ShouldBeTrue();
    }

    [Theory]
    [ClassData(typeof(InvalidDateOfBirthTestData))]
    public async Task ReturnError_WhenDateOfBirthInvalid(string dateOfBirth)
    {
        var command = new HireEmployeeCommandBuilder().WithFixture().WithDateOfBirth(dateOfBirth).Build();

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Error.Count.ShouldBeGreaterThan(0);
        result.Error.All(error =>
            error.Code == DomainErrors.InvalidDate(It.IsNotNull<string>()).Code ||
            error.Code == DomainErrors.DateInFuture().Code).ShouldBeTrue();
    }

    [Fact]
    public async Task ReturnError_WhenEmployeeAlreadyExists()
    {
        var employee = new EmployeeBuilder().WithFixture().Build();
        var command = new HireEmployeeCommandBuilder()
            .WithFixture()
            .WithEmailAddress(employee.EmailAddress.Email)
            .WithFirstName(employee.Name.FirstName)
            .WithDateOfBirth(employee.BirthDate.Date.ToString())
            .WithLastName(employee.Name.LastName)
            .Build();
        _mockUnitOfWork
            .Setup(d => d.GetRepository<Employee, Guid>().GetAsync(
                It.IsAny<Expression<Func<Employee, bool>>>(),
                It.IsAny<Func<IQueryable<Employee>, IOrderedQueryable<Employee>>>(),
                It.IsNotNull<string>(),
                It.IsAny<int>(),
                It.IsAny<int>()))
            .ReturnsAsync(new PagedList<Employee>(new[] {employee}, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()));
        _mockUnitOfWork
            .Setup(d => d.GetRepository<Role, byte>().GetByIdAsync(command.RoleId))
            .ReturnsAsync(Role.Create(RoleName.Create("role").Value, Maybe<Role>.None).Value);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Error.Count.ShouldBe(1);
        result.Error.First().Code.ShouldBe(DomainErrors.ResourceAlreadyExists().Code);
    }

    [Fact]
    public async Task ReturnError_WhenManagerIdIsInvalid()
    {
        var hireEmployeeCommand = new HireEmployeeCommandBuilder()
            .WithFixture()
            .WithManagerId(string.Empty)
            .Build();

        var result = await _sut.Handle(hireEmployeeCommand, CancellationToken.None);
        result.Error.Count.ShouldBe(1);
        result.Error.First().Code.ShouldBe(DomainErrors.NotFound(nameof(Role), hireEmployeeCommand.ReportsToId).Code);
    }
}

public class InvalidDateOfBirthTestData : TheoryData<string>
{
    public InvalidDateOfBirthTestData()
    {
        Add(null!);
        Add(string.Empty);
        Add(new Faker().Random.AlphaNumeric(9));
        Add(DateTime.Now.AddDays(1).ToString(CultureInfo.CurrentCulture));
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