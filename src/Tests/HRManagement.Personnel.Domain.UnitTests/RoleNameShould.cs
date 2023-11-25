using HRManagement.Modules.Personnel.Domain;
using Shouldly;
using Xunit;

namespace HRManagement.Personnel.Domain.UnitTests;

public class RoleNameShould
{
    [Theory]
    [ClassData(typeof(NameNullOrEmptyTestData))]
    public void Fail_OnCreation_IfNameNullOrEmpty(string name)
    {
        var roleCreation = RoleName.Create(name);

        roleCreation.Error.ShouldNotBeNull();
    }
}