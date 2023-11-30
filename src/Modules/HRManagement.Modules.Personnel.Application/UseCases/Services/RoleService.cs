﻿using HRManagement.Modules.Personnel.Domain;

namespace HRManagement.Modules.Personnel.Application.UseCases.Services;

public class RoleService : IRoleService
{
    private readonly ICacheService _cacheService;
    private readonly IUnitOfWork _unitOfWork;

    public RoleService(ICacheService cacheService, IUnitOfWork unitOfWork)
    {
        _cacheService = cacheService;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> CheckIfRoleExists(byte? roleId)
    {
        if (!roleId.HasValue) return true;

        var queryCacheKey = $"GetRoleQuery/{roleId}";
        var roleOrNothing = _cacheService.Get<Maybe<Role>>(queryCacheKey);
        if (roleOrNothing.HasNoValue)
        {
            roleOrNothing =
                await _unitOfWork.GetRepository<Role, byte>().GetByIdAsync(roleId.Value, "ReportsTo");
            if (roleOrNothing.HasValue)
                _cacheService.Set(queryCacheKey, roleOrNothing);
        }

        return roleOrNothing.HasValue;
    }

    public EmployeeCreateOrUpdateDto GetRole(EmployeeCreateOrUpdateDto request)
    {
        var queryCacheKey = $"GetRoleQuery/{request.RoleId}";
        request.RoleOrNothing = _cacheService.Get<Maybe<Role>>(queryCacheKey).Value;

        return request;
    }
}