using CSharpFunctionalExtensions;
using HRManagement.BuildingBlocks.Contracts;
using HRManagement.Modules.Staff.Models;

namespace HRManagement.Modules.Staff.Features.Roles.Get;

public class GetRolesQueryHandler : IQueryHandler<GetRolesQuery, Result<List<RoleDto>>>
{
    private readonly ICacheService _cacheService;
    private readonly IUnitOfWork _unitOfWork;

    public GetRolesQueryHandler(IUnitOfWork unitOfWork, ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<Result<List<RoleDto>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        const string queryCacheKey = "GetRolesQuery";
        var rolesOrNothing = _cacheService.Get<Maybe<List<Role>>>(queryCacheKey);
        if (rolesOrNothing.HasNoValue)
        {
            rolesOrNothing = await _unitOfWork.GetRepository<Role, int>().GetAsync(pageSize: request.PageSize);
            _cacheService.Set(queryCacheKey, rolesOrNothing);
        }

        return rolesOrNothing.Value.Select(x => x.ToResponseDto()).ToList();
    }
}