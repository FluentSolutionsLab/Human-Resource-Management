using AutoFixture;
using AutoFixture.AutoMoq;
using CSharpFunctionalExtensions;
using HRManagement.BuildingBlocks.Models;
using HRManagement.Modules.Staff;
using HRManagement.Modules.Staff.Data;
using HRManagement.Modules.Staff.Features.Services;
using HRManagement.Modules.Staff.Features.UpdateEmployee;
using HRManagement.Modules.Staff.Models;
using HRManagement.Modules.Staff.Models.ValueObjects;
using HRManagement.Staff.Tests.Features.Builders;
using Moq;
using Shouldly;
using Xunit;

namespace HRManagement.Staff.Tests.Features.Employees;

public class UpdateEmployeeCommandHandlerShould
{
    private readonly UpdateEmployeeCommand _command;
    private readonly Employee _employee;
    private readonly Mock<IEmployeeService> _mockEmployeeService;
    private readonly Dictionary<string, Role> _roles = DatabaseInitializer.SeedDataRoles;
    private readonly UpdateEmployeeCommandHandler _sut;

    public UpdateEmployeeCommandHandlerShould()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var mockRoleService = fixture.Freeze<Mock<IRoleService>>();
        _mockEmployeeService = fixture.Freeze<Mock<IEmployeeService>>();
        _sut = fixture.Create<UpdateEmployeeCommandHandler>();

        var role = _roles["president"];
        var manager = new EmployeeBuilder().WithFixture().Build();
        _employee = new EmployeeBuilder().WithFixture().WithRole(role).WithManager(manager).Build();
        _command = new UpdateEmployeeCommandBuilder().WithFixture(_employee).Build();

        _mockEmployeeService
            .SetupSequence(x => x.CheckIfEmployeeExists(It.IsAny<Guid>()))
            .ReturnsAsync(true)
            .ReturnsAsync(true);
        mockRoleService
            .Setup(x => x.CheckIfRoleExists(It.IsAny<byte>()))
            .ReturnsAsync(true);

        _mockEmployeeService
            .Setup(x => x.ValidateRequest(_command))
            .Returns(new EmployeeCreateOrUpdateDtoBuilder().WithFixture(_command).Build());
        _mockEmployeeService
            .Setup(x => x.GetEmployee(It.IsAny<EmployeeCreateOrUpdateDto>()))
            .Returns(new EmployeeCreateOrUpdateDtoBuilder().WithFixture(_command).WithEmployee(_employee).Build());
        mockRoleService
            .Setup(x => x.GetRole(It.IsAny<EmployeeCreateOrUpdateDto>()))
            .Returns(new EmployeeCreateOrUpdateDtoBuilder().WithFixture(_command).WithEmployee(_employee).WithRole(role)
                .Build());
        _mockEmployeeService
            .Setup(x => x.GetManager(It.IsAny<EmployeeCreateOrUpdateDto>()))
            .Returns(new EmployeeCreateOrUpdateDtoBuilder().WithFixture(_command).WithEmployee(_employee).WithRole(role)
                .WithManager(manager).Build());
        _mockEmployeeService
            .Setup(x => x.UpdateEmployee(It.IsAny<EmployeeCreateOrUpdateDto>()))
            .Returns(new EmployeeCreateOrUpdateDtoBuilder().WithFixture(_command).WithEmployee(_employee).WithRole(role)
                .WithManager(manager).Build());
        _mockEmployeeService
            .Setup(x => x.StoreUpdatedEmployee(It.IsAny<Result<EmployeeCreateOrUpdateDto, Error>>()))
            .Callback(() =>
            {
                var name = Name.Create($"{_command.FirstName} Updated", _command.LastName).Value;
                var email = EmailAddress.Create(_command.EmailAddress).Value;
                var dateOfBirth = ValueDate.Create(_command.DateOfBirth).Value;
                var hiringDate = ValueDate.Create(_command.HiringDate).Value;
                _employee.Update(name, email, dateOfBirth, hiringDate, role, manager);
            });
    }

    [Fact(DisplayName = "Succeed when request is valid")]
    public async Task UpdateEmployee_WhenEmployeeExists()
    {
        var result = await _sut.Handle(_command, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        _employee.Name.FirstName.ShouldEndWith(" Updated");
    }

    [Fact(DisplayName = "Fail when request is not valid")]
    public async Task ReturnError_WhenRequestNotValid()
    {
        _mockEmployeeService
            .Setup(x => x.ValidateRequest(_command))
            .Returns(new Error("code", "error"));

        var result = await _sut.Handle(_command, CancellationToken.None);

        result.Error.ShouldNotBeNull();
    }


    [Fact(DisplayName = "Fail when employee not found")]
    public async Task ReturnError_WhenEmployeeNotFound()
    {
        _mockEmployeeService
            .Setup(x => x.CheckIfEmployeeExists(It.IsAny<Guid>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(_command, CancellationToken.None);

        result.Error.ShouldNotBeNull();
        result.Error.ShouldBeEquivalentTo(DomainErrors.NotFound(nameof(Employee), _command.EmployeeId));
    }
}