using CSharpFunctionalExtensions;
using HRManagement.Modules.Personnel.Application.Contracts.Handlers;
using HRManagement.Modules.Personnel.Application.DTOs;

namespace HRManagement.Modules.Personnel.Application.Features.Role;

public class GetRolesQuery : IQuery<Result<List<RoleDto>>>
{
}