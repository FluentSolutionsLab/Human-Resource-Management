using CSharpFunctionalExtensions;
using HRManagement.Common.Domain.Models;
using HRManagement.Common.Endpoints;
using HRManagement.Modules.Staff.Features.GetEmployees;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace HRManagement.Modules.Staff.Features.CreateEmployee;

[Route(BaseApiPath + "/employees")]
public class CreateEmployeeEndpoint : CommonController
{
    [HttpPost]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(List<Error>), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(Tags = new[] {"Employees"})]
    public async Task<IActionResult> Hire([FromBody] CreateEmployeeDto newEmployee)
    {
        var command = newEmployee.ToHireEmployeeCommand();
        var (isSuccess, _, employee, errors) = await Mediator.Send(command);
        return isSuccess
            ? Created($"{BaseApiPath}/employees/{employee.Id}", employee)
            : BadRequest(errors);
    }
}