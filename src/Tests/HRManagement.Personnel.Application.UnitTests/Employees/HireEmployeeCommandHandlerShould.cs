namespace HRManagement.Personnel.Application.UnitTests.Employees;

public class HireEmployeeCommandHandlerShould
{
    private readonly Mock<IGenericUnitOfWork> _mockUnitOfWork;
    private readonly HireEmployeeCommandHandler _sut;

    public HireEmployeeCommandHandlerShould()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        _mockUnitOfWork = fixture.Freeze<Mock<IGenericUnitOfWork>>();
        _sut = fixture.Create<HireEmployeeCommandHandler>();
    }

    [Theory]
    [ClassData(typeof(InvalidNameOnCreationTestData))]
    public async Task ReturnError_WhenNameInvalid(string firstName, string lastName)
    {
        var person = new Faker().Person;
        var command = BuildFakeCommand(person);
        command.FirstName = firstName;
        command.LastName = lastName;

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Error.Count.ShouldBeGreaterThan(0);
        result.Error.All(error => error.Code == DomainErrors.InvalidName(It.IsNotNull<string>()).Code).ShouldBeTrue();
    }

    [Theory]
    [ClassData(typeof(InvalidEmailOnCreationTestData))]
    public async Task ReturnError_WhenEmailInvalid(string email)
    {
        var person = new Faker().Person;
        var command = BuildFakeCommand(person);
        command.EmailAddress = email;

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
        var person = new Faker().Person;
        var command = BuildFakeCommand(person);
        command.DateOfBirth = dateOfBirth;

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Error.Count.ShouldBeGreaterThan(0);
        result.Error.All(error =>
            error.Code == DomainErrors.InvalidDate(It.IsNotNull<string>()).Code ||
            error.Code == DomainErrors.DateOfBirthInFuture().Code).ShouldBeTrue();
    }

    [Fact]
    public async Task ReturnError_WhenEmployeeAlreadyExists()
    {
        var person = new Faker().Person;
        _mockUnitOfWork
            .Setup(d => d.GetRepository<Employee, Guid>().GetAsync(
                It.IsAny<Expression<Func<Employee, bool>>>(),
                It.IsAny<Func<IQueryable<Employee>, IOrderedQueryable<Employee>>>(),
                It.IsNotNull<string>(),
                It.IsAny<int>(),
                It.IsAny<int>()))
            .ReturnsAsync(new PagedList<Employee>(new[] {BuildFakeEmployee(person)}, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()));

        var result = await _sut.Handle(BuildFakeCommand(person), CancellationToken.None);

        result.Error.Count.ShouldBe(1);
        result.Error.First().Code.ShouldBe(DomainErrors.ResourceAlreadyExists().Code);
    }

    [Fact]
    public async Task ReturnError_WhenManagerIdIsInvalid()
    {
        var person = new Faker().Person;
        var hireEmployeeCommand = BuildFakeCommand(person);
        hireEmployeeCommand.ReportsToId = string.Empty;

        var result = await _sut.Handle(hireEmployeeCommand, CancellationToken.None);
        result.Error.Count.ShouldBe(1);
        result.Error.First().Code.ShouldBe(DomainErrors.NotFound(nameof(Role), hireEmployeeCommand.ReportsToId).Code);
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