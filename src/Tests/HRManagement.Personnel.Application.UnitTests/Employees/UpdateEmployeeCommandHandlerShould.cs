﻿using HRManagement.Common.Application.Contracts;

namespace HRManagement.Personnel.Application.UnitTests.Employees;

public class UpdateEmployeeCommandHandlerShould
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly UpdateEmployeeCommandHandler _sut;

    public UpdateEmployeeCommandHandlerShould()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        _mockUnitOfWork = fixture.Freeze<Mock<IUnitOfWork>>();
        _sut = fixture.Create<UpdateEmployeeCommandHandler>();
    }

    [Theory]
    [ClassData(typeof(InvalidNameOnUpdateTestData))]
    public async Task ReturnError_WhenNameInvalid(string firstName, string lastName)
    {
        var person = new Person();
        _mockUnitOfWork
            .Setup(uow => uow.GetRepository<Employee, Guid>().GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(BuildFakeEmployee(person));
        var command = BuildFakeCommand(person);
        command.FirstName = firstName;
        command.LastName = lastName;

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Error.Count.ShouldBeGreaterThan(0);
        result.Error.All(error => error.Code == DomainErrors.InvalidName(It.IsNotNull<string>()).Code).ShouldBeTrue();
        ;
    }

    [Theory]
    [ClassData(typeof(InvalidEmailOnUpdateTestData))]
    public async Task ReturnError_WhenEmailInvalid(string emailAddress)
    {
        var person = new Person();
        _mockUnitOfWork
            .Setup(uow => uow.GetRepository<Employee, Guid>().GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(BuildFakeEmployee(person));
        var command = BuildFakeCommand(person);
        command.EmailAddress = emailAddress;

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Error.ShouldNotBeNull();
    }

    [Theory]
    [ClassData(typeof(InvalidDateOfBirthOnUpdateTestData))]
    public async Task ReturnError_WhenDateOfBirthInvalid(string dateOfBirth)
    {
        var person = new Person();
        _mockUnitOfWork
            .Setup(uow => uow.GetRepository<Employee, Guid>().GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(BuildFakeEmployee(person));
        var command = BuildFakeCommand(person);
        command.DateOfBirth = dateOfBirth;

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Error.ShouldNotBeNull();
    }

    [Fact]
    public async Task ReturnNotFoundError_WhenProvidedKeyInvalid()
    {
        var person = new Person();
        var command = BuildFakeCommand(person);
        var invalidEmployeeId = new Faker().Random.AlphaNumeric(9);
        command.EmployeeId = invalidEmployeeId;

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Error.Count.ShouldBe(1);
        result.Error.First().ShouldBeEquivalentTo(DomainErrors.NotFound(nameof(Employee), invalidEmployeeId));
    }

    [Fact]
    public async Task ReturnError_WhenEmployeeNotFound()
    {
        var person = new Faker().Person;
        var updateEmployee = BuildFakeCommand(person);
        _mockUnitOfWork
            .Setup(d => d.GetRepository<Employee, Guid>().GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => null!);

        var result = await _sut.Handle(updateEmployee, CancellationToken.None);

        result.Error.Count.ShouldBe(1);
        result.Error.First().Code.ShouldBe(DomainErrors.NotFound(nameof(Employee), updateEmployee.EmployeeId).Code);
    }

    [Fact]
    public async Task UpdateEmployee_WhenEmployeeExists()
    {
        var ceoRole = Role.Create(RoleName.Create("CEO").Value, null).Value;
        var presidentRole = Role.Create(RoleName.Create("President").Value, ceoRole).Value;
        var managerData = new Faker().Person;
        var manager = BuildFakeEmployee(managerData, ceoRole);
        var person = new Faker().Person;
        var employee = BuildFakeEmployee(person, presidentRole, manager);
        var updateEmployee = BuildFakeCommand(person);
        _mockUnitOfWork
            .SetupSequence(d => d.GetRepository<Employee, Guid>().GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => employee)
            .ReturnsAsync(() => manager);
        _mockUnitOfWork.Setup(d => d.GetRepository<Role, byte>().GetByIdAsync(It.IsAny<byte>()))
            .ReturnsAsync(() => presidentRole);
        _mockUnitOfWork
            .Setup(d => d.SaveChangesAsync())
            .Callback(() =>
            {
                var name = Name.Create($"{updateEmployee.FirstName} Updated", updateEmployee.LastName).Value;
                var email = EmailAddress.Create(updateEmployee.EmailAddress).Value;
                var dateOfBirth = ValueDate.Create(updateEmployee.DateOfBirth).Value;
                var hiringDate = ValueDate.Create(updateEmployee.HiringDate).Value;
                employee.Update(name, email, dateOfBirth, hiringDate, presidentRole, manager);
            });

        var result = await _sut.Handle(updateEmployee, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        employee.Name.FirstName.ShouldEndWith(" Updated");
    }

    private static Employee BuildFakeEmployee(Person person, Role role = null, Employee manager = null)
    {
        var hiringDate = new Faker().Date.Past(15);
        var employee = Employee.Create(
            Name.Create(person.FirstName, person.LastName).Value,
            EmailAddress.Create(person.Email).Value,
            ValueDate.Create(person.DateOfBirth.ToString("d")).Value,
            ValueDate.Create(hiringDate.ToString("d")).Value,
            role,
            manager).Value;
        return employee;
    }

    private static UpdateEmployeeCommand BuildFakeCommand(Person person)
    {
        var hireEmployee = new UpdateEmployeeCommand
        {
            EmployeeId = Guid.NewGuid().ToString(),
            EmailAddress = person.Email,
            FirstName = person.FirstName,
            LastName = person.LastName,
            DateOfBirth = person.DateOfBirth.ToString("d"),
            HiringDate = new Faker().Date.Past(15).ToString("d"),
            RoleId = It.IsAny<byte>(),
            ReportsToId = Guid.NewGuid().ToString()
        };
        return hireEmployee;
    }
}

public class InvalidNameOnUpdateTestData : TheoryData<string, string>
{
    public InvalidNameOnUpdateTestData()
    {
        var person = new Faker().Person;

        Add(null!, person.LastName);
        Add(string.Empty, person.LastName);
        Add(person.FirstName, null!);
        Add(person.FirstName, string.Empty);
        Add(string.Empty, string.Empty);
        Add(null!, null!);
    }
}

public class InvalidEmailOnUpdateTestData : TheoryData<string>
{
    public InvalidEmailOnUpdateTestData()
    {
        Add(null!);
        Add(string.Empty);
        Add(new Faker().Random.AlphaNumeric(9));
    }
}

public class InvalidDateOfBirthOnUpdateTestData : TheoryData<string>
{
    public InvalidDateOfBirthOnUpdateTestData()
    {
        Add(null!);
        Add(string.Empty);
        Add(new Faker().Random.AlphaNumeric(9));
    }
}