using Carter;
using CSharpFunctionalExtensions;
using HRManagement.Common.Domain.Models;
using HRManagement.Modules.Personnel.Application.DTOs;
using HRManagement.Modules.Personnel.Application.Features.Role;
using MediatR;

namespace HRManagement.Api.Endpoints;

public class RolesManagement : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        const string roles = "/api/roles";
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

        app.MapPost(roles, async (IMediator mediator, CreateOrUpdateRoleDto newRole) =>
            {
                var (isSuccess, _, role, errors) = await mediator.Send(CreateRoleCommand.MapFromDto(newRole));
                return isSuccess ? Results.Created($"{roles}/{{id}}", role) : Results.BadRequest(errors);
            })
            .Produces<RoleDto>(StatusCodes.Status201Created)
            .Produces<List<Error>>(StatusCodes.Status400BadRequest)
            .WithDisplayName(routeContext);

        app.MapPut($"{roles}/{{id}}", async (IMediator mediator, byte id, CreateOrUpdateRoleDto updatedEmployee) =>
            {
                var (isSuccess, _, _, errors) = await mediator.Send(UpdateRoleCommand.MapFromDto(id, updatedEmployee));
                return isSuccess ? Results.NoContent() : Results.BadRequest(errors);
            })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<Error>(StatusCodes.Status404NotFound)
            .WithDisplayName(routeContext);
    }
}