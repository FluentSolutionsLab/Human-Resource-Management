using System.Collections.Generic;
using CSharpFunctionalExtensions;
using HRManagement.Common.Application.Handlers;
using HRManagement.Common.Domain.Models;

namespace HRManagement.Modules.Personnel.Application.UseCases;

public class CreateRoleCommand : ICommand<Result<RoleDto, List<Error>>>
{
    public string Name { get; set; }
    public byte? ReportsToId { get; set; }
}