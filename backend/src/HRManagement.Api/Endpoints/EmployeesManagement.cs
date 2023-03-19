using Carter;
using CSharpFunctionalExtensions;
using HRManagement.Api.Models;
using HRManagement.Common.Domain.Models;
using HRManagement.Modules.Personnel.Application.DTOs;
using HRManagement.Modules.Personnel.Application.Features.Employee;
using MediatR;
using Newtonsoft.Json;

namespace HRManagement.Api.Endpoints;

public class EmployeesManagement : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        const string employees = "/api/employees";
        const string routeContext = "Employees";

        app.MapGet(employees, async (IMediator mediator, HttpContext httpContext, LinkGenerator linker, [AsParameters] PaginationParameters pagination) =>
            {
                var (_, _, value, _) = await mediator.Send(new GetEmployeesQuery {PageNumber = pagination.PageNumber.Value, PageSize = pagination.PageSize.Value});

                var paginationMetadata = BuildPaginationMetadata(value, pagination, linker);
                httpContext.Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));

                return Results.Ok(value);
            })
            .Produces<List<EmployeeDto>>()
            .WithName("GetEmployees")
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
                var (isSuccess, _, _, errors) = await mediator.Send(UpdateEmployeeCommand.MapFromDto(id, updatedEmployee));
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

    private static object BuildPaginationMetadata(PagedList<EmployeeDto> value, PaginationParameters pagination, LinkGenerator linker)
    {
        var previousPageLink = value.HasPrevious
            ? CreatePageResourceUri("GetEmployees", pagination, ResourceUriType.PreviousPage, linker)
            : null;

        var nextPageLink = value.HasNext
            ? CreatePageResourceUri("GetEmployees", pagination, ResourceUriType.NextPage, linker)
            : null;

        var paginationMetadata = new
        {
            totalCount = value.TotalCount,
            pageSize = value.PageSize,
            currentPage = value.CurrentPage,
            totalPages = value.TotalPages,
            previousPageLink,
            nextPageLink
        };

        return paginationMetadata;
    }

    private static string CreatePageResourceUri(string action, PaginationParameters authorsResourceParameters, ResourceUriType type, LinkGenerator linker)
    {
        return type switch
        {
            ResourceUriType.PreviousPage => linker.GetPathByName(action,
                new
                {
                    pageNumber = authorsResourceParameters.PageNumber - 1,
                    pageSize = authorsResourceParameters.PageSize,
                }),
            ResourceUriType.NextPage => linker.GetPathByName(action,
                new
                {
                    pageNumber = authorsResourceParameters.PageNumber + 1,
                    pageSize = authorsResourceParameters.PageSize,
                }),
            _ => linker.GetPathByName(action,
                new {pageNumber = authorsResourceParameters.PageNumber, pageSize = authorsResourceParameters.PageSize,})
        };
    }
}