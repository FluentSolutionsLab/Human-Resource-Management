using CSharpFunctionalExtensions;
using HRManagement.Common.Application.Contracts;
using HRManagement.Common.Domain.Models;
using HRManagement.Modules.Staff.Models;
using HRManagement.Modules.Staff.Models.ValueObjects;

namespace HRManagement.Modules.Staff.Features.TerminateEmployee;

public class TerminateEmployeeCommandHandler : ICommandHandler<TerminateEmployeeCommand, UnitResult<Error>>
{
    private readonly IUnitOfWork _unitOfWork;

    public TerminateEmployeeCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UnitResult<Error>> Handle(TerminateEmployeeCommand request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.EmployeeId, out var employeeId))
            return DomainErrors.NotFound(nameof(Employee), request.EmployeeId);

        var terminationDateCreation = ValueDate.Create(request.TerminationDate);
        if (terminationDateCreation.IsFailure) UnitResult.Failure(terminationDateCreation.Error);

        var employeeOrNot = await _unitOfWork.GetRepository<Employee, Guid>().GetByIdAsync(employeeId);
        if (employeeOrNot.HasNoValue) return DomainErrors.NotFound(nameof(Employee), employeeId);

        var employee = employeeOrNot.Value;
        employee.Terminate(terminationDateCreation.Value);
        _unitOfWork.GetRepository<Employee, Guid>().Update(employee);
        await _unitOfWork.SaveChangesAsync();

        return UnitResult.Success<Error>();
    }
}