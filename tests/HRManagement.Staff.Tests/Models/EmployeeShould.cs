using Bogus;
using CSharpFunctionalExtensions;
using HRManagement.Common.Domain.Models;
using HRManagement.Modules.Staff.Models;
using HRManagement.Modules.Staff.Models.ValueObjects;
using Shouldly;
using Xunit;

namespace HRManagement.Staff.Tests.Models;

public class EmployeeShould
{
    [Theory]
    [ClassData(typeof(NameEmailAddressDOBOrHiringDateTestData))]
    public void Fail_OnCreation_IfNameEmailAddressOrDOBMissing(Name name, EmailAddress emailAddress,
        ValueDate birthDate, ValueDate hiringDate)
    {
        Assert.Throws<ArgumentNullException>(() =>
            Employee.Create(name, emailAddress, birthDate, hiringDate, null, null));
    }

    [Theory]
    [ClassData(typeof(NameEmailAddressDOBOrHiringDateTestData))]
    public void Fail_OnUpdate_IfNameEmailAddressOrDOBMissing(Name name, EmailAddress emailAddress, ValueDate birthDate,
        ValueDate hiringDate)
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            var employee = BuildFakeEmployee().Value;
            employee.Update(name, emailAddress, birthDate, hiringDate, null, null);
        });
    }

    [Fact]
    public void Fail_OnCreation_IfManagerDoesNotHaveExpectedRole()
    {
        var ceoRole = Role.Create(RoleName.Create("CEO").Value, null).Value;
        var presidentRole = Role.Create(RoleName.Create("President").Value, ceoRole).Value;
        var ceo = BuildFakeEmployee(ceoRole).Value;
        var president1 = BuildFakeEmployee(presidentRole, ceo).Value;

        var president2Creation = BuildFakeEmployee(presidentRole, president1);

        president2Creation.IsFailure.ShouldBeTrue();
    }

    [Fact]
    public void Fail_OnUpdate_IfManagerDoesNotHaveExpectedRole()
    {
        var ceoRole = Role.Create(RoleName.Create("CEO").Value, null).Value;
        var presidentRole = Role.Create(RoleName.Create("President").Value, ceoRole).Value;
        var ceo = BuildFakeEmployee(ceoRole).Value;
        var president1 = BuildFakeEmployee(presidentRole, ceo).Value;
        var president2 = BuildFakeEmployee(presidentRole, ceo).Value;

        var president2Update = president2.Update(
            president2.Name, president2.EmailAddress, president2.BirthDate, president2.HireDate, presidentRole,
            president1);

        president2Update.IsFailure.ShouldBeTrue();
    }

    [Fact]
    public void SetTerminationDate_OnTermination()
    {
        var employee = BuildFakeEmployee().Value;
        var terminationDate = ValueDate.Create(new Faker().Date.Recent().ToString("d")).Value;

        employee.Terminate(terminationDate);

        employee.TerminationDate.ShouldNotBeNull();
    }

    private static Result<Employee, Error> BuildFakeEmployee(Role role = null, Employee manager = null)
    {
        var hiringDate = new Faker().Date.Past(15);
        var person = new Faker().Person;
        var employee = Employee.Create(
            Name.Create(person.FirstName, person.LastName).Value,
            EmailAddress.Create(person.Email).Value,
            ValueDate.Create(person.DateOfBirth.ToString("d")).Value,
            ValueDate.Create(hiringDate.ToString("d")).Value,
            role,
            manager);
        return employee;
    }
}

public class NameEmailAddressDOBOrHiringDateTestData : TheoryData<Name, EmailAddress, ValueDate, ValueDate>
{
    public NameEmailAddressDOBOrHiringDateTestData()
    {
        var person = new Faker().Person;
        var name = Name.Create(person.FirstName, person.LastName).Value;
        var emailAddress = EmailAddress.Create(person.Email).Value;
        var dateOfBirth = ValueDate.Create(person.DateOfBirth.ToString("d")).Value;
        var hiringDate = ValueDate.Create(new Faker().Date.Past(15).ToString("d")).Value;

        Add(null!, null!, null!, null!);
        Add(name, null!, null!, null!);
        Add(null!, emailAddress, null!, null!);
        Add(null!, null!, dateOfBirth, null!);
        Add(null!, null!, null!, hiringDate);
    }
}