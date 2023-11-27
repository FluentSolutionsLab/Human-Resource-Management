using HRManagement.Modules.Personnel.Domain;

namespace HRManagement.Modules.Personnel.Application.UseCases;

public class GetRoleByIdQueryHandler : IQueryHandler<GetRoleByIdQuery, Result<RoleDto, Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

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
            roleOrNothing = await _unitOfWork.GetRepository<Role, byte>().GetByIdAsync(request.Id, "ReportsTo");
            if (roleOrNothing.HasValue)
                _cacheService.Set(queryCacheKey, roleOrNothing);
            else
                return DomainErrors.NotFound(nameof(Role), request.Id);
        }

        return roleOrNothing.Value.ToResponseDto();
    }
}