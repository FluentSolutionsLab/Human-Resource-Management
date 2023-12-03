using HRManagement.Modules.Personnel.Application.UseCases.Services;
using HRManagement.Modules.Personnel.Domain;

namespace HRManagement.Modules.Personnel.Application.UseCases;

public class CreateRoleCommandHandler : ICommandHandler<CreateRoleCommand, Result<RoleDto, Error>>
{
    private readonly ICacheService _cacheService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateRoleCommandHandler(IUnitOfWork unitOfWork, ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<Result<RoleDto, Error>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        return await ValidateRoleName(request)
            .Ensure(async validRequest => await CheckIfNameIsUnique(validRequest.Name),
                DomainErrors.ResourceAlreadyExists())
            .Ensure(async validRequest => await CheckIfManagerRoleExists(validRequest.ManagerRoleId),
                DomainErrors.NotFound(nameof(Role), request.ReportsToId))
            .Map(GetManagerRole)
            .Ensure(async validRequest => await CheckIfRoleIsHierarchyTop(validRequest.ManagerRoleId),
                DomainErrors.ResourceAlreadyExists())
            .Map(validRequest => Role.Create(validRequest.Name, validRequest.ManagerRoleOrNothing))
            .Tap(async result =>
            {
                var role = result.Value;
                await _unitOfWork.GetRepository<Role, byte>().AddAsync(role);
                await _unitOfWork.SaveChangesAsync();
            })
            .Tap(() => _cacheService.RemoveAll(k => k.Contains("GetRoleQuery") || k.Contains("GetRolesQuery")))
            .Map(result => result.Value.ToResponseDto());
    }

    private static Result<RoleCreateOrUpdateDto, Error> ValidateRoleName(CreateRoleCommand request)
    {
        var result = RoleName.Create(request.Name);
        return result.IsSuccess
            ? Result.Success<RoleCreateOrUpdateDto, Error>(new RoleCreateOrUpdateDto
                {Name = result.Value, ManagerRoleId = request.ReportsToId})
            : Result.Failure<RoleCreateOrUpdateDto, Error>(result.Error);
    }

    private async Task<bool> CheckIfNameIsUnique(RoleName name)
    {
        var nameIsNotUniqueCheck = await _unitOfWork.GetRepository<Role, byte>()
            .HasMatches(role => role.Name.Value == name.Value);

        return nameIsNotUniqueCheck.IsFailure;
    }

    private async Task<bool> CheckIfManagerRoleExists(byte? managerRoleId)
    {
        if (!managerRoleId.HasValue) return true;

        var queryCacheKey = $"GetRoleQuery/{managerRoleId}";
        var managerRoleOrNothing = _cacheService.Get<Maybe<Role>>(queryCacheKey);
        if (managerRoleOrNothing.HasNoValue)
        {
            managerRoleOrNothing =
                await _unitOfWork.GetRepository<Role, byte>().GetByIdAsync(managerRoleId.Value);
            if (managerRoleOrNothing.HasValue)
                _cacheService.Set(queryCacheKey, managerRoleOrNothing);
        }

        return managerRoleOrNothing.HasValue;
    }

    private RoleCreateOrUpdateDto GetManagerRole(RoleCreateOrUpdateDto request)
    {
        if (request.ManagerRoleId.HasValue)
        {
            var queryCacheKey = $"GetRoleQuery/{request.ManagerRoleId}";
            request.ManagerRoleOrNothing = _cacheService.Get<Maybe<Role>>(queryCacheKey);
        }

        return request;
    }

    private async Task<bool> CheckIfRoleIsHierarchyTop(byte? managerRoleId)
    {
        if (managerRoleId.HasValue) return true;
        var headAlreadyExistsCheck =
            await _unitOfWork.GetRepository<Role, byte>().HasMatches(role => role.ReportsTo == null);

        return headAlreadyExistsCheck.IsFailure;
    }
}