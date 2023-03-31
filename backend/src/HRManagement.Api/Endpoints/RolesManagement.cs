using System.Collections.Generic;
using Carter;
using CSharpFunctionalExtensions;
using HRManagement.Common.Domain.Models;
using HRManagement.Modules.Personnel.Application.UseCases;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace HRManagement.Api.Endpoints;

public class RolesManagement : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        const string roles = "/api/personnel-management/roles";
        const string routeContext = "Roles";

        app.MapGet(roles, async (IMediator mediator) =>
            {
                var (_, _, value, _) = await mediator.Send(new GetRolesQuery());
                return Results.Ok(value);
            })
            .Produces<List<RoleDto>>()
            .WithDisplayName(routeContext)
            .WithMetadata();

        app.MapGet($"{roles}/{{id}}", async (IMediator mediator, byte id) =>
            {
                var (isSuccess, _, role, error) = await mediator.Send(new GetRoleByIdQuery {Id = id});
                return isSuccess ? Results.Ok(role) : Results.NotFound(error);
            })
            .Produces<RoleDto>()
            .Produces<Error>(StatusCodes.Status404NotFound)
            .WithDisplayName(routeContext);

        app.MapPost(roles, async (IMediator mediator, CreateRoleDto newRole) =>
            {
                var (isSuccess, _, role, errors) = await mediator.Send(newRole.ToCreateRoleCommand());
                return isSuccess ? Results.Created($"{roles}/{{id}}", role) : Results.BadRequest(errors);
            })
            .Produces<RoleDto>(StatusCodes.Status201Created)
            .Produces<List<Error>>(StatusCodes.Status400BadRequest)
            .WithDisplayName(routeContext);

        app.MapPut($"{roles}/{{id}}", async (IMediator mediator, byte id, UpdateRoleDto updatedEmployee) =>
            {
                var result = await mediator.Send(updatedEmployee.ToUpdateRoleCommand(id));
                return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result.Error);
            })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<Error>(StatusCodes.Status404NotFound)
            .WithDisplayName(routeContext);
    }
}