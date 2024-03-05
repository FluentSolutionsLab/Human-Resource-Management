﻿using HRManagement.Common.Domain.Models;
using HRManagement.Common.Endpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace HRManagement.Modules.Staff.Features.TerminateEmployee;

[Route(BaseApiPath + "/employees")]
public class HardDeleteEmployeeEndpoint : CommonController
{
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(Tags = new[] {"Employees"})]
    public async Task<IActionResult> Delete(string id)
    {
        var command = new HardDeleteEmployeeCommand {EmployeeId = id};
        var result = await Mediator.Send(command);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}