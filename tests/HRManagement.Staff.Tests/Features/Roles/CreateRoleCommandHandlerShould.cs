using System.Linq.Expressions;
using AutoFixture;
using AutoFixture.AutoMoq;
using CSharpFunctionalExtensions;
using HRManagement.BuildingBlocks.Contracts;
using HRManagement.Modules.Staff;
using HRManagement.Modules.Staff.Data;
using HRManagement.Modules.Staff.Features.Roles.Create;
using HRManagement.Modules.Staff.Features.Roles.Get;
using HRManagement.Modules.Staff.Models;
using HRManagement.Staff.Tests.Features.Builders;
using Moq;
using Shouldly;
using Xunit;

namespace HRManagement.Staff.Tests.Features.Roles;

public class CreateRoleCommandHandlerShould
{
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Dictionary<string, Role> _roles = DatabaseInitializer.SeedDataRoles;
    private readonly CreateRoleCommandHandler _sut;
    private CreateRoleCommand _command;
    private Role _newRole;

    public CreateRoleCommandHandlerShould()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        _mockUnitOfWork = fixture.Freeze<Mock<IUnitOfWork>>();
        _mockCacheService = fixture.Freeze<Mock<ICacheService>>();
        _sut = fixture.Create<CreateRoleCommandHandler>();

        _newRole = _roles["junior-dev"];
        _mockCacheService
            .Setup(service => service.Get<Maybe<Role>>(It.IsAny<string>()))
            .Returns(_roles["lead-dev"]);
        _command = new CreateRoleCommandBuilder().WithName(_newRole.Name.Value).WithManagerId(_newRole.ReportsTo.Id)
            .Build();
        _mockUnitOfWork
            .Setup(d => d.GetRepository<Role, int>().HasMatches(It.IsAny<Expression<Func<Role, bool>>>()))
            .ReturnsAsync(Result.Failure<bool>("No match found."));
        _mockUnitOfWork
            .Setup(d => d.GetRepository<Role, int>().GetByIdAsync(It.IsAny<byte>()))
            .ReturnsAsync(_roles["lead-dev"]);
    }

    [Fact(DisplayName =
        "Succeed when new role is not a duplication, and manager role exists, if manager role is ID provided")]
    public async Task HappyPath()
    {
        var result = await _sut.Handle(_command, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeOfType<RoleDto>();
    }

    [Fact(DisplayName = "Fail when role name is not unique")]
    public async Task Fail_WhenRoleAlreadyExists()
    {
        _mockUnitOfWork
            .Setup(d => d.GetRepository<Role, int>().HasMatches(It.IsAny<Expression<Func<Role, bool>>>()))
            .ReturnsAsync(Result.Success(true));

        var result = await _sut.Handle(_command, CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBeEquivalentTo(DomainErrors.ResourceAlreadyExists());
    }

    [Fact(DisplayName = "Fail when manager does not exist, if manager ID provided")]
    public async Task Fail_WhenManagerDoesNotExist_IfManagerIdProvided()
    {
        _mockCacheService
            .Setup(service => service.Get<Maybe<Role>>(It.IsAny<string>()))
            .Returns(Maybe<Role>.None);

        _mockUnitOfWork
            .Setup(d => d.GetRepository<Role, int>().GetByIdAsync(_command.ReportsToId.Value))
            .ReturnsAsync(Maybe<Role>.None);

        var result = await _sut.Handle(_command, CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBeEquivalentTo(DomainErrors.NotFound(nameof(Role), _command.ReportsToId));
    }

    [Fact(DisplayName = "Fail when role without manager already exist, that would be the CEO")]
    public async Task Fail_WhenEmployeeWithoutManagerAlreadyExists()
    {
        _newRole = _roles["ceo"];
        _command = new CreateRoleCommandBuilder().WithName(_newRole.Name.Value).Build();
        _mockUnitOfWork
            .Setup(d => d.GetRepository<Role, int>().HasMatches(role => role.ReportsTo == null))
            .ReturnsAsync(Result.Success(true));

        var result = await _sut.Handle(_command, CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBeEquivalentTo(DomainErrors.ResourceAlreadyExists());
    }

    [Fact(DisplayName = "Fail when role name is not valid")]
    public async Task Fail_WhenRoleNameIsNotValid()
    {
        _command = new CreateRoleCommandBuilder().WithName("").Build();

        var result = await _sut.Handle(_command, CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBeEquivalentTo(DomainErrors.NullOrEmptyName("Role Name"));
    }
}