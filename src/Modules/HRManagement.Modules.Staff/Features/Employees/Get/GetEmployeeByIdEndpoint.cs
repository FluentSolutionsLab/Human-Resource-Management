using HRManagement.BuildingBlocks.Endpoints;
using HRManagement.BuildingBlocks.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace HRManagement.Modules.Staff.Features.Employees.Get;

[Route(BaseApiPath + "/employees")]
public class GetEmployeeByIdEndpoint : CommonController
{
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [SwaggerOperation(Tags = new[] {"Employees"})]
    public async Task<IActionResult> Find(string id)
    {
        var query = new GetEmployeeQuery {EmployeeId = id};
        var result = await Mediator.Send(query);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }
}