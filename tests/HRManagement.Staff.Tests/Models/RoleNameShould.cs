using HRManagement.Modules.Staff.Models.ValueObjects;
using Shouldly;
using Xunit;

namespace HRManagement.Staff.Tests.Models;

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