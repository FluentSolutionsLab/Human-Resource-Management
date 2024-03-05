using CSharpFunctionalExtensions;
using HRManagement.Common.Application.Contracts;
using HRManagement.Common.Domain.Models;
using HRManagement.Modules.Staff.Features.FindEmployees;

namespace HRManagement.Modules.Staff.Features.CreateEmployee;

public class CreateEmployeeCommand : ICommand<Result<EmployeeDto, Error>>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public string DateOfBirth { get; set; }
    public string HiringDate { get; set; }
    public byte RoleId { get; set; }
    public string ReportsToId { get; set; }
}