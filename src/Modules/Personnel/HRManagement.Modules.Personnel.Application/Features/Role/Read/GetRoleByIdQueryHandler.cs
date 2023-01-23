using CSharpFunctionalExtensions;
using HRManagement.Common.Domain.Models;
using HRManagement.Modules.Personnel.Application.Contracts;
using HRManagement.Modules.Personnel.Application.Contracts.Handlers;
using HRManagement.Modules.Personnel.Application.DTOs;
using HRManagement.Modules.Personnel.Domain;

namespace HRManagement.Modules.Personnel.Application.Features.Role;

public class GetRoleByIdQueryHandler : IQueryHandler<GetRoleByIdQuery, Result<RoleDto, Error>>
{
    private readonly IRoleRepository _repository;

    public GetRoleByIdQueryHandler(IRoleRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<RoleDto, Error>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        var role = await _repository.GetByIdAsync(request.Id);
        if (role == null) return DomainErrors.NotFound(nameof(Domain.Role.Role), request.Id);

        return RoleDto.MapFromEntity(role);
    }
}