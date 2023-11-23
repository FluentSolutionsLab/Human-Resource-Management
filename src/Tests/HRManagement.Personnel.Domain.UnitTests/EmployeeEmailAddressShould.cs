using Bogus;
using HRManagement.Modules.Personnel.Domain;
using Shouldly;
using Xunit;

namespace HRManagement.Personnel.Domain.UnitTests;

public class EmployeeEmailAddressShould
{
    [Theory]
    [ClassData(typeof(EmailAddressInvalidTestData))]
    public void Fail_OnCreation_IfInvalid(string emailAddress)
    {
        var emailCreation = EmailAddress.Create(emailAddress);

        emailCreation.Error.ShouldNotBeNull();
    }

    [Fact]
    public void Succeed_OnCreation_IfValid()
    {
        var emailCreation = EmailAddress.Create("fake@gmail.com");

        emailCreation.IsSuccess.ShouldBeTrue();
    }
}

public class EmailAddressInvalidTestData : TheoryData<string>
{
    public EmailAddressInvalidTestData()
    {
        Add(null);
        Add(string.Empty);
        Add(new Faker().Random.AlphaNumeric(9));
    }
}