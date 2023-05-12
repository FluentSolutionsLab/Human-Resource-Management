﻿using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using HRManagement.Api.Utils;
using HRManagement.Common.Application.Models;
using HRManagement.Common.Domain.Models;
using HRManagement.Modules.Personnel.Application.UseCases;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;

namespace HRManagement.Api.Controllers.PersonnelManagement;

[ApiController]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly LinkGenerator _linker;

    public EmployeesController(IMediator mediator, LinkGenerator linker)
    {
        _mediator = mediator;
        _linker = linker;
    }

    [HttpGet(Name = "FindAll")]
    [ProducesResponseType(typeof(IEnumerable<EmployeeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Find([FromQuery] FilterParameters parameters)
    {
        var request = new GetEmployeesQuery {FilterParameters = parameters};
        var result = await _mediator.Send(request);
        
        var paginationMetadata = Helpers.BuildPaginationMetadata(result.Value, parameters, "FindAll", _linker);
        Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));
        
        return Ok(result.Value);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Find(string id)
    {
        var query = new GetEmployeeQuery {EmployeeId = id};
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpPost]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(List<Error>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Hire([FromBody] HireEmployeeDto newEmployee)
    {
        var command = newEmployee.ToHireEmployeeCommand();
        var (isSuccess, _, employee, errors) = await _mediator.Send(command);
        return isSuccess
            ? CreatedAtAction(nameof(Find), new {id = employee.Id}, Ok(employee))
            : BadRequest(errors);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(List<Error>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateEmployeeDto updatedEmployee)
    {
        var command = updatedEmployee.ToUpdateEmployeeCommand(id);
        var result = await _mediator.Send(command);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPut("{id}/terminate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Terminate(string id)
    {
        var command = new TerminateEmployeeCommand {EmployeeId = id};
        var result = await _mediator.Send(command);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(string id)
    {
        var command = new HardDeleteEmployeeCommand {EmployeeId = id};
        var result = await _mediator.Send(command);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}