using Bogus;
using Shouldly;
using Xunit;

namespace HRManagement.Modules.Personnel.Domain.UnitTests;

public class EmployeeDateOfBirthShould
{
    [Fact]
    public void Fail_OnCreation_IfDateInFuture()
    {
        var dateCreation = DateOfBirth.Create(new Faker().Date.FutureDateOnly().ToString());

        dateCreation.Error.Count.ShouldBeGreaterThan(0);
    }    
}