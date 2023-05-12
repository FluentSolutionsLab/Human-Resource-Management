using System;
using HRManagement.Modules.Personnel.Application.Contracts;
using HRManagement.Modules.Personnel.Domain;

namespace HRManagement.Modules.Personnel.Application.UseCases;

public class HardDeleteEmployeeCommandHandler : ICommandHandler<HardDeleteEmployeeCommand, UnitResult<Error>>
{
    private readonly IGenericUnitOfWork _unitOfWork;

    public HardDeleteEmployeeCommandHandler(IGenericUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UnitResult<Error>> Handle(HardDeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.EmployeeId, out var employeeId))
            return DomainErrors.NotFound(nameof(Employee), request.EmployeeId);

        Maybe<Employee> employeeOrNot = await _unitOfWork.GetRepository<Employee, Guid>().GetByIdAsync(employeeId);
        if (employeeOrNot.HasNoValue) return DomainErrors.NotFound(nameof(Employee), employeeId);

        _unitOfWork.GetRepository<Employee, Guid>().Delete(employeeOrNot.Value);
        await _unitOfWork.SaveChangesAsync();

        return UnitResult.Success<Error>();
    }
}