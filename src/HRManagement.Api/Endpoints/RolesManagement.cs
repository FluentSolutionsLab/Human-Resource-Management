using Carter;
using CSharpFunctionalExtensions;
using HRManagement.Api.Models;
using HRManagement.Modules.Personnel.Application.DTOs;
using HRManagement.Modules.Personnel.Application.Features.Role;
using MediatR;

namespace HRManagement.Api.Endpoints;

public class RolesManagement : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        const string roles = "/roles";
        const string routeContext = "Roles";

        app.MapGet(roles, async (IMediator mediator) =>
        {
            var (_, _, value, _) = await mediator.Send(new GetRolesQuery());
            return Results.Ok(ApiResponse<List<RoleDto>>.Ok(value));
        }).WithDisplayName(routeContext);

        app.MapGet($"{roles}/{{id}}", async (IMediator mediator, byte id) =>
        {
            var (isSuccess, _, value, error) = await mediator.Send(new GetRoleByIdQuery {Id = id});
            return isSuccess
                ? Results.Ok(ApiResponse<RoleDto>.Ok(value))
                : Results.BadRequest(ApiResponse<RoleDto>.Error(error));
        }).WithDisplayName(routeContext);
        
        app.MapPost(roles, async (IMediator mediator, CreateOrUpdateRoleDto newRole) =>
        {
            var (isSuccess, _, role, error) = await mediator.Send(CreateRoleCommand.MapFromDto(newRole));
            return isSuccess
                ? Results.Created($"{roles}/{{id}}", ApiResponse<RoleDto>.Ok(role))
                : Results.BadRequest(ApiResponse<RoleDto>.Error(error));
        }).WithDisplayName(routeContext);
        
        app.MapPut($"{roles}/{{id}}", async (IMediator mediator, byte id, CreateOrUpdateRoleDto updatedEmployee) =>
        {
            var (isSuccess, _, _, error) = await mediator.Send(UpdateRoleCommand.MapFromDto(id, updatedEmployee));
            return isSuccess ? Results.NoContent() : Results.BadRequest(ApiResponse<Unit>.Error(error));
        }).WithDisplayName(routeContext);
    }
}