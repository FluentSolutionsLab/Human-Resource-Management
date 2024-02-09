using System;
using HRManagement.Modules.Staff.Domain;

namespace HRManagement.Modules.Staff.Application.UseCases;

public class HardDeleteEmployeeCommandHandler : ICommandHandler<HardDeleteEmployeeCommand, UnitResult<Error>>
{
    private readonly IUnitOfWork _unitOfWork;

    public HardDeleteEmployeeCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UnitResult<Error>> Handle(HardDeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.EmployeeId, out var employeeId))
            return DomainErrors.NotFound(nameof(Employee), request.EmployeeId);

        var employeeOrNot = await _unitOfWork.GetRepository<Employee, Guid>().GetByIdAsync(employeeId);
        if (employeeOrNot.HasNoValue) return DomainErrors.NotFound(nameof(Employee), employeeId);

        _unitOfWork.GetRepository<Employee, Guid>().Delete(employeeOrNot.Value);
        await _unitOfWork.SaveChangesAsync();

        return UnitResult.Success<Error>();
    }
}