using HRManagement.BuildingBlocks.Endpoints;
using HRManagement.BuildingBlocks.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace HRManagement.Modules.Staff.Features.Roles.Update;

[Route(BaseApiPath + "/roles")]
public class UpdateRoleEndpoint : CommonController
{
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(List<Error>), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(Tags = new[] {"Roles"})]
    public async Task<IActionResult> Update(byte id, [FromBody] UpdateRoleDto updatedRoleDto)
    {
        var command = updatedRoleDto.ToUpdateRoleCommand(id);
        var result = await Mediator.Send(command);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}