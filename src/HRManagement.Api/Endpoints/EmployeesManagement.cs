using AutoMapper;
using CSharpFunctionalExtensions;
using HRManagement.Api.Models;
using HRManagement.Modules.Personnel.Application.DTOs;
using HRManagement.Modules.Personnel.Application.Features.Employee;
using MediatR;

namespace HRManagement.Api.Endpoints;

public static class EmployeesManagement
{
    public static void AddEmployeesManagementEndpoints(this IEndpointRouteBuilder app)
    {
        const string employees = "/employees";

        app.MapGet(employees, async (IMediator mediator) =>
        {
            var request = new GetEmployeesQuery();
            var (isSuccess, _, value, error) = await mediator.Send(request);
            return isSuccess
                ? Results.Ok(ApiResponse<List<EmployeeDto>>.Ok(value))
                : Results.BadRequest(ApiResponse<List<EmployeeDto>>.Error(error));
        });

        app.MapGet($"{employees}/{{id}}", async (IMediator mediator, string id) =>
        {
            var request = new GetEmployeeQuery {EmployeeId = id};
            var (isSuccess, _, value, error) = await mediator.Send(request);
            return isSuccess
                ? Results.Ok(ApiResponse<EmployeeDto>.Ok(value))
                : Results.BadRequest(ApiResponse<EmployeeDto>.Error(error));
        });

        app.MapPost(employees, async (IMediator mediator, IMapper mapper, HireEmployeeDto newEmployee) =>
        {
            var (isSuccess, _, employee, error) = await mediator.Send(mapper.Map<HireEmployeeCommand>(newEmployee));
            return isSuccess
                ? Results.Created($"{employees}/{{id}}", ApiResponse<EmployeeDto>.Ok(employee))
                : Results.BadRequest(ApiResponse<EmployeeDto>.Error(error));
        });

        app.MapPut($"{employees}/{{id}}", async (IMediator mediator, IMapper mapper, string id, UpdateEmployeeDto updatedEmployee) =>
            {
                var command = mapper.Map<UpdateEmployeeCommand>(updatedEmployee);
                command.EmployeeId = id;
                var (isSuccess, _, _, error) = await mediator.Send(command);
                return isSuccess ? Results.NoContent() : Results.BadRequest(ApiResponse<Unit>.Error(error));
            });

        app.MapPut($"{employees}/{{id}}/terminate", async (IMediator mediator, string id) =>
        {
            var (isSuccess, _, _, error) = await mediator.Send(new TerminateEmployeeCommand {EmployeeId = id});
            return isSuccess ? Results.NoContent() : Results.BadRequest(ApiResponse<Unit>.Error(error));
        });

        app.MapDelete($"{employees}/{{id}}", async (IMediator mediator, string id) =>
        {
            var command = new HardDeleteEmployeeCommand {EmployeeId = id};
            var (isSuccess, _, _, error) = await mediator.Send(command);
            return isSuccess ? Results.NoContent() : Results.BadRequest(ApiResponse<Unit>.Error(error));
        });
    }
}