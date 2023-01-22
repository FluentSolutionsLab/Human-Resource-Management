using Bogus;
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
        Assert.Throws<ArgumentNullException>(() => Employee.Employee.Create(name, emailAddress, dateOfBirth, null));
    }    

    [Theory]
    [ClassData(typeof(NameEmailAddressOrDOBTestData))]
    public void Fail_OnUpdate_IfNameEmailAddressOrDOBMissing(Name name, EmailAddress emailAddress, DateOfBirth dateOfBirth)
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            var employee = BuildFakeEmployee();
            employee.Update(name, emailAddress, dateOfBirth, null);
        });
    }

    [Fact]
    public void SetTerminationDate_OnTermination()
    {
        var employee = BuildFakeEmployee();
        
        employee.Terminate();

        employee.TerminationDate.ShouldNotBeNull();
    }
    
    private static Employee.Employee BuildFakeEmployee()
    {
        var person = new Faker().Person;
        var employee = Employee.Employee.Create(
            Name.Create(person.FirstName, person.LastName).Value,
            EmailAddress.Create(person.Email).Value,
            DateOfBirth.Create(person.DateOfBirth.ToString("d")).Value, 
            null).Value;
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