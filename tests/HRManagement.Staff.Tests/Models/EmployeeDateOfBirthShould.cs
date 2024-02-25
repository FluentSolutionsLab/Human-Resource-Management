using Bogus;
using HRManagement.Modules.Staff.Models.ValueObjects;
using Shouldly;
using Xunit;

namespace HRManagement.Staff.Tests.Models;

public class EmployeeDateOfBirthShould
{
    [Theory]
    [ClassData(typeof(EmployeeDateOfBirthInvalidTestData))]
    public void Fail_OnCreation_IfDateInvalid(string date)
    {
        var dateCreation = ValueDate.Create(date);

        dateCreation.Error.ShouldNotBeNull();
    }
}

public class EmployeeDateOfBirthInvalidTestData : TheoryData<string>
{
    public EmployeeDateOfBirthInvalidTestData()
    {
        Add(null);
        Add(string.Empty);
        Add(new Faker().Random.AlphaNumeric(9));
        Add(new Faker().Date.FutureDateOnly().ToString());
    }
}