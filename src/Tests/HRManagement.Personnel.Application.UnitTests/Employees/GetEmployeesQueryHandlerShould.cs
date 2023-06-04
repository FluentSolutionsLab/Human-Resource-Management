using HRManagement.Common.Application.Contracts;

namespace HRManagement.Personnel.Application.UnitTests.Employees;

public class GetEmployeesQueryHandlerShould
{
    private readonly IFixture _fixture;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ICacheService> _mockCacheService;
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
        var person = new Faker().Person;
        _mockCacheService.Setup(x => x.Get<PagedList<Employee>>(It.IsAny<string>())).Returns((PagedList<Employee>) null);
        _mockUnitOfWork
            .Setup(d => d.GetRepository<Employee, Guid>().GetAsync(
                It.IsAny<Expression<Func<Employee, bool>>>(),
                It.IsAny<Func<IQueryable<Employee>, IOrderedQueryable<Employee>>>(), 
                It.IsNotNull<string>(),
                It.IsAny<int>(),
                It.IsAny<int>()))
            .ReturnsAsync(new PagedList<Employee>(new[] {BuildFakeEmployee(person)}, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()));

        var result = await _sut.Handle(_fixture.Create<GetEmployeesQuery>(), CancellationToken.None);

        result.Value.ShouldNotBeNull();
        result.Value.First().FirstName.ShouldBe(person.FirstName);
    }
    
    private static Employee BuildFakeEmployee(Person person)
    {
        var hiringDate = new Faker().Date.Past(15);
        return Employee.Create(
            Name.Create(person.FirstName, person.LastName).Value,
            EmailAddress.Create(person.Email).Value,
            ValueDate.Create(person.DateOfBirth.ToString("d")).Value,
            ValueDate.Create(hiringDate.ToString("d")).Value,
            Role.Create("ceo", null).Value,
            null).Value;
    }

}