using HRManagement.Common.Application.Contracts;
using HRManagement.Personnel.Application.UnitTests.Builders;

namespace HRManagement.Personnel.Application.UnitTests.Employees;

public class GetEmployeeQueryHandlerShould
{
    private readonly IFixture _fixture;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly GetEmployeeQueryHandler _sut;

    public GetEmployeeQueryHandlerShould()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _mockUnitOfWork = _fixture.Freeze<Mock<IUnitOfWork>>();
        _mockCacheService = _fixture.Freeze<Mock<ICacheService>>();
        _sut = _fixture.Create<GetEmployeeQueryHandler>();
    }

    [Fact]
    public async Task ReturnEmployee_WhenEmployeeExists()
    {
        var fakeEmployee = new EmployeeBuilder().WithFixture().Build();
        _mockCacheService.Setup(x => x.Get<Employee>(It.IsAny<string>())).Returns((Employee) null);
        _mockUnitOfWork
            .Setup(d => d.GetRepository<Employee, Guid>().GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(fakeEmployee);
        var getEmployee = _fixture.Create<GetEmployeeQuery>();
        getEmployee.EmployeeId = Guid.NewGuid().ToString();

        var result = await _sut.Handle(getEmployee, CancellationToken.None);

        result.Value.ShouldNotBeNull();
        result.Value.FirstName.ShouldBe(fakeEmployee.Name.FirstName);
    }

    [Fact]
    public async Task ReturnNotFoundError_WhenProvidedKeyInvalid()
    {
        var getEmployee = _fixture.Create<GetEmployeeQuery>();
        var invalidEmployeeId = new Faker().Random.AlphaNumeric(9);
        getEmployee.EmployeeId = invalidEmployeeId;

        var result = await _sut.Handle(getEmployee, CancellationToken.None);

        result.Error.ShouldNotBeNull();
        result.Error.ShouldBeEquivalentTo(DomainErrors.NotFound(nameof(Employee), invalidEmployeeId));
    }

    [Fact]
    public async Task ReturnError_WhenEmployeeDoesNotExist()
    {
        _mockUnitOfWork
            .Setup(d => d.GetRepository<Employee, Guid>().GetByIdAsync(It.IsAny<Guid>()))!
            .ReturnsAsync(default(Employee));

        var result = await _sut.Handle(_fixture.Create<GetEmployeeQuery>(), CancellationToken.None);

        result.Error.ShouldNotBeNull();
        result.Error.Code.ShouldBe(DomainErrors.NotFound(It.IsAny<string>(), It.IsAny<Guid>()).Code);
    }
}