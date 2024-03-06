using System.Linq.Expressions;
using AutoFixture;
using AutoFixture.AutoMoq;
using CSharpFunctionalExtensions;
using HRManagement.BuildingBlocks.Contracts;
using HRManagement.BuildingBlocks.Models;
using HRManagement.Modules.Staff.Features.Employees.Get;
using HRManagement.Modules.Staff.Models;
using HRManagement.Staff.Tests.Features.Builders;
using Moq;
using Shouldly;
using Xunit;

namespace HRManagement.Staff.Tests.Features.Employees;

public class GetEmployeesQueryHandlerShould
{
    private readonly Employee _employee;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly PagedList<Employee> _pagedList;
    private readonly GetEmployeesQuery _query;
    private readonly GetEmployeesQueryHandler _sut;

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