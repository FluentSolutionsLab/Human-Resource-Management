using CSharpFunctionalExtensions;
using HRManagement.Modules.Staff.Domain;
using Xunit;

namespace HRManagement.Staff.Domain.UnitTests;

public class RoleShould
{
    [Fact]
    public void Fail_OnCreation_IfNameNullOrEmpty()
    {
        Assert.Throws<ArgumentNullException>(() => Role.Create(Maybe<RoleName>.None, Maybe.None));
    }

    [Fact]
    public void Fail_OnUpdate_IfNameNullOrEmpty()
    {
        var role = Role.Create(RoleName.Create("name").Value, null).Value;

        Assert.Throws<ArgumentNullException>(() => role.Update(Maybe<RoleName>.None, Maybe<Role>.None));
    }
}

public class NameNullOrEmptyTestData : TheoryData<string>
{
    public NameNullOrEmptyTestData()
    {
        Add(string.Empty);
        Add(" ");
        Add(null!);
    }
}