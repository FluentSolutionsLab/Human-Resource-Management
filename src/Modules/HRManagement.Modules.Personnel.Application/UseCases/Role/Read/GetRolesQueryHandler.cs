﻿using System.Collections.Generic;
using HRManagement.Modules.Personnel.Application.Contracts;

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