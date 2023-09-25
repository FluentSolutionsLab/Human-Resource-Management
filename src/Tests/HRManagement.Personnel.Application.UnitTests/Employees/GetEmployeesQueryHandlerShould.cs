using HRManagement.Common.Application.Contracts;
using HRManagement.Personnel.Application.UnitTests.Builders;

namespace HRManagement.Personnel.Application.UnitTests.Employees;

public class GetEmployeesQueryHandlerShould
{
    private readonly IFixture _fixture;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly GetEmployeesQueryHandler _sut;

    public GetEmployeesQueryHandlerShould()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _mockUnitOfWork = _fixture.Freeze<Mock<IUnitOfWork>>();
        _mockCacheService = _fixture.Freeze<Mock<ICacheService>>();
        _sut = _fixture.Create<GetEmployeesQueryHandler>();
    }

    [Fact]
    public async Task ReturnListOfEmployees_WhenCalled()
    {
        _mockCacheService.Setup(x => x.Get<PagedList<Employee>>(It.IsAny<string>()))
            .Returns((PagedList<Employee>) null);
        var fakeEmployee = new EmployeeBuilder().WithFixture().Build();
        _mockUnitOfWork
            .Setup(d => d.GetRepository<Employee, Guid>().GetAsync(
                It.IsAny<Expression<Func<Employee, bool>>>(),
                It.IsAny<Func<IQueryable<Employee>, IOrderedQueryable<Employee>>>(),
                It.IsNotNull<string>(),
                It.IsAny<int>(),
                It.IsAny<int>()))
            .ReturnsAsync(new PagedList<Employee>(new[] {fakeEmployee}, It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<int>()));

        var result = await _sut.Handle(_fixture.Create<GetEmployeesQuery>(), CancellationToken.None);

        result.Value.ShouldNotBeNull();
        result.Value.First().FirstName.ShouldBe(fakeEmployee.Name.FirstName);
    }
}