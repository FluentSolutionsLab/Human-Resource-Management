using System.Collections.Generic;
using Carter;
using CSharpFunctionalExtensions;
using HRManagement.Api.Models;
using HRManagement.Api.Utils;
using HRManagement.Common.Domain.Models;
using HRManagement.Modules.Personnel.Application.UseCases;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;

namespace HRManagement.Api.Endpoints;

public class EmployeesManagement : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        const string employees = "/api/personnel-management/employees";
        const string routeContext = "Employees";

        const string actionMethod = "GetEmployees";
        app.MapGet(employees, async (IMediator mediator, HttpContext httpContext, LinkGenerator linker, [AsParameters] PaginationParameters pagination) =>
            {
                var query = new GetEmployeesQuery {PageNumber = pagination.PageNumber.Value, PageSize = pagination.PageSize.Value};
                var result = await mediator.Send(query);

                var paginationMetadata = Helpers.BuildPaginationMetadata(result.Value, pagination, actionMethod, linker);
                httpContext.Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));

                return Results.Ok(result.Value);
            })
            .Produces<List<EmployeeDto>>()
            .WithName(actionMethod)
            .WithDisplayName(routeContext)
            .WithMetadata();

        app.MapGet($"{employees}/{{id}}", async (IMediator mediator, string id) =>
            {
                var query = new GetEmployeeQuery {EmployeeId = id};
                var (isSuccess, _, value, error) = await mediator.Send(query);
                return isSuccess ? Results.Ok(value) : Results.NotFound(error);
            })
            .Produces<EmployeeDto>()
            .Produces<Error>(StatusCodes.Status404NotFound)
            .WithDisplayName(routeContext);

        app.MapPost(employees, async (IMediator mediator, HireEmployeeDto newEmployee) =>
            {
                var command = newEmployee.ToHireEmployeeCommand();
                var (isSuccess, _, employee, errors) = await mediator.Send(command);
                return isSuccess
                    ? Results.Created($"{employees}/{{id}}", employee)
                    : Results.BadRequest(errors);
            })
            .Produces<EmployeeDto>(StatusCodes.Status201Created)
            .Produces<List<Error>>(StatusCodes.Status400BadRequest)
            .WithDisplayName(routeContext);

        app.MapPut($"{employees}/{{id}}", async (IMediator mediator, string id, UpdateEmployeeDto updatedEmployee) =>
            {
                var command = updatedEmployee.ToUpdateEmployeeCommand(id);
                var result = await mediator.Send(command);
                return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result.Error);
            })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<List<Error>>(StatusCodes.Status400BadRequest)
            .WithDisplayName(routeContext);

        app.MapPut($"{employees}/{{id}}/terminate", async (IMediator mediator, string id) =>
            {
                var command = new TerminateEmployeeCommand {EmployeeId = id};
                var result = await mediator.Send(command);
                return result.IsSuccess ? Results.NoContent() : Results.NotFound(result.Error);
            })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<Error>(StatusCodes.Status404NotFound)
            .WithDisplayName(routeContext);

        app.MapDelete($"{employees}/{{id}}/delete", async (IMediator mediator, string id) =>
            {
                var command = new HardDeleteEmployeeCommand {EmployeeId = id};
                var result = await mediator.Send(command);
                return result.IsSuccess ? Results.NoContent() : Results.NotFound(result.Error);
            })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<Error>(StatusCodes.Status404NotFound)
            .WithDisplayName(routeContext);
    }
}