using Bogus;
using HRManagement.Modules.Personnel.Domain;
using Shouldly;
using Xunit;

namespace HRManagement.Personnel.Domain.UnitTests;

public class EmployeeDateOfBirthShould
{
    [Fact]
    public void Fail_OnCreation_IfDateInFuture()
    {
        var dateCreation = ValueDate.Create(new Faker().Date.FutureDateOnly().ToString());

        dateCreation.Error.Count.ShouldBeGreaterThan(0);
    }
}