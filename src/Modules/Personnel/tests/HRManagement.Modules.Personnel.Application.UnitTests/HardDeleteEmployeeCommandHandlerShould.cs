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

public class HardDeleteEmployeeCommandHandlerShould
{
    [Fact]
    public async Task ReturnNotFoundError_WhenProvidedKeyInvalid()
    {
        var fixture = SetFixture(out _);
        var sut = fixture.Create<HardDeleteEmployeeCommandHandler>();
        var hardDeleteEmployee = fixture.Create<HardDeleteEmployeeCommand>();
        var invalidEmployeeId = new Faker().Random.AlphaNumeric(9);
        hardDeleteEmployee.EmployeeId = invalidEmployeeId;

        var result = await sut.Handle(hardDeleteEmployee, CancellationToken.None);

        result.Error.ShouldNotBeNull();
        result.Error.ShouldBeEquivalentTo(DomainErrors.NotFound(nameof(Employee), invalidEmployeeId));
    }
    
    [Fact]
    public async Task ReturnError_WhenEmployeeNotFound()
    {
        var fixture = SetFixture(out var mockUnitOfWork);
        var hardDeleteEmployee = BuildFakeCommand();
        mockUnitOfWork
            .Setup(d => d.Employees.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => null!);
        var sut = fixture.Create<HardDeleteEmployeeCommandHandler>();

        var result = await sut.Handle(hardDeleteEmployee, CancellationToken.None);

        result.Error.ShouldNotBeNull();
        result.Error.Code.ShouldBe(DomainErrors.NotFound(nameof(Employee), hardDeleteEmployee.EmployeeId).Code);
    }

    [Fact]
    public async Task HardDeleteEmployee_WhenEmployeeExists()
    {
        var fixture = SetFixture(out var mockUnitOfWork);
        var person = new Faker().Person;
        var employees = new List<Employee>{BuildFakeEmployee(person)};
        var hardDeleteEmployee = BuildFakeCommand();
        mockUnitOfWork
            .Setup(d => d.Employees.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => employees.First());
        mockUnitOfWork
            .Setup(d => d.SaveChangesAsync())
            .Callback(() => employees.Clear());
        var sut = fixture.Create<HardDeleteEmployeeCommandHandler>();

        var result = await sut.Handle(hardDeleteEmployee, CancellationToken.None);

        result.Value.ShouldBe(Unit.Value);
        employees.Count.ShouldBe(0);
    }

    private static IFixture SetFixture(out Mock<IUnitOfWork> mockUnitOfWork)
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        mockUnitOfWork = fixture.Freeze<Mock<IUnitOfWork>>();
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

    private static HardDeleteEmployeeCommand BuildFakeCommand()
    {
        var hireEmployee = new HardDeleteEmployeeCommand
        {
            EmployeeId = Guid.NewGuid().ToString()
        };
        return hireEmployee;
    }
}