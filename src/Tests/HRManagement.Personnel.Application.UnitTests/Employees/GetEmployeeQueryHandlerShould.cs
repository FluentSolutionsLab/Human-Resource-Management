using CSharpFunctionalExtensions;
using HRManagement.Common.Application.Contracts;
using HRManagement.Personnel.Application.UnitTests.Builders;

namespace HRManagement.Personnel.Application.UnitTests.Employees;

public class GetEmployeeQueryHandlerShould
{
    private readonly IFixture _fixture;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly GetEmployeeQueryHandler _sut;
    private readonly Employee _employee;
    private readonly GetEmployeeQuery _query;

    public GetEmployeeQueryHandlerShould()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _mockUnitOfWork = _fixture.Freeze<Mock<IUnitOfWork>>();
        _mockCacheService = _fixture.Freeze<Mock<ICacheService>>();
        _sut = _fixture.Create<GetEmployeeQueryHandler>();
        _query = _fixture.Create<GetEmployeeQuery>();

        _query.EmployeeId = Guid.NewGuid().ToString();
        _employee = new EmployeeBuilder().WithFixture().Build();
        _mockCacheService.Setup(x => x.Get<Maybe<Employee>>(It.IsAny<string>())).Returns(_employee);
    }

    [Fact(DisplayName = "Succeed when cached employee exists")]
    public async Task ReturnEmployee_WhenEmployeeExists()
    {
        var result = await _sut.Handle(_query, CancellationToken.None);

        result.Value.ShouldNotBeNull();
        result.Value.FirstName.ShouldBe(_employee.Name.FirstName);
    }

    [Fact(DisplayName = "Fail when employee ID provided is not valid")]
    public async Task ReturnNotFoundError_WhenProvidedKeyInvalid()
    {
        var invalidEmployeeId = new Faker().Random.AlphaNumeric(9);
        _query.EmployeeId = invalidEmployeeId;

        var result = await _sut.Handle(_query, CancellationToken.None);

        result.Error.ShouldNotBeNull();
        result.Error.ShouldBeEquivalentTo(DomainErrors.NotFound(nameof(Employee), invalidEmployeeId));
    }

    [Fact(DisplayName = "Fail when employee does not exist")]
    public async Task ReturnError_WhenEmployeeDoesNotExist()
    {
        _mockCacheService.Setup(x => x.Get<Maybe<Employee>>(It.IsAny<string>())).Returns(Maybe<Employee>.None);
        _mockUnitOfWork
            .Setup(d => d.GetRepository<Employee, Guid>().GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Maybe<Employee>.None);

        var result = await _sut.Handle(_fixture.Create<GetEmployeeQuery>(), CancellationToken.None);

        result.Error.ShouldNotBeNull();
        result.Error.Code.ShouldBe(DomainErrors.NotFound(It.IsAny<string>(), It.IsAny<Guid>()).Code);
    }
}