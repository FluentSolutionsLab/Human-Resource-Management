using CSharpFunctionalExtensions;
using HRManagement.Common.Application.Contracts;
using HRManagement.Personnel.Application.UnitTests.Builders;

namespace HRManagement.Personnel.Application.UnitTests.Employees;

public class GetEmployeesQueryHandlerShould
{
    private readonly GetEmployeesQueryHandler _sut;
    private readonly Employee _employee;
    private readonly GetEmployeesQuery _query;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly PagedList<Employee> _pagedList;

    public GetEmployeesQueryHandlerShould()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        _mockUnitOfWork = fixture.Freeze<Mock<IUnitOfWork>>();
        _mockCacheService = fixture.Freeze<Mock<ICacheService>>();
        _sut = fixture.Create<GetEmployeesQueryHandler>();
        _query = fixture.Create<GetEmployeesQuery>();

        _employee = new EmployeeBuilder().WithFixture().Build();
        _pagedList = new PagedList<Employee>(new[] {_employee}, It.IsAny<int>(), It.IsAny<int>(),
            It.IsAny<int>());
        _mockCacheService.Setup(x => x.Get<Maybe<PagedList<Employee>>>(It.IsAny<string>()))
            .Returns(_pagedList);
    }

    [Fact(DisplayName = "Succeed when cached list of matching employees found")]
    public async Task ReturnCachedListOfEmployees_WhenCalled()
    {
        var result = await _sut.Handle(_query, CancellationToken.None);

        result.Value.ShouldNotBeNull();
        result.Value.ShouldBeOfType<PagedList<EmployeeDto>>();
        result.Value.First().FirstName.ShouldBe(_employee.Name.FirstName);
    }

    [Fact(DisplayName = "Succeed when not cached list of matching employees found")]
    public async Task ReturnListOfEmployees_WhenCalled()
    {
        _mockCacheService.Setup(x => x.Get<Maybe<PagedList<Employee>>>(It.IsAny<string>()))
            .Returns(Maybe<PagedList<Employee>>.None);
        _mockUnitOfWork
            .Setup(d => d.GetRepository<Employee, Guid>().GetAsync(
                It.IsAny<Expression<Func<Employee, bool>>>(),
                It.IsAny<Func<IQueryable<Employee>, IOrderedQueryable<Employee>>>(),
                It.IsNotNull<string>(),
                It.IsAny<int>(),
                It.IsAny<int>()))
            .ReturnsAsync(_pagedList);
        var result = await _sut.Handle(_query, CancellationToken.None);

        result.Value.ShouldNotBeNull();
        result.Value.ShouldBeOfType<PagedList<EmployeeDto>>();
        result.Value.First().FirstName.ShouldBe(_employee.Name.FirstName);
    }
}