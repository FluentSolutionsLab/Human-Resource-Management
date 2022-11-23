using System.Linq.Expressions;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using Bogus;
using HRManagement.Modules.Personnel.Application.Contracts;
using HRManagement.Modules.Personnel.Application.DTOs;
using HRManagement.Modules.Personnel.Application.Features.Employee;
using HRManagement.Modules.Personnel.Domain.Employee;
using Moq;
using Shouldly;
using Xunit;

namespace HRManagement.Modules.Personnel.Application.UnitTests;

public class GetEmployeesQueryHandlerShould
{
    [Fact]
    public async Task ReturnListOfEmployees_WhenCalled()
    {
        var fixture = SetFixture(out var mockEmployeeRepo, out var mockMapper);
        var person = new Faker().Person;
        var employee = Employee.Create(
            Name.Create(person.FirstName, person.LastName).Value,
            EmailAddress.Create(person.Email).Value,
            DateOfBirth.Create(person.DateOfBirth.ToString("d")).Value).Value;
        var employeeDto = new EmployeeDto
        {
            FirstName = person.FirstName,
            LastName = person.LastName,
            DateOfBirth = DateOnly.FromDateTime(person.DateOfBirth).ToShortDateString(),
            EmailAddress = person.Email,
            HireDate = employee.HireDate.ToShortDateString()
        };
        mockEmployeeRepo
            .Setup(d => d.GetAsync(It.IsAny<Expression<Func<Employee, bool>>>(), It.IsAny<Func<IQueryable<Employee>, IOrderedQueryable<Employee>>>()))
            .ReturnsAsync(new List<Employee> {employee});
        mockMapper
            .Setup(x => x.Map<List<EmployeeDto>>(It.IsAny<List<Employee>>()))
            .Returns(new List<EmployeeDto> {employeeDto});
        var sut = fixture.Create<GetEmployeesQueryHandler>();

        var result = await sut.Handle(fixture.Create<GetEmployeesQuery>(), CancellationToken.None);

        result.Value.ShouldNotBeNull();
        result.Value.First().FirstName.ShouldBe(person.FirstName);
    }

    private static IFixture SetFixture(out Mock<IEmployeeRepository> mockEmployeeRepo, out Mock<IMapper> mockMapper)
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        mockEmployeeRepo = fixture.Freeze<Mock<IEmployeeRepository>>();
        mockMapper = fixture.Freeze<Mock<IMapper>>();
        return fixture;
    }
}