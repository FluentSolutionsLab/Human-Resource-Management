using HRManagement.BuildingBlocks.Models;
using HRManagement.BuildingBlocks.Utilities;
using HRManagement.Modules.Staff.Features.Employees.Create;
using HRManagement.Modules.Staff.Features.Employees.Get;
using HRManagement.Modules.Staff.Features.Employees.Update;
using HRManagement.Modules.Staff.Features.Roles.Create;
using HRManagement.Modules.Staff.Features.Roles.Get;
using HRManagement.Modules.Staff.Features.Roles.Update;
using HRManagement.Modules.Staff.Models;

namespace HRManagement.Modules.Staff.Features;

public static class Mapping
{
    public static EmployeeDto ToResponseDto(this Employee employee)
    {
        var manager = employee.Manager != null
            ? new EmployeeManagerDto
            {
                Id = employee.Manager.Id.ToString(),
                FirstName = employee.Manager.Name.FirstName,
                LastName = employee.Manager.Name.LastName,
                Role = employee.Manager.Role.ToString()
            }
            : null;

        return new EmployeeDto
        {
            Id = employee.Id.ToString(),
            FirstName = employee.Name.FirstName,
            LastName = employee.Name.LastName,
            EmailAddress = employee.EmailAddress.Email,
            DateOfBirth = employee.BirthDate.Date.ToISO8601String(),
            HireDate = employee.HireDate.Date.ToISO8601String(),
            Role = employee.Role.ToString(),
            Manager = manager
        };
    }

    public static CreateEmployeeCommand ToHireEmployeeCommand(this CreateEmployeeDto dto)
    {
        return new CreateEmployeeCommand
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            EmailAddress = dto.EmailAddress,
            DateOfBirth = dto.DateOfBirth,
            HiringDate = dto.HiringDate,
            RoleId = dto.RoleId,
            ReportsToId = dto.ManagerId
        };
    }

    public static UpdateEmployeeCommand ToUpdateEmployeeCommand(this UpdateEmployeeDto dto, string id)
    {
        return new UpdateEmployeeCommand
        {
            EmployeeId = id,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            EmailAddress = dto.EmailAddress,
            DateOfBirth = dto.DateOfBirth,
            HiringDate = dto.HiringDate,
            RoleId = dto.RoleId,
            ReportsToId = dto.ManagerId
        };
    }

    public static PagedList<EmployeeDto> ToResponseDto(this PagedList<Employee> employees)
    {
        var dtos = employees.Select(x => x.ToResponseDto()).ToList();
        return new PagedList<EmployeeDto>(dtos, employees.TotalCount, employees.CurrentPage, employees.PageSize);
    }

    public static CreateRoleCommand ToCreateRoleCommand(this CreateRoleDto dto)
    {
        return new CreateRoleCommand
        {
            Name = dto.Name,
            ReportsToId = dto.ReportsToId
        };
    }

    public static RoleDto ToResponseDto(this Role role)
    {
        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name.Value,
            ReportsTo = role.ReportsTo?.Name.Value
        };
    }

    public static UpdateRoleCommand ToUpdateRoleCommand(this UpdateRoleDto dto, byte id)
    {
        return new UpdateRoleCommand
        {
            Id = id,
            Name = dto.Name,
            ReportsToId = dto.ReportsToId
        };
    }
}