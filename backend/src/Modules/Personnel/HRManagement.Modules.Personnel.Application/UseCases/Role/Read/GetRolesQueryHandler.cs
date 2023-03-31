using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using HRManagement.Modules.Personnel.Application.Contracts;
using HRManagement.Modules.Personnel.Application.Contracts.Handlers;

namespace HRManagement.Modules.Personnel.Application.UseCases;

public class GetRolesQueryHandler : IQueryHandler<GetRolesQuery, Result<List<RoleDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetRolesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<RoleDto>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await _unitOfWork.Roles.GetAsync();

        return roles.Select(x => x.ToResponseDto()).ToList();
    }
}