using System.Text.Json;
using HRManagement.Common.Application.Models;
using HRManagement.Common.Endpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Swashbuckle.AspNetCore.Annotations;

namespace HRManagement.Modules.Staff.Features.GetEmployees;

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

        var paginationMetadata = Helpers.BuildPaginationMetadata(result.Value, parameters, "GetEmployees", _linker);
        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

        return Ok(result.Value);
    }
}