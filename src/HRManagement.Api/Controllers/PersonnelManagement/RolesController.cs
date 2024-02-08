using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using HRManagement.Common.Domain.Models;
using HRManagement.Modules.Staff.Application.UseCases;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRManagement.Api.Controllers.PersonnelManagement;

[Route("api/[controller]")]
public class RolesController : CommonController
{
    private readonly IMediator _mediator;

    public RolesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RoleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Find([FromQuery] int pageSize = 50)
    {
        var query = new GetRolesQuery {PageSize = pageSize};
        var (_, _, value, _) = await _mediator.Send(query);
        return Ok(value);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(IEnumerable<RoleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Find(byte id)
    {
        var query = new GetRoleByIdQuery {Id = id};
        var (isSuccess, _, role, error) = await _mediator.Send(query);
        return isSuccess ? Ok(role) : NotFound(error);
    }

    [HttpPost]
    [ProducesResponseType(typeof(RoleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(List<Error>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Hire([FromBody] CreateRoleDto newRole)
    {
        var (isSuccess, _, role, errors) = await _mediator.Send(newRole.ToCreateRoleCommand());
        return isSuccess
            ? CreatedAtAction(nameof(Find), new {id = role.Id}, role)
            : BadRequest(errors);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(List<Error>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(byte id, [FromBody] UpdateRoleDto updatedRoleDto)
    {
        var command = updatedRoleDto.ToUpdateRoleCommand(id);
        var result = await _mediator.Send(command);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}