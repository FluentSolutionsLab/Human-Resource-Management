using System.Collections.Generic;
using HRManagement.Modules.Personnel.Application.Contracts;
using HRManagement.Modules.Personnel.Domain;

namespace HRManagement.Modules.Personnel.Application.UseCases;

public class GetRolesQueryHandler : IQueryHandler<GetRolesQuery, Result<List<RoleDto>>>
{
    private readonly IGenericUnitOfWork _unitOfWork;

    public GetRolesQueryHandler(IGenericUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<RoleDto>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await _unitOfWork.GetRepository<Role, byte>().GetAsync();

        return roles.Select(x => x.ToResponseDto()).ToList();
    }
}