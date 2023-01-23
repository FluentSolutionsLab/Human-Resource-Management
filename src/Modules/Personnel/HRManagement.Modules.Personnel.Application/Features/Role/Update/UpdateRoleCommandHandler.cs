using CSharpFunctionalExtensions;
using HRManagement.Common.Domain.Models;
using HRManagement.Modules.Personnel.Application.Contracts;
using HRManagement.Modules.Personnel.Application.Contracts.Handlers;
using HRManagement.Modules.Personnel.Domain;
using MediatR;

namespace HRManagement.Modules.Personnel.Application.Features.Role;

public class UpdateRoleCommandHandler : ICommandHandler<UpdateRoleCommand, Result<Unit, List<Error>>>
{
    private readonly IRoleRepository _repository;

    public UpdateRoleCommandHandler(IRoleRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<Unit, List<Error>>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _repository.GetByIdAsync(request.Id);
        if (role == null) return new List<Error> {DomainErrors.NotFound(nameof(Domain.Role.Role), request.Id)};

        var rolesWithSaneName = await _repository.GetAsync(role => role.Name == request.Name);
        if (rolesWithSaneName.Any(r => r.Id != request.Id)) return new List<Error> {DomainErrors.ResourceAlreadyExists()};
        
        Domain.Role.Role reportsTo = null;
        if (request.ReportsToId.HasValue)
        {
            reportsTo = await _repository.GetByIdAsync(request.ReportsToId.Value);
            if (reportsTo == null)
                return new List<Error> {DomainErrors.NotFound(nameof(Domain.Role.Role), request.ReportsToId)};
        }

        var roleUpdate = role.Update(request.Name, reportsTo);
        if (roleUpdate.IsFailure) return new List<Error> {roleUpdate.Error};

        _repository.Update(roleUpdate.Value);
        await _repository.CommitAsync();

        return Unit.Value;
    }
}