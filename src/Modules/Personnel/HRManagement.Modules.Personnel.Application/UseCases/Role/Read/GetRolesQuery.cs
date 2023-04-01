using System.Collections.Generic;
using CSharpFunctionalExtensions;
using HRManagement.Common.Application.Handlers;

namespace HRManagement.Modules.Personnel.Application.UseCases;

public class GetRolesQuery : IQuery<Result<List<RoleDto>>>
{
}