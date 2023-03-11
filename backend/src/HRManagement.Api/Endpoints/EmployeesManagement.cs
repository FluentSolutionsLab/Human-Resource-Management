using Carter;
using CSharpFunctionalExtensions;
using HRManagement.Common.Domain.Models;
using HRManagement.Modules.Personnel.Application.DTOs;
using HRManagement.Modules.Personnel.Application.Features.Employee;
using MediatR;

namespace HRManagement.Api.Endpoints;

public class EmployeesManagement : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        const string employees = "/api/employees";
        const string routeContext = "Employees";

        app.MapGet(employees, async (IMediator mediator) =>
            {
                var (_, _, value, _) = await mediator.Send(new GetEmployeesQuery());
                return Results.Ok(value);
            })
            .Produces<List<EmployeeDto>>()
            .WithDisplayName(routeContext)
            .WithMetadata();

        app.MapGet($"{employees}/{{id}}", async (IMediator mediator, string id) =>
            {
                var (isSuccess, _, value, error) = await mediator.Send(new GetEmployeeQuery {EmployeeId = id});
                return isSuccess ? Results.Ok(value) : Results.NotFound(error);
            })
            .Produces<EmployeeDto>()
            .Produces<Error>(StatusCodes.Status404NotFound)
            .WithDisplayName(routeContext);

        app.MapPost(employees, async (IMediator mediator, HireEmployeeDto newEmployee) =>
            {
                var (isSuccess, _, employee, errors) = await mediator.Send(HireEmployeeCommand.MapFromDto(newEmployee));
                return isSuccess
                    ? Results.Created($"{employees}/{{id}}", employee)
                    : Results.BadRequest(errors);
            })
            .Produces<EmployeeDto>(StatusCodes.Status201Created)
            .Produces<List<Error>>(StatusCodes.Status400BadRequest)
            .WithDisplayName(routeContext);

        app.MapPut($"{employees}/{{id}}", async (IMediator mediator, string id, UpdateEmployeeDto updatedEmployee) =>
            {
                var (isSuccess, _, _, errors) =
                    await mediator.Send(UpdateEmployeeCommand.MapFromDto(id, updatedEmployee));
                return isSuccess ? Results.NoContent() : Results.BadRequest(errors);
            })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<List<Error>>(StatusCodes.Status400BadRequest)
            .WithDisplayName(routeContext);

        app.MapPut($"{employees}/{{id}}/terminate", async (IMediator mediator, string id) =>
            {
                var (isSuccess, _, _, error) = await mediator.Send(new TerminateEmployeeCommand {EmployeeId = id});
                return isSuccess ? Results.NoContent() : Results.NotFound(error);
            })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<Error>(StatusCodes.Status404NotFound)
            .WithDisplayName(routeContext);

        app.MapDelete($"{employees}/{{id}}", async (IMediator mediator, string id) =>
            {
                var (isSuccess, _, _, error) = await mediator.Send(new HardDeleteEmployeeCommand {EmployeeId = id});
                return isSuccess ? Results.NoContent() : Results.NotFound(error);
            })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<Error>(StatusCodes.Status404NotFound)
            .WithDisplayName(routeContext);
    }
}