using CSharpFunctionalExtensions;
using HRManagement.Common.Domain.Models;
using HRManagement.Modules.Personnel.Application.Contracts.Handlers;

namespace HRManagement.Modules.Personnel.Application.UseCases;

public class GetRoleByIdQuery : IQuery<Result<RoleDto, Error>>
{
    public byte Id { get; set; }
}