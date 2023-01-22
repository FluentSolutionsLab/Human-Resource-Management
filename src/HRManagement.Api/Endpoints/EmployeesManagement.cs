using Carter;
using CSharpFunctionalExtensions;
using HRManagement.Api.Models;
using HRManagement.Modules.Personnel.Application.DTOs;
using HRManagement.Modules.Personnel.Application.Features.Employee;
using MediatR;

namespace HRManagement.Api.Endpoints;

public class EmployeesManagement : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        const string employees = "/employees";
        const string routeContext = "Employees";

        app.MapGet(employees, async (IMediator mediator) =>
        {
            var (_, _, value, _) = await mediator.Send(new GetEmployeesQuery());
            return Results.Ok(ApiResponse<List<EmployeeDto>>.Ok(value));
        }).WithDisplayName(routeContext);

        app.MapGet($"{employees}/{{id}}", async (IMediator mediator, string id) =>
        {
            var (isSuccess, _, value, error) = await mediator.Send(new GetEmployeeQuery {EmployeeId = id});
            return isSuccess
                ? Results.Ok(ApiResponse<EmployeeDto>.Ok(value))
                : Results.BadRequest(ApiResponse<EmployeeDto>.Error(error));
        }).WithDisplayName(routeContext);

        app.MapPost(employees, async (IMediator mediator, HireEmployeeDto newEmployee) =>
        {
            var (isSuccess, _, employee, error) = await mediator.Send(HireEmployeeCommand.MapFromDto(newEmployee));
            return isSuccess
                ? Results.Created($"{employees}/{{id}}", ApiResponse<EmployeeDto>.Ok(employee))
                : Results.BadRequest(ApiResponse<EmployeeDto>.Error(error));
        }).WithDisplayName(routeContext);

        app.MapPut($"{employees}/{{id}}", async (IMediator mediator, string id, UpdateEmployeeDto updatedEmployee) =>
        {
            var (isSuccess, _, _, error) = await mediator.Send(UpdateEmployeeCommand.MapFromDto(id, updatedEmployee));
            return isSuccess ? Results.NoContent() : Results.BadRequest(ApiResponse<Unit>.Error(error));
        }).WithDisplayName(routeContext);

        app.MapPut($"{employees}/{{id}}/terminate", async (IMediator mediator, string id) =>
        {
            var (isSuccess, _, _, error) = await mediator.Send(new TerminateEmployeeCommand {EmployeeId = id});
            return isSuccess ? Results.NoContent() : Results.BadRequest(ApiResponse<Unit>.Error(error));
        }).WithDisplayName(routeContext);

        app.MapDelete($"{employees}/{{id}}", async (IMediator mediator, string id) =>
        {
            var (isSuccess, _, _, error) = await mediator.Send(new HardDeleteEmployeeCommand {EmployeeId = id});
            return isSuccess ? Results.NoContent() : Results.BadRequest(ApiResponse<Unit>.Error(error));
        }).WithDisplayName(routeContext);
    }
}