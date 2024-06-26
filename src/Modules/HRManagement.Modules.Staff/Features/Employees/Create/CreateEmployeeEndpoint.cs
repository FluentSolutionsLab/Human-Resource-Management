﻿using CSharpFunctionalExtensions;
using HRManagement.BuildingBlocks.Endpoints;
using HRManagement.BuildingBlocks.Models;
using HRManagement.Modules.Staff.Features.Employees.Get;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace HRManagement.Modules.Staff.Features.Employees.Create;

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