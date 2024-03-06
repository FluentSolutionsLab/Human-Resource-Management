using Bogus;
using HRManagement.Modules.Staff.Features.Employees.Update;
using HRManagement.Modules.Staff.Models;
using Moq;

namespace HRManagement.Staff.Tests.Features.Builders;

public class UpdateEmployeeCommandBuilder
{
    private readonly UpdateEmployeeCommand _command = new();
    private readonly Person _person = new Faker().Person;

    public UpdateEmployeeCommandBuilder WithFixture(Employee employee)
    {
        _command.EmployeeId = It.IsNotNull<Guid>().ToString();
        _command.EmailAddress = employee.EmailAddress.Email;
        _command.FirstName = employee.Name.FirstName;
        _command.LastName = employee.Name.LastName;
        _command.DateOfBirth = employee.BirthDate.Date.ToString("d");
        _command.HiringDate = employee.HireDate.Date.ToString();
        _command.ReportsToId = employee.Manager.Id.ToString();
        _command.RoleId = employee.Role.Id;

        return this;
    }

    public UpdateEmployeeCommand Build()
    {
        return _command;
    }
}