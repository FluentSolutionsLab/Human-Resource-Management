using HRManagement.Common.Domain.Models;
using HRManagement.Common.Endpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace HRManagement.Modules.Staff.Features.UpdateEmployee;

[Route(BaseApiPath + "/employees")]
public class UpdateEmployeeEndpoint : CommonController
{
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(List<Error>), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(Tags = new[] {"Employees"})]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateEmployeeDto updatedEmployee)
    {
        var command = updatedEmployee.ToUpdateEmployeeCommand(id);
        var result = await Mediator.Send(command);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}