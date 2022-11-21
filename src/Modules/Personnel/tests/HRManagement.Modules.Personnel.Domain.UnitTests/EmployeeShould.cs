using Bogus;
using HRManagement.Modules.Personnel.Domain.Employee;
using Xunit;

namespace HRManagement.Modules.Personnel.Domain.UnitTests;

public class EmployeeShould
{
    [Theory]
    [ClassData(typeof(NameEmailAddressOrDOBTestData))]
    public void Fail_OnCreation_IfNameEmailAddressOrDOBMissing(Name name, EmailAddress emailAddress, DateOfBirth dateOfBirth)
    {
        Assert.Throws<ArgumentNullException>(() => Employee.Employee.Create(name, emailAddress, dateOfBirth));
    }    
}

public class NameEmailAddressOrDOBTestData : TheoryData<Name, EmailAddress, DateOfBirth>
{
    public NameEmailAddressOrDOBTestData()
    {
        var person = new Faker().Person;
        var name = Name.Create(person.FirstName, person.LastName).Value;
        var emailAddress = EmailAddress.Create(person.Email).Value;
        var dateOfBirth = DateOfBirth.Create(DateOnly.FromDateTime(person.DateOfBirth)).Value;

        Add(null!, null!, null!);        
        Add(name, null!, null!);        
        Add(null!, emailAddress, null!);        
        Add(null!, null!, dateOfBirth);        
    }
}