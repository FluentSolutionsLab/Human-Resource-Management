using AutoMapper;
using CSharpFunctionalExtensions;
using HRManagement.Modules.Personnel.Application.Contracts;
using HRManagement.Modules.Personnel.Application.Contracts.Handlers;
using HRManagement.Modules.Personnel.Application.DTOs;
using HRManagement.Modules.Personnel.Domain;

namespace HRManagement.Modules.Personnel.Application.Features.Employee;

public class GetEmployeesQueryHandler : IQueryHandler<GetEmployeesQuery, Result<List<EmployeeDto>, Error>>
{
    private readonly IMapper _mapper;
    private readonly IEmployeeRepository _repository;

    public GetEmployeesQueryHandler(IMapper mapper, IEmployeeRepository repository)
    {
        _mapper = mapper;
        _repository = repository;
    }
    
    public async Task<Result<List<EmployeeDto>, Error>> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
    {
        var employees = await _repository.GetAsync();

        return _mapper.Map<List<EmployeeDto>>(employees);
    }
}