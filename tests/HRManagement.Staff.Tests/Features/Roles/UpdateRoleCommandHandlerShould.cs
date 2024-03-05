using System.Linq.Expressions;
using AutoFixture;
using AutoFixture.AutoMoq;
using CSharpFunctionalExtensions;
using HRManagement.BuildingBlocks.Contracts;
using HRManagement.Modules.Staff;
using HRManagement.Modules.Staff.Data;
using HRManagement.Modules.Staff.Features.UpdateRole;
using HRManagement.Modules.Staff.Models;
using HRManagement.Staff.Tests.Features.Builders;
using Moq;
using Shouldly;
using Xunit;

namespace HRManagement.Staff.Tests.Features.Roles;

public class UpdateRoleCommandHandlerShould
{
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Role _newRole;
    private readonly Dictionary<string, Role> _roles = DatabaseInitializer.SeedDataRoles;
    private readonly UpdateRoleCommandHandler _sut;
    private UpdateRoleCommand _command;

    public UpdateRoleCommandHandlerShould()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        _mockUnitOfWork = fixture.Freeze<Mock<IUnitOfWork>>();
        _mockCacheService = fixture.Freeze<Mock<ICacheService>>();
        _sut = fixture.Create<UpdateRoleCommandHandler>();

        _newRole = _roles["junior-dev"];
        _mockCacheService
            .SetupSequence(service => service.Get<Maybe<Role>>(It.IsAny<string>()))
            .Returns(_newRole)
            .Returns(_newRole)
            .Returns(_roles["lead-dev"])
            .Returns(_roles["lead-dev"]);
        _mockUnitOfWork
            .Setup(d => d.GetRepository<Role, int>().HasMatches(It.IsAny<Expression<Func<Role, bool>>>()))
            .ReturnsAsync(Result.Failure<bool>("No match found."));
        _command = new UpdateRoleCommandBuilder()
            .WithId(1)
            .WithName(_newRole.Name.Value)
            .WithManagerId(_newRole.ReportsTo.Id)
            .Build();
    }

    [Fact(DisplayName =
        "Succeed when role name is valid, unique, and manager role exists, if manager role is ID provided")]
    public async Task HappyPath()
    {
        var result = await _sut.Handle(_command, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        _mockUnitOfWork.Verify(work => work.GetRepository<Role, int>().Update(It.IsAny<Role>()), Times.Once);
        _mockUnitOfWork.Verify(work => work.SaveChangesAsync(), Times.Once);
    }

    [Fact(DisplayName = "Fail when role name is not valid")]
    public async Task RoleNameIsNotValid()
    {
        _command = new UpdateRoleCommandBuilder().WithName(string.Empty).Build();

        var result = await _sut.Handle(_command, CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBeEquivalentTo(DomainErrors.NullOrEmptyName("Role Name"));
    }

    [Fact(DisplayName = "Fail when role name is not unique")]
    public async Task RoleNameIsNotUnique()
    {
        _mockUnitOfWork
            .Setup(d => d.GetRepository<Role, int>().HasMatches(It.IsAny<Expression<Func<Role, bool>>>()))
            .ReturnsAsync(Result.Success(true));

        var result = await _sut.Handle(_command, CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBeEquivalentTo(DomainErrors.ResourceAlreadyExists());
    }

    [Fact(DisplayName = "Fail when no role found for provided ID")]
    public async Task RoleNotFound()
    {
        _mockCacheService
            .Setup(service => service.Get<Maybe<Role>>(It.IsAny<string>()))
            .Returns(Maybe<Role>.None);

        var result = await _sut.Handle(_command, CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBeEquivalentTo(DomainErrors.NotFound(nameof(Role), _command.Id));
    }

    [Fact(DisplayName = "Fail when no manager role found for provided ID")]
    public async Task ManagerRoleNotFoundForIdProvided()
    {
        _mockCacheService
            .SetupSequence(service => service.Get<Maybe<Role>>(It.IsAny<string>()))
            .Returns(_newRole)
            .Returns(_newRole)
            .Returns(Maybe<Role>.None);

        var result = await _sut.Handle(_command, CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBeEquivalentTo(DomainErrors.NotFound(nameof(Role), _command.ReportsToId));
    }
}