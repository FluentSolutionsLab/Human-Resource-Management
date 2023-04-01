using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using HRManagement.Common.Application.Contracts;
using HRManagement.Common.Domain.Models;
using HRManagement.Modules.Personnel.Application.Contracts;
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
        var role = await _unitOfWork.Roles.GetByIdAsync(request.Id);
        if (role == null) return new List<Error> {DomainErrors.NotFound(nameof(Role), request.Id)};

        var rolesWithSaneName = await _unitOfWork.Roles.GetAsync(x => x.Name == request.Name);
        if (rolesWithSaneName.Any(r => r.Id != request.Id)) return new List<Error> {DomainErrors.ResourceAlreadyExists()};
        
        Role reportsTo = null;
        if (request.ReportsToId.HasValue)
        {
            reportsTo = await _unitOfWork.Roles.GetByIdAsync(request.ReportsToId.Value);
            if (reportsTo == null)
                return new List<Error> {DomainErrors.NotFound(nameof(Role), request.ReportsToId)};
        }

        var roleUpdate = role.Update(request.Name, reportsTo);
        if (roleUpdate.IsFailure) return new List<Error> {roleUpdate.Error};

        _unitOfWork.Roles.Update(roleUpdate.Value);
        await _unitOfWork.SaveChangesAsync();

        return UnitResult.Success<List<Error>>();
    }
}