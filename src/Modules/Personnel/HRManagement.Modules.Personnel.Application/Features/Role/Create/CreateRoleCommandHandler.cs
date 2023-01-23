using CSharpFunctionalExtensions;
using HRManagement.Common.Domain.Models;
using HRManagement.Modules.Personnel.Application.Contracts;
using HRManagement.Modules.Personnel.Application.Contracts.Handlers;
using HRManagement.Modules.Personnel.Application.DTOs;
using HRManagement.Modules.Personnel.Domain;

namespace HRManagement.Modules.Personnel.Application.Features.Role;

public class CreateRoleCommandHandler : ICommandHandler<CreateRoleCommand, Result<RoleDto, List<Error>>>
{
    private readonly IRoleRepository _repository;

    public CreateRoleCommandHandler(IRoleRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<RoleDto, List<Error>>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var rolesWithSaneName = await _repository.GetAsync(role => role.Name == request.Name);
        if (rolesWithSaneName.Any()) return new List<Error> {DomainErrors.ResourceAlreadyExists()};

        Domain.Role.Role reportsTo = null;
        if (request.ReportsToId.HasValue)
        {
            reportsTo = await _repository.GetByIdAsync(request.ReportsToId.Value);
            if (reportsTo == null)
                return new List<Error> {DomainErrors.NotFound(nameof(Domain.Role.Role), request.ReportsToId)};
        }

        var roleCreation = Domain.Role.Role.Create(request.Name, reportsTo);
        if (roleCreation.IsFailure) return new List<Error> {roleCreation.Error};

        var role = roleCreation.Value;
        await _repository.AddAsync(role);
        await _repository.CommitAsync();

        return RoleDto.MapFromEntity(role);
    }
}