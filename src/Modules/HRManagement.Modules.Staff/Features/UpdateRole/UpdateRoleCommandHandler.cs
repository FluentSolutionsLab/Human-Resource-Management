using CSharpFunctionalExtensions;
using HRManagement.Common.Application.Contracts;
using HRManagement.Common.Domain.Models;
using HRManagement.Modules.Staff.Features.Services;
using HRManagement.Modules.Staff.Models;
using HRManagement.Modules.Staff.Models.ValueObjects;

namespace HRManagement.Modules.Staff.Features.UpdateRole;

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
                _unitOfWork.GetRepository<Role, int>().Update(roleToUpdate);
                await _unitOfWork.SaveChangesAsync();
            })
            .Tap(() => _cacheService.RemoveAll(k => k.Contains("GetRoleQuery") || k.Contains("GetRolesQuery")))
            .Map(_ => UnitResult.Success<Error>());
    }


    private static Result<RoleCreateOrUpdateDto, Error> ValidateRoleName(UpdateRoleCommand request)
    {
        var result = RoleName.Create(request.Name);
        return result.IsSuccess
            ? Result.Success<RoleCreateOrUpdateDto, Error>(new RoleCreateOrUpdateDto
            {
                Id = request.Id, Name = result.Value, ManagerRoleId = request.ReportsToId
            })
            : Result.Failure<RoleCreateOrUpdateDto, Error>(result.Error);
    }

    private async Task<bool> CheckIfNameIsUnique(RoleName name, int roleId)
    {
        var nameIsNotUniqueCheck = await _unitOfWork.GetRepository<Role, int>()
            .HasMatches(role => role.Name.Value == name.Value && role.Id != roleId);

        return nameIsNotUniqueCheck.IsFailure;
    }

    private async Task<bool> CheckIfRoleExists(int? roleId)
    {
        if (!roleId.HasValue) return true;

        var queryCacheKey = $"GetRoleQuery/{roleId}";
        var roleOrNothing = _cacheService.Get<Maybe<Role>>(queryCacheKey);
        if (roleOrNothing.HasNoValue)
        {
            roleOrNothing =
                await _unitOfWork.GetRepository<Role, int>().GetByIdAsync(roleId.Value);
            if (roleOrNothing.HasValue)
                _cacheService.Set(queryCacheKey, roleOrNothing);
        }

        return roleOrNothing.HasValue;
    }

    private RoleCreateOrUpdateDto GetRoleFromCache(RoleCreateOrUpdateDto request)
    {
        var queryCacheKey = $"GetRoleQuery/{request.Id}";
        request.RoleToUpdate = _cacheService.Get<Maybe<Role>>(queryCacheKey).Value;

        return request;
    }

    private RoleCreateOrUpdateDto GetManagerRoleFromCache(RoleCreateOrUpdateDto request)
    {
        var queryCacheKey = $"GetRoleQuery/{request.ManagerRoleId}";
        request.ManagerRoleOrNothing = _cacheService.Get<Maybe<Role>>(queryCacheKey).Value;

        return request;
    }

    private static Role UpdateRole(RoleCreateOrUpdateDto validRequest)
    {
        var roleToUpdate = validRequest.RoleToUpdate;
        roleToUpdate.Update(validRequest.Name, validRequest.ManagerRoleOrNothing);
        return roleToUpdate;
    }
}