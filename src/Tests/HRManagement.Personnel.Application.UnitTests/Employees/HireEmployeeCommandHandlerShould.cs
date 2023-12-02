using System.Globalization;
using CSharpFunctionalExtensions;
using HRManagement.Common.Application.Contracts;
using HRManagement.Modules.Personnel.Persistence;
using HRManagement.Personnel.Application.UnitTests.Builders;

namespace HRManagement.Personnel.Application.UnitTests.Employees;

public class HireEmployeeCommandHandlerShould
{
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Dictionary<string, Role> _roles = DatabaseInitializer.SeedDataRoles;
    private readonly HireEmployeeCommandHandler _sut;
    private HireEmployeeCommand _command;

    public HireEmployeeCommandHandlerShould()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        _mockUnitOfWork = fixture.Freeze<Mock<IUnitOfWork>>();
        _mockCacheService = fixture.Freeze<Mock<ICacheService>>();
        _sut = fixture.Create<HireEmployeeCommandHandler>();

        _command = new HireEmployeeCommandBuilder().WithFixture().Build();
        _mockCacheService
            .Setup(service => service.Get<Maybe<Role>>(It.IsAny<string>()))
            .Returns(_roles["president"]);
        _mockCacheService
            .Setup(service => service.Get<Maybe<Employee>>(It.IsAny<string>()))
            .Returns(new EmployeeBuilder().WithFixture().Build());
        _mockUnitOfWork
            .Setup(d => d.GetRepository<Employee, Guid>().HasMatches(It.IsAny<Expression<Func<Employee, bool>>>()))
            .ReturnsAsync(Result.Failure<bool>("No match found."));
    }

    [Fact(DisplayName = "Succeed when request is valid, and employee does not exist yet.")]
    public async Task Succeed_WhenPayloadIsValid()
    {
        var result = await _sut.Handle(_command, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeOfType<EmployeeDto>();
    }

    [Theory(DisplayName = "Fail when firstname or lastname is not valid")]
    [ClassData(typeof(InvalidNameOnCreationTestData))]
    public async Task Fail_WhenNameInvalid(string firstName, string lastName)
    {
        _command = new HireEmployeeCommandBuilder()
            .WithFixture()
            .WithFirstName(firstName)
            .WithLastName(lastName)
            .Build();

        var result = await _sut.Handle(_command, CancellationToken.None);

        result.Error.ShouldNotBeNull();
        result.Error.Code.ShouldBe(DomainErrors.InvalidInput(It.IsNotNull<string>()).Code);
    }

    [Theory(DisplayName = "Fail when email is not valid")]
    [ClassData(typeof(InvalidEmailOnCreationTestData))]
    public async Task Fail_WhenEmailInvalid(string email)
    {
        var command = new HireEmployeeCommandBuilder().WithFixture().WithEmailAddress(email).Build();

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Error.ShouldNotBeNull();
        result.Error.Code.ShouldBe(DomainErrors.InvalidInput(It.IsNotNull<string>()).Code);
    }

    [Theory(DisplayName = "Fail when date of birth is not valid")]
    [ClassData(typeof(InvalidDateOfBirthTestData))]
    public async Task Fail_WhenDateOfBirthInvalid(string dateOfBirth)
    {
        var command = new HireEmployeeCommandBuilder().WithFixture().WithDateOfBirth(dateOfBirth).Build();

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Error.ShouldNotBeNull();
        result.Error.Code.ShouldBe(DomainErrors.InvalidInput(It.IsNotNull<string>()).Code);
    }

    [Fact(DisplayName = "Fail when the employee already exists")]
    public async Task Fail_WhenEmployeeAlreadyExists()
    {
        _mockUnitOfWork
            .Setup(d => d.GetRepository<Employee, Guid>().HasMatches(It.IsAny<Expression<Func<Employee, bool>>>()))
            .ReturnsAsync(Result.Success(true));

        var result = await _sut.Handle(_command, CancellationToken.None);

        result.Error.ShouldNotBeNull();
        result.Error.ShouldBeEquivalentTo(DomainErrors.ResourceAlreadyExists());
    }

    [Fact(DisplayName = "Fail when the manager does not exist")]
    public async Task Fail_WhenManagerIdIsInvalid()
    {
        _mockCacheService
            .Setup(service => service.Get<Maybe<Employee>>(It.IsAny<string>()))
            .Returns(Maybe<Employee>.None);

        var result = await _sut.Handle(_command, CancellationToken.None);

        result.Error.ShouldNotBeNull();
        result.Error.ShouldBeEquivalentTo(DomainErrors.NotFound(nameof(Employee), _command.ReportsToId));
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