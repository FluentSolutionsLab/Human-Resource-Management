using System.Collections.Generic;
using HRManagement.Modules.Personnel.Domain;

namespace HRManagement.Modules.Personnel.Application.UseCases;

public class GetRolesQueryHandler : IQueryHandler<GetRolesQuery, Result<List<RoleDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

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
            rolesOrNothing = await _unitOfWork.GetRepository<Role, byte>().GetAsync(pageSize: request.PageSize);
            _cacheService.Set(queryCacheKey, rolesOrNothing);
        }

        return rolesOrNothing.Value.Select(x => x.ToResponseDto()).ToList();
    }
}