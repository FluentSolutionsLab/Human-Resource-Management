using HRManagement.Common.Application.Contracts;
using HRManagement.Staff.Application.UnitTests.Builders;

namespace HRManagement.Staff.Application.UnitTests.Employees;

public class HardDeleteEmployeeCommandHandlerShould
{
    private readonly IFixture _fixture;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly HardDeleteEmployeeCommandHandler _sut;

    public HardDeleteEmployeeCommandHandlerShould()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _mockUnitOfWork = _fixture.Freeze<Mock<IUnitOfWork>>();
        _sut = _fixture.Create<HardDeleteEmployeeCommandHandler>();
    }

    [Fact]
    public async Task ReturnNotFoundError_WhenProvidedKeyInvalid()
    {
        var hardDeleteEmployee = _fixture.Create<HardDeleteEmployeeCommand>();
        var invalidEmployeeId = new Faker().Random.AlphaNumeric(9);
        hardDeleteEmployee.EmployeeId = invalidEmployeeId;

        var result = await _sut.Handle(hardDeleteEmployee, CancellationToken.None);

        result.Error.ShouldNotBeNull();
        result.Error.ShouldBeEquivalentTo(DomainErrors.NotFound(nameof(Employee), invalidEmployeeId));
    }

    [Fact]
    public async Task ReturnError_WhenEmployeeNotFound()
    {
        var hardDeleteEmployee = BuildDeleteCommand();
        _mockUnitOfWork
            .Setup(d => d.GetRepository<Employee, Guid>().GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => null!);

        var result = await _sut.Handle(hardDeleteEmployee, CancellationToken.None);

        result.Error.ShouldNotBeNull();
        result.Error.Code.ShouldBe(DomainErrors.NotFound(nameof(Employee), hardDeleteEmployee.EmployeeId).Code);
    }

    [Fact]
    public async Task HardDeleteEmployee_WhenEmployeeExists()
    {
        var employees = new List<Employee> {new EmployeeBuilder().WithFixture().Build()};
        var hardDeleteEmployee = BuildDeleteCommand();
        _mockUnitOfWork
            .Setup(d => d.GetRepository<Employee, Guid>().GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => employees.First());
        _mockUnitOfWork
            .Setup(d => d.SaveChangesAsync())
            .Callback(() => employees.Clear());

        var result = await _sut.Handle(hardDeleteEmployee, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        employees.Count.ShouldBe(0);
    }

    private HardDeleteEmployeeCommand BuildDeleteCommand()
    {
        return new HardDeleteEmployeeCommand
        {
            EmployeeId = Guid.NewGuid().ToString()
        };
    }
}