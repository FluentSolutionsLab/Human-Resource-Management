using System.Collections.Generic;
using HRManagement.Modules.Personnel.Domain;

namespace HRManagement.Modules.Personnel.Application.UseCases;

public class UpdateRoleCommandHandler : ICommandHandler<UpdateRoleCommand, UnitResult<List<Error>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateRoleCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UnitResult<List<Error>>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _unitOfWork.GetRepository<Role, byte>().GetByIdAsync(request.Id);
        if (role == null) return new List<Error> {DomainErrors.NotFound(nameof(Role), request.Id)};

        var rolesWithSaneName = await _unitOfWork.GetRepository<Role, byte>().GetAsync(x => x.Name.Value == request.Name);
        if (rolesWithSaneName.Any(r => r.Id != request.Id))
            return new List<Error> {DomainErrors.ResourceAlreadyExists()};

        Role reportsTo = null;
        if (request.ReportsToId.HasValue)
        {
            reportsTo = await _unitOfWork.GetRepository<Role, byte>().GetByIdAsync(request.ReportsToId.Value);
            if (reportsTo == null)
                return new List<Error> {DomainErrors.NotFound(nameof(Role), request.ReportsToId)};
        }

        role.Update(RoleName.Create(request.Name).Value, reportsTo);

        _unitOfWork.GetRepository<Role, byte>().Update(role);
        await _unitOfWork.SaveChangesAsync();

        return UnitResult.Success<List<Error>>();
    }
}