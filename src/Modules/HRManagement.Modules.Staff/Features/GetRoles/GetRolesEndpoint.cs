using CSharpFunctionalExtensions;
using HRManagement.Common.Endpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace HRManagement.Modules.Staff.Features.FindRoles;

[Route(BaseApiPath + "/roles")]
public class GetRolesEndpoint : CommonController
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RoleDto>), StatusCodes.Status200OK)]
    [SwaggerOperation(Tags = new[] {"Roles"})]
    public async Task<IActionResult> Find([FromQuery] int pageSize = 50)
    {
        var query = new GetRolesQuery {PageSize = pageSize};
        var (_, _, value, _) = await Mediator.Send(query);
        return Ok(value);
    }
}