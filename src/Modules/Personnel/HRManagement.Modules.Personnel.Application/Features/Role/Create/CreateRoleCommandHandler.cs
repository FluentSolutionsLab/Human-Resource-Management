using CSharpFunctionalExtensions;
using HRManagement.Common.Domain.Models;
using HRManagement.Modules.Personnel.Application.Contracts;
using HRManagement.Modules.Personnel.Application.Contracts.Handlers;
using HRManagement.Modules.Personnel.Application.DTOs;
using HRManagement.Modules.Personnel.Domain;

namespace HRManagement.Modules.Personnel.Application.Features.Role;

public class CreateRoleCommandHandler : ICommandHandler<CreateRoleCommand, Result<RoleDto, List<Error>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateRoleCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<RoleDto, List<Error>>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var rolesWithSaneName = await _unitOfWork.Roles.GetAsync(role => role.Name == request.Name);
        if (rolesWithSaneName.Any()) return new List<Error> {DomainErrors.ResourceAlreadyExists()};

        Domain.Role.Role reportsTo = null;
        if (request.ReportsToId.HasValue)
        {
            reportsTo = await _unitOfWork.Roles.GetByIdAsync(request.ReportsToId.Value);
            if (reportsTo == null)
                return new List<Error> {DomainErrors.NotFound(nameof(Domain.Role.Role), request.ReportsToId)};
        }

        var roleCreation = Domain.Role.Role.Create(request.Name, reportsTo);
        if (roleCreation.IsFailure) return new List<Error> {roleCreation.Error};

        var role = roleCreation.Value;
        await _unitOfWork.Roles.AddAsync(role);
        await _unitOfWork.SaveChangesAsync();

        return RoleDto.MapFromEntity(role);
    }
}