﻿using AutoFixture;
using AutoFixture.AutoMoq;
using Bogus;
using CSharpFunctionalExtensions;
using HRManagement.BuildingBlocks.Contracts;
using HRManagement.Modules.Staff;
using HRManagement.Modules.Staff.Features.Employees.Get;
using HRManagement.Modules.Staff.Models;
using HRManagement.Staff.Tests.Features.Builders;
using Moq;
using Shouldly;
using Xunit;

namespace HRManagement.Staff.Tests.Features.Employees;

public class GetEmployeeQueryHandlerShould
{
    private readonly Employee _employee;
    private readonly IFixture _fixture;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly GetEmployeeQuery _query;
    private readonly GetEmployeeQueryHandler _sut;

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