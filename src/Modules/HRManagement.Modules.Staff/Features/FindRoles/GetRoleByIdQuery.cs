using CSharpFunctionalExtensions;
using HRManagement.Common.Application.Contracts;
using HRManagement.Common.Domain.Models;

namespace HRManagement.Modules.Staff.Features.FindRoles;

public class GetRoleByIdQuery : IQuery<Result<RoleDto, Error>>
{
    public byte Id { get; set; }
}