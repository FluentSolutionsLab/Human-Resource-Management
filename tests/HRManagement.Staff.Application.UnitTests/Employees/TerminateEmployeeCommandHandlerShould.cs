using HRManagement.Common.Application.Contracts;

namespace HRManagement.Staff.Application.UnitTests.Employees;

public class TerminateEmployeeCommandHandlerShould
{
    private readonly IFixture _fixture;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly TerminateEmployeeCommandHandler _sut;

    public TerminateEmployeeCommandHandlerShould()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _mockUnitOfWork = _fixture.Freeze<Mock<IUnitOfWork>>();
        _sut = _fixture.Create<TerminateEmployeeCommandHandler>();
    }

    [Fact]
    public async Task ReturnNotFoundError_WhenProvidedKeyInvalid()
    {
        var terminateEmployee = _fixture.Create<TerminateEmployeeCommand>();
        var invalidEmployeeId = new Faker().Random.AlphaNumeric(9);
        terminateEmployee.EmployeeId = invalidEmployeeId;

        var result = await _sut.Handle(terminateEmployee, CancellationToken.None);

        result.Error.ShouldNotBeNull();
        result.Error.ShouldBeEquivalentTo(DomainErrors.NotFound(nameof(Employee), invalidEmployeeId));
    }

    [Fact]
    public async Task ReturnError_WhenEmployeeNotFound()
    {
        var updateEmployee = BuildFakeCommand();
        _mockUnitOfWork
            .Setup(d => d.GetRepository<Employee, Guid>().GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => null!);

        var result = await _sut.Handle(updateEmployee, CancellationToken.None);

        result.Error.ShouldNotBeNull();
        result.Error.Code.ShouldBe(DomainErrors.NotFound(nameof(Employee), updateEmployee.EmployeeId).Code);
    }

    [Fact]
    public async Task TerminateEmployee_WhenEmployeeExists()
    {
        var person = new Faker().Person;
        var employees = new List<Employee> {BuildFakeEmployee(person)};
        var terminateEmployee = BuildFakeCommand();
        _mockUnitOfWork
            .Setup(d => d.GetRepository<Employee, Guid>().GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => employees.First());
        _mockUnitOfWork
            .Setup(d => d.SaveChangesAsync());

        var result = await _sut.Handle(terminateEmployee, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        employees.First().TerminationDate.ShouldNotBeNull();
    }

    private static Employee BuildFakeEmployee(Person person)
    {
        var hiringDate = new Faker().Date.Past(15);
        var employee = Employee.Create(
            Name.Create(person.FirstName, person.LastName).Value,
            EmailAddress.Create(person.Email).Value,
            ValueDate.Create(person.DateOfBirth.ToString("d")).Value,
            ValueDate.Create(hiringDate.ToString("d")).Value,
            null,
            null).Value;
        return employee;
    }

    private static TerminateEmployeeCommand BuildFakeCommand()
    {
        var terminateEmployee = new TerminateEmployeeCommand
        {
            EmployeeId = Guid.NewGuid().ToString(),
            TerminationDate = new Faker().Date.Recent().ToString("d")
        };
        return terminateEmployee;
    }
}