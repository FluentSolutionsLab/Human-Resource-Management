﻿using HRManagement.BuildingBlocks.Models;
using Microsoft.AspNetCore.Routing;

namespace HRManagement.BuildingBlocks.Utilities;

public static class Utilities
{
    public static string ToISO8601String(this DateOnly date)
    {
        return date.ToString("O");
    }
    
    public static object BuildPaginationMetadata<TResponseDto>(PagedList<TResponseDto> value, FilterParameters filter,
        string actionMethod, LinkGenerator linker)
    {
        var previousPageLink = value.HasPrevious
            ? CreatePageResourceUri(actionMethod, filter.PageNumber, filter.PageSize, ResourceUriType.PreviousPage,
                linker)
            : null;

        var nextPageLink = value.HasNext
            ? CreatePageResourceUri(actionMethod, filter.PageNumber, filter.PageSize, ResourceUriType.NextPage, linker)
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

    private static string CreatePageResourceUri(string action, int pageNumber, int pageSize, ResourceUriType type,
        LinkGenerator linker)
    {
        return type switch
        {
            ResourceUriType.PreviousPage => linker.GetPathByName(action, new {pageNumber = pageNumber - 1, pageSize}),
            ResourceUriType.NextPage => linker.GetPathByName(action, new {pageNumber = pageNumber + 1, pageSize}),
            _ => linker.GetPathByName(action, new {pageNumber, pageSize})
        };
    }
}