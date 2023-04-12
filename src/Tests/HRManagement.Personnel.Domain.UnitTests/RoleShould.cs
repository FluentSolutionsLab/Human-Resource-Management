﻿using HRManagement.Modules.Personnel.Domain;
using Shouldly;
using Xunit;

namespace HRManagement.Personnel.Domain.UnitTests;

public class RoleShould
{
    [Theory]
    [ClassData(typeof(NameNullOrEmptyTestData))]
    public void Fail_OnCreation_IfNameNullOrEmpty(string name)
    {
        var roleCreation = Role.Create(name, null);

        roleCreation.Error.ShouldNotBeNull();
    }

    [Theory]
    [ClassData(typeof(NameNullOrEmptyTestData))]
    public void Fail_OnUpdate_IfNameNullOrEmpty(string name)
    {
        var role = Role.Create("name", null).Value;

        var roleUpdate = role.Update(name, null);

        roleUpdate.Error.ShouldNotBeNull();
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