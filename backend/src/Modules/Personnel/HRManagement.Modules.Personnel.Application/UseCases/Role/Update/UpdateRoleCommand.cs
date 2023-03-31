using System.Collections.Generic;
using CSharpFunctionalExtensions;
using HRManagement.Common.Domain.Models;
using HRManagement.Modules.Personnel.Application.Contracts.Handlers;

namespace HRManagement.Modules.Personnel.Application.UseCases;

public class UpdateRoleCommand : ICommand<UnitResult<List<Error>>>
{
    public byte Id { get; set; }
    public string Name { get; set; }
    public byte? ReportsToId { get; set; }
}