using CSharpFunctionalExtensions;
using HRManagement.Common.Application.Contracts;
using HRManagement.Modules.Personnel.Persistence;
using HRManagement.Personnel.Application.UnitTests.Builders;

namespace HRManagement.Personnel.Application.UnitTests.Employees;

public class UpdateEmployeeCommandHandlerShould
{
    private readonly Employee _employee;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Dictionary<string, Role> _roles = DatabaseInitializer.SeedDataRoles;
    private readonly UpdateEmployeeCommandHandler _sut;
    private UpdateEmployeeCommand _command;

    public UpdateEmployeeCommandHandlerShould()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var mockUnitOfWork = fixture.Freeze<Mock<IUnitOfWork>>();
        _mockCacheService = fixture.Freeze<Mock<ICacheService>>();
        _sut = fixture.Create<UpdateEmployeeCommandHandler>();

        var role = _roles["president"];
        var manager = new EmployeeBuilder().WithFixture().Build();
        _employee = new EmployeeBuilder().WithFixture().WithRole(role).WithManager(manager).Build();
        _command = new UpdateEmployeeCommandBuilder().WithFixture(_employee).Build();
        _mockCacheService
            .SetupSequence(d => d.Get<Maybe<Employee>>(It.IsAny<string>()))
            .Returns(_employee)
            .Returns(_employee)
            .Returns(manager)
            .Returns(manager);
        _mockCacheService.Setup(d => d.Get<Maybe<Role>>(It.IsAny<string>()))
            .Returns(role);
        mockUnitOfWork
            .Setup(d => d.SaveChangesAsync())
            .Callback(() =>
            {
                var name = Name.Create($"{_command.FirstName} Updated", _command.LastName).Value;
                var email = EmailAddress.Create(_command.EmailAddress).Value;
                var dateOfBirth = ValueDate.Create(_command.DateOfBirth).Value;
                var hiringDate = ValueDate.Create(_command.HiringDate).Value;
                _employee.Update(name, email, dateOfBirth, hiringDate, role, manager);
            });
    }

    [Theory(DisplayName = "Fail when firstname or lastname is not valid")]
    [ClassData(typeof(InvalidNameOnUpdateTestData))]
    public async Task ReturnError_WhenNameInvalid(string firstName, string lastName)
    {
        _command = new UpdateEmployeeCommandBuilder().WithFixture(_employee).WithFirstName(firstName)
            .WithLastName(lastName).Build();

        var result = await _sut.Handle(_command, CancellationToken.None);

        result.Error.ShouldNotBeNull();
        result.Error.Code.ShouldBe(DomainErrors.InvalidInput(It.IsNotNull<string>()).Code);
    }

    [Theory(DisplayName = "Fail when email address is not valid")]
    [ClassData(typeof(InvalidEmailOnUpdateTestData))]
    public async Task ReturnError_WhenEmailInvalid(string emailAddress)
    {
        _command = new UpdateEmployeeCommandBuilder().WithFixture(_employee).WithEmailAddress(emailAddress).Build();

        var result = await _sut.Handle(_command, CancellationToken.None);

        result.Error.ShouldNotBeNull();
        result.Error.Code.ShouldBe(DomainErrors.InvalidInput(It.IsNotNull<string>()).Code);
    }

    [Theory(DisplayName = "Fail when date of birth is not valid")]
    [ClassData(typeof(InvalidDateOfBirthOnUpdateTestData))]
    public async Task ReturnError_WhenDateOfBirthInvalid(string dateOfBirth)
    {
        _command = new UpdateEmployeeCommandBuilder().WithFixture(_employee).WithDateOfBirth(dateOfBirth).Build();

        var result = await _sut.Handle(_command, CancellationToken.None);

        result.Error.ShouldNotBeNull();
        result.Error.Code.ShouldBe(DomainErrors.InvalidInput(It.IsNotNull<string>()).Code);
    }

    [Fact(DisplayName = "Fail when employee ID is not valid")]
    public async Task ReturnNotFoundError_WhenProvidedKeyInvalid()
    {
        var invalidEmployeeId = new Faker().Random.AlphaNumeric(9);
        _command = new UpdateEmployeeCommandBuilder().WithFixture(_employee).WithEmployeeId(invalidEmployeeId).Build();

        var result = await _sut.Handle(_command, CancellationToken.None);

        result.Error.ShouldNotBeNull();
        result.Error.ShouldBeEquivalentTo(DomainErrors.NotFound(nameof(Employee), invalidEmployeeId));
    }

    [Fact(DisplayName = "Fail when employee not found")]
    public async Task ReturnError_WhenEmployeeNotFound()
    {
        _mockCacheService
            .SetupSequence(d => d.Get<Maybe<Employee>>(It.IsAny<string>()))
            .Returns(Maybe<Employee>.None);

        var result = await _sut.Handle(_command, CancellationToken.None);

        result.Error.ShouldNotBeNull();
        result.Error.ShouldBeEquivalentTo(DomainErrors.NotFound(nameof(Employee), _command.EmployeeId));
    }

    [Fact(DisplayName = "Succeed when request is valid")]
    public async Task UpdateEmployee_WhenEmployeeExists()
    {
        var result = await _sut.Handle(_command, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        _employee.Name.FirstName.ShouldEndWith(" Updated");
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