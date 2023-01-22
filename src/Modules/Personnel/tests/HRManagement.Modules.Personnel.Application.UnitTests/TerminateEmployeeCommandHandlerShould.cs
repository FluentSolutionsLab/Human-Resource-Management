using AutoFixture;
using AutoFixture.AutoMoq;
using Bogus;
using HRManagement.Modules.Personnel.Application.Contracts;
using HRManagement.Modules.Personnel.Application.Features.Employee;
using HRManagement.Modules.Personnel.Domain;
using HRManagement.Modules.Personnel.Domain.Employee;
using MediatR;
using Moq;
using Shouldly;
using Xunit;

namespace HRManagement.Modules.Personnel.Application.UnitTests;

public class TerminateEmployeeCommandHandlerShould
{
    [Fact]
    public async Task ReturnNotFoundError_WhenProvidedKeyInvalid()
    {
        var fixture = SetFixture(out _);
        var sut = fixture.Create<TerminateEmployeeCommandHandler>();
        var terminateEmployee = fixture.Create<TerminateEmployeeCommand>();
        var invalidEmployeeId = new Faker().Random.AlphaNumeric(9);
        terminateEmployee.EmployeeId = invalidEmployeeId;

        var result = await sut.Handle(terminateEmployee, CancellationToken.None);

        result.Error.ShouldNotBeNull();
        result.Error.ShouldBeEquivalentTo(DomainErrors.NotFound(nameof(Employee), invalidEmployeeId));
    }

    [Fact]
    public async Task ReturnError_WhenEmployeeNotFound()
    {
        var fixture = SetFixture(out var mockEmployeeRepo);
        var updateEmployee = BuildFakeCommand();
        mockEmployeeRepo
            .Setup(d => d.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => null!);
        var sut = fixture.Create<TerminateEmployeeCommandHandler>();

        var result = await sut.Handle(updateEmployee, CancellationToken.None);

        result.Error.ShouldNotBeNull();
        result.Error.Code.ShouldBe(DomainErrors.NotFound(nameof(Employee), updateEmployee.EmployeeId).Code);
    }

    [Fact]
    public async Task TerminateEmployee_WhenEmployeeExists()
    {
        var fixture = SetFixture(out var mockEmployeeRepo);
        var person = new Faker().Person;
        var employees = new List<Employee>{BuildFakeEmployee(person)};
        var terminateEmployee = BuildFakeCommand();
        mockEmployeeRepo
            .Setup(d => d.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => employees.First());
        mockEmployeeRepo
            .Setup(d => d.CommitAsync());
        var sut = fixture.Create<TerminateEmployeeCommandHandler>();

        var result = await sut.Handle(terminateEmployee, CancellationToken.None);

        result.Value.ShouldBe(Unit.Value);
        employees.First().TerminationDate.ShouldNotBeNull();
    }

    private static IFixture SetFixture(out Mock<IEmployeeRepository> mockEmployeeRepo)
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        mockEmployeeRepo = fixture.Freeze<Mock<IEmployeeRepository>>();
        return fixture;
    }
    
    private static Employee BuildFakeEmployee(Person person)
    {
        var employee = Employee.Create(
            Name.Create(person.FirstName, person.LastName).Value,
            EmailAddress.Create(person.Email).Value,
            DateOfBirth.Create(person.DateOfBirth.ToString("d")).Value, 
            null).Value;
        return employee;
    }

    private static TerminateEmployeeCommand BuildFakeCommand()
    {
        var terminateEmployee = new TerminateEmployeeCommand
        {
            EmployeeId = Guid.NewGuid().ToString()
        };
        return terminateEmployee;
    }
}