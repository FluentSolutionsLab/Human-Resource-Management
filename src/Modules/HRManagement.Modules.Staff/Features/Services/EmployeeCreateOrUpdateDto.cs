using CSharpFunctionalExtensions;
using HRManagement.Modules.Staff.Models;
using HRManagement.Modules.Staff.Models.ValueObjects;

namespace HRManagement.Modules.Staff.Features.Services;

public class EmployeeCreateOrUpdateDto
{
    public Maybe<Guid> EmployeeId { get; set; } = Maybe<Guid>.None;
    public Name Name { get; set; }
    public EmailAddress EmailAddress { get; set; }
    public ValueDate DateOfBirth { get; set; }
    public ValueDate HiringDate { get; set; }
    public int RoleId { get; set; }
    public Maybe<Role> RoleOrNothing { get; set; } = Maybe<Role>.None;
    public Guid ManagerId { get; set; }
    public Maybe<Employee> ManagerOrNothing { get; set; } = Maybe<Employee>.None;
    public Employee Employee { get; set; }
}