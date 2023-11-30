using HRManagement.Common.Domain.Models;
using HRManagement.Modules.Personnel.Application.UseCases.Services;
using HRManagement.Modules.Personnel.Persistence;
using HRManagement.Personnel.Application.UnitTests.Builders;

namespace HRManagement.Personnel.Application.UnitTests.Employees;

public class HireEmployeeCommandHandlerShould
{
    private readonly Mock<IEmployeeService> _mockEmployeeService;
    private readonly Dictionary<string, Role> _roles = DatabaseInitializer.SeedDataRoles;
    private readonly HireEmployeeCommandHandler _sut;
    private readonly HireEmployeeCommand _command;

    public HireEmployeeCommandHandlerShould()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        _mockEmployeeService = fixture.Freeze<Mock<IEmployeeService>>();
        var mockRoleService = fixture.Freeze<Mock<IRoleService>>();
        _sut = fixture.Create<HireEmployeeCommandHandler>();

        _command = new HireEmployeeCommandBuilder().WithFixture().Build();
        var role = _roles["president"];
        var manager = new EmployeeBuilder().WithFixture().Build();
        var employee = new EmployeeBuilder().WithFixture().WithRole(role).WithManager(manager).Build();

        mockRoleService
            .Setup(x => x.CheckIfRoleExists(It.IsAny<byte>()))
            .ReturnsAsync(true);
        _mockEmployeeService
            .Setup(x => x.CheckIfEmployeeExists(It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _mockEmployeeService
            .Setup(x => x.CheckIfEmployeeIsUnique(It.IsAny<EmployeeCreateOrUpdateDto>()))
            .ReturnsAsync(true);

        _mockEmployeeService
            .Setup(x => x.ValidateRequest(_command))
            .Returns(new EmployeeCreateOrUpdateDtoBuilder().WithFixture(_command).Build());
        mockRoleService
            .Setup(x => x.GetRole(It.IsAny<EmployeeCreateOrUpdateDto>()))
            .Returns(new EmployeeCreateOrUpdateDtoBuilder().WithFixture(_command).WithRole(role).Build());
        _mockEmployeeService
            .Setup(x => x.GetManager(It.IsAny<EmployeeCreateOrUpdateDto>()))
            .Returns(new EmployeeCreateOrUpdateDtoBuilder().WithFixture(_command).WithRole(role).WithManager(manager).Build());
        _mockEmployeeService
            .Setup(x => x.CreateEmployee(It.IsAny<EmployeeCreateOrUpdateDto>()))
            .Returns(new EmployeeCreateOrUpdateDtoBuilder().WithFixture(_command).WithRole(role).WithManager(manager).WithEmployee(employee).Build());
    }

    [Fact(DisplayName = "Succeed when request is valid")]
    public async Task Succeed_WhenPayloadIsValid()
    {
        var result = await _sut.Handle(_command, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeOfType<EmployeeDto>();
    }

    [Fact(DisplayName = "Fail when request is not valid")]
    public async Task Fail_WhenRequestNotValid()
    {
        _mockEmployeeService
            .Setup(x => x.ValidateRequest(_command))
            .Returns(new Error("code", "error"));

        var result = await _sut.Handle(_command, CancellationToken.None);

        result.Error.ShouldNotBeNull();
    }

    [Fact(DisplayName = "Fail when the employee already exists")]
    public async Task Fail_WhenEmployeeAlreadyExists()
    {
        _mockEmployeeService
            .Setup(x => x.CheckIfEmployeeIsUnique(It.IsAny<EmployeeCreateOrUpdateDto>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(_command, CancellationToken.None);

        result.Error.ShouldNotBeNull();
        result.Error.ShouldBeEquivalentTo(DomainErrors.ResourceAlreadyExists());
    }

    [Fact(DisplayName = "Fail when the manager does not exist")]
    public async Task Fail_WhenManagerIdIsInvalid()
    {
        _mockEmployeeService
            .Setup(x => x.CheckIfEmployeeExists(It.IsAny<Guid>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(_command, CancellationToken.None);

        result.Error.ShouldNotBeNull();
        result.Error.ShouldBeEquivalentTo(DomainErrors.NotFound(nameof(Employee), _command.ReportsToId));
    }
}