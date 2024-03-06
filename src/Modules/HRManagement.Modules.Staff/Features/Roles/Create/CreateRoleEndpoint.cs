using CSharpFunctionalExtensions;
using HRManagement.BuildingBlocks.Endpoints;
using HRManagement.BuildingBlocks.Models;
using HRManagement.Modules.Staff.Features.Roles.Get;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace HRManagement.Modules.Staff.Features.Roles.Create;

[Route(BaseApiPath + "/roles")]
public class CreateRoleEndpoint : CommonController
{
    [HttpPost]
    [ProducesResponseType(typeof(RoleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(List<Error>), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(Tags = new[] {"Roles"})]
    public async Task<IActionResult> Hire([FromBody] CreateRoleDto newRole)
    {
        var (isSuccess, _, role, errors) = await Mediator.Send(newRole.ToCreateRoleCommand());
        return isSuccess
            ? Created($"{BaseApiPath}/roles/{role.Id}", role)
            : BadRequest(errors);
    }
}