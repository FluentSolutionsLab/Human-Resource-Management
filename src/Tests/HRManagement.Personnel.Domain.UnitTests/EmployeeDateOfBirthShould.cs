using Bogus;
using HRManagement.Modules.Personnel.Domain;
using Shouldly;
using Xunit;

namespace HRManagement.Personnel.Domain.UnitTests;

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