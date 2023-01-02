using AutoMapper;
using CSharpFunctionalExtensions;
using HRManagement.Api.Models;
using HRManagement.Modules.Personnel.Application.DTOs;
using HRManagement.Modules.Personnel.Application.Features.Employee;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HRManagement.Api.Controllers;

[Route("[controller]")]
public class EmployeesController : CommonController
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public EmployeesController(IMapper mapper, IMediator mediator)
    {
        _mapper = mapper;
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<EmployeeDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Find()
    {
        var request = new GetEmployeesQuery();
        var result = await _mediator.Send(request);
        return FormatResponse(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<EmployeeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Find(string id)
    {
        var request = new GetEmployeeQuery {EmployeeId = id};
        var result = await _mediator.Send(request);
        return FormatResponse(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<EmployeeDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Hire([FromBody] HireEmployeeDto newEmployee)
    {
        var (isSuccess, _, employee, error) = await _mediator.Send(_mapper.Map<HireEmployeeCommand>(newEmployee));
        return isSuccess
            ? CreatedAtAction(nameof(Find), new {id = employee.Id}, ApiResponse<EmployeeDto>.Ok(employee))
            : BadRequest(ApiResponse<EmployeeDto>.Error(error));
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateEmployeeDto updatedEmployee)
    {
        var command = _mapper.Map<UpdateEmployeeCommand>(updatedEmployee);
        command.EmployeeId = id;
        var (isSuccess, _, _, error) = await _mediator.Send(command);
        return isSuccess ? NoContent() : BadRequest(ApiResponse<Unit>.Error(error));
    }

    [HttpPut("{id}/terminate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Terminate(string id)
    {
        var command = new TerminateEmployeeCommand {EmployeeId = id};
        var (isSuccess, _, _, error) = await _mediator.Send(command);
        return isSuccess ? NoContent() : BadRequest(ApiResponse<Unit>.Error(error));
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(string id)
    {
        var command = new HardDeleteEmployeeCommand {EmployeeId = id};
        var (isSuccess, _, _, error) = await _mediator.Send(command);
        return isSuccess ? NoContent() : BadRequest(ApiResponse<Unit>.Error(error));
    }
}