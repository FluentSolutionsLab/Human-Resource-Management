using System.Collections.Generic;
using CSharpFunctionalExtensions;
using HRManagement.Common.Domain.Models;
using HRManagement.Modules.Personnel.Application.Contracts.Handlers;

namespace HRManagement.Modules.Personnel.Application.UseCases;

public class CreateRoleCommand : ICommand<Result<RoleDto, List<Error>>>
{
    public string Name { get; set; }
    public byte? ReportsToId { get; set; }
}