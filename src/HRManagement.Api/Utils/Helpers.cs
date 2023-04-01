using HRManagement.Api.Models;
using HRManagement.Common.Application.Models;
using Microsoft.AspNetCore.Routing;

namespace HRManagement.Api.Utils;

public static class Helpers
{
    public static object BuildPaginationMetadata<TResponseDto>(PagedList<TResponseDto> value, PaginationParameters pagination, string actionMethod, LinkGenerator linker)
    {
        var previousPageLink = value.HasPrevious
            ? CreatePageResourceUri(actionMethod, pagination, ResourceUriType.PreviousPage, linker)
            : null;

        var nextPageLink = value.HasNext
            ? CreatePageResourceUri(actionMethod, pagination, ResourceUriType.NextPage, linker)
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