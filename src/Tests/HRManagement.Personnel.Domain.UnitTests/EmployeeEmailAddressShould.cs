using Bogus;
using HRManagement.Modules.Personnel.Domain;
using Shouldly;
using Xunit;

namespace HRManagement.Personnel.Domain.UnitTests;

public class EmployeeEmailAddressShould
{
    [Theory]
    [ClassData(typeof(EmailAddressNotInvalidTestData))]
    public void Fail_OnCreation_IfInvalid(string emailAddress)
    {
        var emailCreation = EmailAddress.Create(emailAddress);
        
        emailCreation.Error.Count.ShouldBeGreaterThan(0);
    }
}

public class EmailAddressNotInvalidTestData : TheoryData<string>
{
    public EmailAddressNotInvalidTestData()
    {
        Add(null!);
        Add(string.Empty);
        Add(new Faker().Random.AlphaNumeric(9));
    }
}