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
        return await ValidateRequest(request)
            .Ensure(async validRequest => await CheckIfNameIsUnique(validRequest.Name),
                DomainErrors.ResourceAlreadyExists())
            .Ensure(async validRequest => await CheckIfManagerRoleExists(validRequest),
                DomainErrors.NotFound(nameof(Role), request.ReportsToId))
            .Map(validRequest => GetManagerRole(validRequest))
            .Ensure(async validRequest => await CheckIfRoleIsHierarchyTop(validRequest.ManagerRoleId),
                DomainErrors.ResourceAlreadyExists())
            .Map(validRequest => Role.Create(validRequest.Name, validRequest.ManagerRoleOrNothing))
            .Tap(async result =>
            {
                var role = result.Value;
                await _unitOfWork.GetRepository<Role, byte>().AddAsync(role);
                await _unitOfWork.SaveChangesAsync();
            }).Map(result => result.Value.ToResponseDto());
    }

    private static Result<ValidRequest, Error> ValidateRequest(CreateRoleCommand request)
    {
        var result = RoleName.Create(request.Name);
        return result.IsSuccess
            ? Result.Success<ValidRequest, Error>(new ValidRequest
                {Name = result.Value, ManagerRoleId = request.ReportsToId})
            : Result.Failure<ValidRequest, Error>(result.Error);
    }

    private async Task<bool> CheckIfNameIsUnique(RoleName name)
    {
        var nameIsUniqueCheck = await _unitOfWork.GetRepository<Role, byte>()
            .HasMatches(role => role.Name.Value == name.Value);

        return nameIsUniqueCheck.IsFailure;
    }

    private async Task<bool> CheckIfManagerRoleExists(ValidRequest request)
    {
        if (!request.ManagerRoleId.HasValue) return true;

        var queryCacheKey = $"GetRoleQuery/{request.ManagerRoleId}";
        var managerRoleOrNothing = _cacheService.Get<Maybe<Role>>(queryCacheKey);
        if (managerRoleOrNothing.HasNoValue)
        {
            managerRoleOrNothing =
                await _unitOfWork.GetRepository<Role, byte>().GetByIdAsync(request.ManagerRoleId.Value);
            if (managerRoleOrNothing.HasValue)
                _cacheService.Set(queryCacheKey, managerRoleOrNothing);
        }

        return managerRoleOrNothing.HasValue;
    }

    private ValidRequest GetManagerRole(ValidRequest request)
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

    private class ValidRequest
    {
        public RoleName Name { get; init; }
        public byte? ManagerRoleId { get; init; }
        public Maybe<Role> ManagerRoleOrNothing { get; set; } = Maybe<Role>.None;
    }
}