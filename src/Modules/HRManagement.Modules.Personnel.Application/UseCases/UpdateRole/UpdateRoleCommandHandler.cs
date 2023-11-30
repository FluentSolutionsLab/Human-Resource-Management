using HRManagement.Modules.Personnel.Domain;

namespace HRManagement.Modules.Personnel.Application.UseCases;

public class UpdateRoleCommandHandler : ICommandHandler<UpdateRoleCommand, UnitResult<Error>>
{
    private readonly ICacheService _cacheService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateRoleCommandHandler(IUnitOfWork unitOfWork, ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<UnitResult<Error>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        return await ValidateRoleName(request)
            .Ensure(validRequest => CheckIfRoleExists(validRequest.Id), DomainErrors.NotFound(nameof(Role), request.Id))
            .Map(validRequest => GetRoleFromCache(validRequest))
            .Ensure(validRequest => CheckIfNameIsUnique(validRequest.Name, validRequest.Id),
                DomainErrors.ResourceAlreadyExists())
            .Ensure(validRequest => CheckIfRoleExists(validRequest.ManagerRoleId),
                DomainErrors.NotFound(nameof(Role), request.ReportsToId))
            .Map(validRequest => GetManagerRoleFromCache(validRequest))
            .Map(validRequest => UpdateRole(validRequest))
            .Tap(async roleToUpdate =>
            {
                _unitOfWork.GetRepository<Role, byte>().Update(roleToUpdate);
                await _unitOfWork.SaveChangesAsync();
            })
            .Tap(() =>
            {
                foreach (var key in _cacheService.GetAllKeys()
                             .Where(k => k.Contains("GetRoleQuery") || k.Contains("GetRolesQuery")))
                    _cacheService.Remove(key);
            })
            .Map(_ => UnitResult.Success<Error>());
    }


    private static Result<UpdateDto, Error> ValidateRoleName(UpdateRoleCommand request)
    {
        var result = RoleName.Create(request.Name);
        return result.IsSuccess
            ? Result.Success<UpdateDto, Error>(new UpdateDto
            {
                Id = request.Id, Name = result.Value, ManagerRoleId = request.ReportsToId
            })
            : Result.Failure<UpdateDto, Error>(result.Error);
    }

    private async Task<bool> CheckIfNameIsUnique(RoleName name, byte roleId)
    {
        var nameIsNotUniqueCheck = await _unitOfWork.GetRepository<Role, byte>()
            .HasMatches(role => role.Name.Value == name.Value && role.Id != roleId);

        return nameIsNotUniqueCheck.IsFailure;
    }

    private async Task<bool> CheckIfRoleExists(byte? roleId)
    {
        if (!roleId.HasValue) return true;

        var queryCacheKey = $"GetRoleQuery/{roleId}";
        var roleOrNothing = _cacheService.Get<Maybe<Role>>(queryCacheKey);
        if (roleOrNothing.HasNoValue)
        {
            roleOrNothing =
                await _unitOfWork.GetRepository<Role, byte>().GetByIdAsync(roleId.Value);
            if (roleOrNothing.HasValue)
                _cacheService.Set(queryCacheKey, roleOrNothing);
        }

        return roleOrNothing.HasValue;
    }

    private UpdateDto GetRoleFromCache(UpdateDto request)
    {
        var queryCacheKey = $"GetRoleQuery/{request.Id}";
        request.RoleToUpdate = _cacheService.Get<Maybe<Role>>(queryCacheKey).Value;

        return request;
    }

    private UpdateDto GetManagerRoleFromCache(UpdateDto request)
    {
        var queryCacheKey = $"GetRoleQuery/{request.ManagerRoleId}";
        request.ManagerRoleOrNothing = _cacheService.Get<Maybe<Role>>(queryCacheKey).Value;

        return request;
    }

    private static Role UpdateRole(UpdateDto validRequest)
    {
        var roleToUpdate = validRequest.RoleToUpdate;
        roleToUpdate.Update(validRequest.Name, validRequest.ManagerRoleOrNothing);
        return roleToUpdate;
    }

    private class UpdateDto
    {
        public byte Id { get; init; }

        public RoleName Name { get; init; }

        public Role RoleToUpdate { get; set; }

        public byte? ManagerRoleId { get; init; }
        public Maybe<Role> ManagerRoleOrNothing { get; set; } = Maybe<Role>.None;
    }
}