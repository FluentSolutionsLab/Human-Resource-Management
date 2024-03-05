using CSharpFunctionalExtensions;
using HRManagement.Common.Application.Contracts;
using HRManagement.Common.Domain.Models;
using HRManagement.Modules.Staff.Models;

namespace HRManagement.Modules.Staff.Features.GetRoles;

public class GetRoleByIdQueryHandler : IQueryHandler<GetRoleByIdQuery, Result<RoleDto, Error>>
{
    private readonly ICacheService _cacheService;
    private readonly IUnitOfWork _unitOfWork;

    public GetRoleByIdQueryHandler(IUnitOfWork unitOfWork, ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<Result<RoleDto, Error>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        var queryCacheKey = $"GetRoleQuery/{request.Id}";
        var roleOrNothing = _cacheService.Get<Maybe<Role>>(queryCacheKey);
        if (roleOrNothing.HasNoValue)
        {
            roleOrNothing = await _unitOfWork.GetRepository<Role, int>().GetByIdAsync(request.Id, "ReportsTo");
            if (roleOrNothing.HasValue)
                _cacheService.Set(queryCacheKey, roleOrNothing);
            else
                return DomainErrors.NotFound(nameof(Role), request.Id);
        }

        return roleOrNothing.Value.ToResponseDto();
    }
}