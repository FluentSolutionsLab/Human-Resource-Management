using HRManagement.Modules.Personnel.Application.Contracts;
using HRManagement.Modules.Personnel.Domain;

namespace HRManagement.Modules.Personnel.Application.UseCases;

public class GetRoleByIdQueryHandler : IQueryHandler<GetRoleByIdQuery, Result<RoleDto, Error>>
{
    private readonly IGenericUnitOfWork _unitOfWork;

    public GetRoleByIdQueryHandler(IGenericUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<RoleDto, Error>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        var role = await _unitOfWork.GetRepository<Role, byte>().GetByIdAsync(request.Id);
        if (role == null) return DomainErrors.NotFound(nameof(Role), request.Id);

        return role.ToResponseDto();
    }
}