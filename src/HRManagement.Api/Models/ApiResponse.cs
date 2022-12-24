using HRManagement.Common.Domain;

namespace HRManagement.Api.Models;

public class ApiResponse<TData>
{
    public TData Data { get; }
    public List<Error> Errors { get; }

    public ApiResponse(TData data, List<Error> errors)
    {
        Data = data;
        Errors = errors;
    }

    public static ApiResponse<TData> Ok(TData data)
    {
        return new ApiResponse<TData>(data, default);
    }

    public static ApiResponse<TData> Error(Error error)
    {
        return new ApiResponse<TData>(default, new List<Error> {error});
    }

    public static ApiResponse<TData> Error(List<Error> errors)
    {
        return new ApiResponse<TData>(default, errors);
    }
}