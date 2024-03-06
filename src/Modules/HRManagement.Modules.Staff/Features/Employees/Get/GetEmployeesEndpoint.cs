using System.Text.Json;
using HRManagement.BuildingBlocks.Endpoints;
using HRManagement.BuildingBlocks.Models;
using HRManagement.BuildingBlocks.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Swashbuckle.AspNetCore.Annotations;

namespace HRManagement.Modules.Staff.Features.Employees.Get;

[Route(BaseApiPath + "/employees")]
public class GetEmployeesEndpoint : CommonController
{
    private readonly LinkGenerator _linker;

    public GetEmployeesEndpoint(LinkGenerator linker)
    {
        _linker = linker;
    }

    [HttpGet(Name = "GetEmployees")]
    [ProducesResponseType(typeof(IEnumerable<EmployeeDto>), StatusCodes.Status200OK)]
    [SwaggerOperation(Tags = new[] {"Employees"})]
    public async Task<IActionResult> Find([FromQuery] FilterParameters parameters)
    {
        var request = new GetEmployeesQuery {FilterParameters = parameters};
        var result = await Mediator.Send(request);

        var paginationMetadata = Utilities.BuildPaginationMetadata(result.Value, parameters, "GetEmployees", _linker);
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

        return Ok(result.Value);
    }
}