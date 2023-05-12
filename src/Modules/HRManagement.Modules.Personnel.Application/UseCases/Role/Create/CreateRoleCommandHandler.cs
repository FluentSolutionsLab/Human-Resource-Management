using System.Collections.Generic;
using HRManagement.Modules.Personnel.Application.Contracts;
using HRManagement.Modules.Personnel.Domain;

namespace HRManagement.Modules.Personnel.Application.UseCases;

public class CreateRoleCommandHandler : ICommandHandler<CreateRoleCommand, Result<RoleDto, List<Error>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateRoleCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<RoleDto, List<Error>>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var rolesWithSaneName = await _unitOfWork.GetRepository<Role, byte>().GetAsync(role => role.Name == request.Name);
        if (rolesWithSaneName.Any()) return new List<Error> {DomainErrors.ResourceAlreadyExists()};

        Role reportsTo = null;
        if (request.ReportsToId.HasValue)
        {
            reportsTo = await _unitOfWork.GetRepository<Role, byte>().GetByIdAsync(request.ReportsToId.Value);
            if (reportsTo == null)
                return new List<Error> {DomainErrors.NotFound(nameof(Role), request.ReportsToId)};
        }

        var roleCreation = Role.Create(request.Name, reportsTo);
        if (roleCreation.IsFailure) return new List<Error> {roleCreation.Error};

        var role = roleCreation.Value;
        await _unitOfWork.GetRepository<Role, byte>().AddAsync(role);
        await _unitOfWork.SaveChangesAsync();

        return role.ToResponseDto();
    }
}