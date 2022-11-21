using Bogus;
using HRManagement.Modules.Personnel.Domain.Employee;
using Shouldly;
using Xunit;

namespace HRManagement.Modules.Personnel.Domain.UnitTests;

public class EmployeeEmailAddressShould
{
    [Theory]
    [ClassData(typeof(EmailAddressNotInvalidTestData))]
    public void Fail_OnCreation_IfInvalid(string emailAddress)
    {
        var emailCreation = EmailAddress.Create(emailAddress);
        
        emailCreation.Error.ShouldNotBeNull();
    }
}

public class EmailAddressNotInvalidTestData : TheoryData<string>
{
    public EmailAddressNotInvalidTestData()
    {
        var randomAlphaNumeric = new Faker().Random.AlphaNumeric(9);
        Add(null!);
        Add(string.Empty);
        Add(randomAlphaNumeric);
    }
}