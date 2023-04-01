using Bogus;
using Shouldly;
using Xunit;

namespace HRManagement.Modules.Personnel.Domain.UnitTests;

public class EmployeeNameShould
{
    [Theory]
    [ClassData(typeof(NameNotAllowsInvalidFirstAndLatNameTestData))]
    public void Fail_OnCreation_IfFirstAndLastNameInvalid(string firstName, string lastName)
    {
        var nameCreation = Name.Create(firstName, lastName);
        
        nameCreation.Error.Count.ShouldBeGreaterThan(0);
    }
}

public class NameNotAllowsInvalidFirstAndLatNameTestData : TheoryData<string, string>
{
    public NameNotAllowsInvalidFirstAndLatNameTestData()
    {
        var randomAlphaNumeric = new Faker().Random.AlphaNumeric(9);
        var validFirstName = new Faker().Person.FirstName;
        var validLastName = new Faker().Person.LastName;
        Add(null!, null!);
        Add(null!, validLastName);
        Add(string.Empty, validLastName);
        Add(validFirstName, null!);
        Add(validFirstName, string.Empty);
        Add(validFirstName, randomAlphaNumeric);
        Add(randomAlphaNumeric, validLastName);
    }
}