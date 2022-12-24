using CSharpFunctionalExtensions;
using HRManagement.Common.Domain;
using HRManagement.Modules.Personnel.Application.Contracts;
using HRManagement.Modules.Personnel.Application.Contracts.Handlers;
using HRManagement.Modules.Personnel.Domain;
using MediatR;

namespace HRManagement.Modules.Personnel.Application.Features.Employee;

public class HardDeleteEmployeeCommandHandler : ICommandHandler<HardDeleteEmployeeCommand, Result<Unit, Error>>
{
    private readonly IEmployeeRepository _repository;

    public HardDeleteEmployeeCommandHandler(IEmployeeRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<Result<Unit, Error>> Handle(HardDeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.EmployeeId, out var employeeId)) 
            return DomainErrors.NotFound(nameof(Domain.Employee.Employee), request.EmployeeId);

        Maybe<Domain.Employee.Employee> employeeOrNot = await _repository.GetByIdAsync(employeeId);
        if (employeeOrNot.HasNoValue) return DomainErrors.NotFound(nameof(Domain.Employee.Employee), employeeId);
        
        _repository.Delete(employeeOrNot.Value);
        await _repository.CommitAsync();

        return Unit.Value;
    }
}