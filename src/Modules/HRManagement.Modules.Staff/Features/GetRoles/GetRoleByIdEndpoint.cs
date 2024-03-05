using CSharpFunctionalExtensions;
using HRManagement.Common.Domain.Models;
using HRManagement.Common.Endpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace HRManagement.Modules.Staff.Features.FindRoles;

[Route(BaseApiPath + "/roles")]
public class GetRoleByIdEndpoint : CommonController
{
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(IEnumerable<RoleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [SwaggerOperation(Tags = new[] {"Roles"})]
    public async Task<IActionResult> Find(byte id)
    {
        var query = new GetRoleByIdQuery {Id = id};
        var (isSuccess, _, role, error) = await Mediator.Send(query);
        return isSuccess ? Ok(role) : NotFound(error);
    }
}