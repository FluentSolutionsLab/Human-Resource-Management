using HRManagement.Modules.Personnel.Application.UseCases.Services;

namespace HRManagement.Personnel.Application.UnitTests.Builders;

public class EmployeeCreateOrUpdateDtoBuilder
{
    private readonly EmployeeCreateOrUpdateDto _dto = new();

    public EmployeeCreateOrUpdateDtoBuilder WithFixture(HireEmployeeCommand command)
    {
        _dto.Name = Name.Create(command.FirstName, command.LastName).Value;
        _dto.EmailAddress = EmailAddress.Create(command.EmailAddress).Value;
        _dto.DateOfBirth = ValueDate.Create(command.DateOfBirth).Value;
        _dto.HiringDate = ValueDate.Create(command.HiringDate).Value;
        _dto.RoleId = command.RoleId;
        return this;
    }

    public EmployeeCreateOrUpdateDtoBuilder WithFixture(UpdateEmployeeCommand command)
    {
        _dto.EmployeeId = Guid.Parse(command.EmployeeId);
        _dto.Name = Name.Create(command.FirstName, command.LastName).Value;
        _dto.EmailAddress = EmailAddress.Create(command.EmailAddress).Value;
        _dto.DateOfBirth = ValueDate.Create(command.DateOfBirth).Value;
        _dto.HiringDate = ValueDate.Create(command.HiringDate).Value;
        _dto.RoleId = command.RoleId;
        return this;
    }
    
    public EmployeeCreateOrUpdateDtoBuilder WithRole(Role role)
    {
        _dto.RoleOrNothing = role;
        return this;
    }

    public EmployeeCreateOrUpdateDtoBuilder WithManager(Employee employee)
    {
        _dto.ManagerOrNothing = employee;
        return this;
    }

    public EmployeeCreateOrUpdateDtoBuilder WithEmployee(Employee employee)
    {
        _dto.Employee = employee;
        return this;
    }
    
    public EmployeeCreateOrUpdateDto Build()
    {
        return _dto;
    }
}