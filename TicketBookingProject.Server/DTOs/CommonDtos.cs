using Microsoft.AspNetCore.Mvc;

namespace TicketBookingProject.Server;

public record ApiResponse<T>(
    bool Success,
    string Message,
    T? Data,
    object? Errors = null)
{
    public static ApiResponse<T> Ok(T data, string message = "Success") =>
        new(true, message, data);

    public static ApiResponse<T> Fail(string message, object? errors = null) =>
        new(false, message, default, errors);
}

public record ApiResponse(bool Success, string Message, object? Errors = null)
{
    public static ApiResponse Ok(string message = "Success") =>
        new(true, message);

    public static ApiResponse Fail(string message, object? errors = null) =>
        new(false, message, errors);
}


public class NApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public object? Errors { get; set; }
}

public class Result<T>
{
    public bool IsSuccess { get; init; }
    public string?Message { get; init; }
    public T? Data { get; init; }
    public int StatusCode { get; init; }
    public object? Errors { get; init; }

    public static Result<T> Success(T data, string? message = null) => new()
    {
        IsSuccess = true,
        Data = data,
        Message = message,
        StatusCode = StatusCodes.Status200OK
    };

    public static Result<T> Success(T data, int statusCode, string? message = null) => new()
    {
        IsSuccess = true,
        Data = data,
        Message = message,
        StatusCode = statusCode
    };

    public static Result<T> Failure(string message, object? errors = null) => new()
    {
        IsSuccess = false,
        Message = message,
        StatusCode = StatusCodes.Status400BadRequest,
        Errors = errors
    };

    public static Result<T> Failure(string message, int statusCode, object? errors = null) => new()
    {
        IsSuccess = false,
        Message = message,
        StatusCode = statusCode,
        Errors = errors
    };
}

// ─────────────────────────────────────────────
// Pagination
// ─────────────────────────────────────────────

public record PagedRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? SortBy { get; init; }
    public bool SortDesc { get; init; } = true;
}

public record PagedResponse<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalCount)
{
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPrevPage => Page > 1;
}



