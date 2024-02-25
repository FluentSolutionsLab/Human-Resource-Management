using Bogus;
using HRManagement.Modules.Staff.Models;
using Shouldly;
using Xunit;

namespace HRManagement.Staff.Tests.Models;

public class EmployeeNameShould
{
    [Theory]
    [ClassData(typeof(NameNotAllowsInvalidFirstAndLatNameTestData))]
    public void Fail_OnCreation_IfFirstAndLastNameInvalid(string firstName, string lastName)
    {
        var nameCreation = Name.Create(firstName, lastName);

        nameCreation.Error.ShouldNotBeNull();
    }
}

public class NameNotAllowsInvalidFirstAndLatNameTestData : TheoryData<string, string>
{
    public NameNotAllowsInvalidFirstAndLatNameTestData()
    {
        var randomAlphaNumeric = new Faker().Random.AlphaNumeric(9);
        var validFirstName = new Faker().Person.FirstName;
        var validLastName = new Faker().Person.LastName;
        Add(null, null);
        Add(null, validLastName);
        Add(string.Empty, validLastName);
        Add(validFirstName, null);
        Add(validFirstName, string.Empty);
        Add(validFirstName, randomAlphaNumeric);
        Add(randomAlphaNumeric, validLastName);
    }
}