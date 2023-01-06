using AutoFixture;
using AutoFixture.AutoMoq;
using Bogus;
using HRManagement.Modules.Personnel.Application.Contracts;
using HRManagement.Modules.Personnel.Application.Features.Employee;
using HRManagement.Modules.Personnel.Domain;
using HRManagement.Modules.Personnel.Domain.Employee;
using Moq;
using Shouldly;
using Xunit;

namespace HRManagement.Modules.Personnel.Application.UnitTests;

public class GetEmployeeQueryHandlerShould
{
    [Fact]
    public async Task ReturnEmployee_WhenEmployeeExists()
    {
        var fixture = SetFixture(out var mockEmployeeRepo);
        var person = new Faker().Person;
        PrepareMocks(mockEmployeeRepo, person);
        var sut = fixture.Create<GetEmployeeQueryHandler>();
        var getEmployee = fixture.Create<GetEmployeeQuery>();
        getEmployee.EmployeeId = Guid.NewGuid().ToString();

        var result = await sut.Handle(getEmployee, CancellationToken.None);

        result.Value.ShouldNotBeNull();
        result.Value.FirstName.ShouldBe(person.FirstName);
    }

    [Fact]
    public async Task ReturnNotFoundError_WhenProvidedKeyInvalid()
    {
        var fixture = SetFixture(out _);
        var sut = fixture.Create<GetEmployeeQueryHandler>();
        var getEmployee = fixture.Create<GetEmployeeQuery>();
        var invalidEmployeeId = new Faker().Random.AlphaNumeric(9);
        getEmployee.EmployeeId = invalidEmployeeId;

        var result = await sut.Handle(getEmployee, CancellationToken.None);

        result.Error.ShouldNotBeNull();
        result.Error.ShouldBeEquivalentTo(DomainErrors.NotFound(nameof(Employee), invalidEmployeeId));
    }
    
    [Fact]
    public async Task ReturnError_WhenEmployeeDoesNotExist()
    {
        var fixture = SetFixture(out var mock);
        mock.Setup(d => d.GetByIdAsync(It.IsAny<Guid>()))!.ReturnsAsync(default(Employee));
        var sut = fixture.Create<GetEmployeeQueryHandler>();

        var result = await sut.Handle(fixture.Create<GetEmployeeQuery>(), CancellationToken.None);

        result.Error.ShouldNotBeNull();
        result.Error.Code.ShouldBe(DomainErrors.NotFound(It.IsAny<string>(), It.IsAny<Guid>()).Code);
    }

    private static void PrepareMocks(Mock<IEmployeeRepository> mockEmployeeRepo, Person person)
    {
        var employee = Employee.Create(
            Name.Create(person.FirstName, person.LastName).Value,
            EmailAddress.Create(person.Email).Value,
            DateOfBirth.Create(person.DateOfBirth.ToString("d")).Value).Value;
        mockEmployeeRepo.Setup(d => d.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(employee);
    }

    private static IFixture SetFixture(out Mock<IEmployeeRepository> mockEmployeeRepo)
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        mockEmployeeRepo = fixture.Freeze<Mock<IEmployeeRepository>>();
        return fixture;
    }
}