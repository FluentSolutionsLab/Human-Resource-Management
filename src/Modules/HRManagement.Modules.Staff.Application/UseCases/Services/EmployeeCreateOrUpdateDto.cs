using System;
using HRManagement.Modules.Staff.Domain;

namespace HRManagement.Modules.Staff.Application.UseCases.Services;

public class EmployeeCreateOrUpdateDto
{
    public Maybe<Guid> EmployeeId { get; set; } = Maybe<Guid>.None;
    public Name Name { get; set; }
    public EmailAddress EmailAddress { get; set; }
    public ValueDate DateOfBirth { get; set; }
    public ValueDate HiringDate { get; set; }
    public byte RoleId { get; set; }
    public Maybe<Role> RoleOrNothing { get; set; } = Maybe<Role>.None;
    public Guid ManagerId { get; set; }
    public Maybe<Employee> ManagerOrNothing { get; set; } = Maybe<Employee>.None;
    public Employee Employee { get; set; }
}