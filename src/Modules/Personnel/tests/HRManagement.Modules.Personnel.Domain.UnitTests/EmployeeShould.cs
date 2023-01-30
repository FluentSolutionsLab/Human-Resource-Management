using Bogus;
using CSharpFunctionalExtensions;
using HRManagement.Common.Domain.Models;
using HRManagement.Modules.Personnel.Domain.Employee;
using Shouldly;
using Xunit;

namespace HRManagement.Modules.Personnel.Domain.UnitTests;

public class EmployeeShould
{
    [Theory]
    [ClassData(typeof(NameEmailAddressOrDOBTestData))]
    public void Fail_OnCreation_IfNameEmailAddressOrDOBMissing(Name name, EmailAddress emailAddress, DateOfBirth dateOfBirth)
    {
        Assert.Throws<ArgumentNullException>(() => Employee.Employee.Create(name, emailAddress, dateOfBirth, null, null));
    }    

    [Theory]
    [ClassData(typeof(NameEmailAddressOrDOBTestData))]
    public void Fail_OnUpdate_IfNameEmailAddressOrDOBMissing(Name name, EmailAddress emailAddress, DateOfBirth dateOfBirth)
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            var employee = BuildFakeEmployee().Value;
            employee.Update(name, emailAddress, dateOfBirth, null, null);
        });
    }

    [Fact]
    public void Fail_OnCreation_IfManagerDoesNotHaveExpectedRole()
    {
        var ceoRole = Role.Role.Create("CEO", null).Value;
        var presidentRole = Role.Role.Create("President", ceoRole).Value;
        var ceo = BuildFakeEmployee(ceoRole).Value;
        var president1 = BuildFakeEmployee(presidentRole, ceo).Value;
        
        var president2Creation = BuildFakeEmployee(presidentRole, president1);
        
        president2Creation.IsFailure.ShouldBeTrue();
    }

    [Fact]
    public void Fail_OnUpdate_IfManagerDoesNotHaveExpectedRole()
    {
        var ceoRole = Role.Role.Create("CEO", null).Value;
        var presidentRole = Role.Role.Create("President", ceoRole).Value;
        var ceo = BuildFakeEmployee(ceoRole).Value;
        var president1 = BuildFakeEmployee(presidentRole, ceo).Value;
        var president2 = BuildFakeEmployee(presidentRole, ceo).Value;

        var president2Update = president2.Update(president2.Name, president2.EmailAddress, president2.DateOfBirth, presidentRole, president1); 
        
        president2Update.IsFailure.ShouldBeTrue();
    }

    [Fact]
    public void SetTerminationDate_OnTermination()
    {
        var employee = BuildFakeEmployee().Value;
        
        employee.Terminate();

        employee.TerminationDate.ShouldNotBeNull();
    }

    private static Result<Employee.Employee, Error> BuildFakeEmployee(Role.Role? role = null, Employee.Employee? manager = null)
    {
        var person = new Faker().Person;
        var employee = Employee.Employee.Create(
            Name.Create(person.FirstName, person.LastName).Value,
            EmailAddress.Create(person.Email).Value,
            DateOfBirth.Create(person.DateOfBirth.ToString("d")).Value, 
            role,
            manager);
        return employee;
    }
}

public class NameEmailAddressOrDOBTestData : TheoryData<Name, EmailAddress, DateOfBirth>
{
    public NameEmailAddressOrDOBTestData()
    {
        var person = new Faker().Person;
        var name = Name.Create(person.FirstName, person.LastName).Value;
        var emailAddress = EmailAddress.Create(person.Email).Value;
        var dateOfBirth = DateOfBirth.Create(person.DateOfBirth.ToString("d")).Value;

        Add(null!, null!, null!);        
        Add(name, null!, null!);        
        Add(null!, emailAddress, null!);        
        Add(null!, null!, dateOfBirth);        
    }
}