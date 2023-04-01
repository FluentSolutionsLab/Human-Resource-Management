﻿using System.Linq.Expressions;
using AutoFixture;
using AutoFixture.AutoMoq;
using Bogus;
using HRManagement.Common.Application.Models;
using HRManagement.Modules.Personnel.Application.Contracts;
using HRManagement.Modules.Personnel.Application.UseCases;
using HRManagement.Modules.Personnel.Domain;
using Moq;
using Shouldly;
using Xunit;

namespace HRManagement.Modules.Personnel.Application.UnitTests.Employees;

public class GetEmployeesQueryHandlerShould
{
    private readonly IFixture _fixture;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly GetEmployeesQueryHandler _sut;

    public GetEmployeesQueryHandlerShould()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _mockUnitOfWork = _fixture.Freeze<Mock<IUnitOfWork>>();
        _sut = _fixture.Create<GetEmployeesQueryHandler>();
    }

    [Fact]
    public async Task ReturnListOfEmployees_WhenCalled()
    {
        var person = new Faker().Person;
        _mockUnitOfWork
            .Setup(d => d.Employees.GetAsync(
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
        return Employee.Create(
            Name.Create(person.FirstName, person.LastName).Value,
            EmailAddress.Create(person.Email).Value,
            DateOfBirth.Create(person.DateOfBirth.ToString("d")).Value,
            Role.Create("ceo", null).Value,
            null).Value;
    }

}