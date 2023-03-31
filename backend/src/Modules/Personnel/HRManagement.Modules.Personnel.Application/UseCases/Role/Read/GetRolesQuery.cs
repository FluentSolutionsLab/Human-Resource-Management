using System.Collections.Generic;
using CSharpFunctionalExtensions;
using HRManagement.Modules.Personnel.Application.Contracts.Handlers;

namespace HRManagement.Modules.Personnel.Application.UseCases;

public class GetRolesQuery : IQuery<Result<List<RoleDto>>>
{
}