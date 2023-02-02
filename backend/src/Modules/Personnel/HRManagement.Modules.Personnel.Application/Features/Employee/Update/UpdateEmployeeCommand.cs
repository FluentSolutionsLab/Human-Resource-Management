using CSharpFunctionalExtensions;
using HRManagement.Common.Domain.Models;
using HRManagement.Modules.Personnel.Application.Contracts.Handlers;
using HRManagement.Modules.Personnel.Application.DTOs;
using MediatR;

namespace HRManagement.Modules.Personnel.Application.Features.Employee;

public class UpdateEmployeeCommand : ICommand<Result<Unit, List<Error>>>
{
    public string EmployeeId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public string DateOfBirth { get; set; }
    public byte RoleId { get; set; }
    public string ReportsToId { get; set; }

    public static UpdateEmployeeCommand MapFromDto(string id, UpdateEmployeeDto dto)
    {
        return new UpdateEmployeeCommand
        {
            EmployeeId = id,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            EmailAddress = dto.EmailAddress,
            DateOfBirth = dto.DateOfBirth,
            RoleId = dto.RoleId,
            ReportsToId = dto.ManagerId
        };
    }
}