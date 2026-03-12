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

// ─────────────────────────────────────────────
// Pagination
// ─────────────────────────────────────────────

public record PagedRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? SortBy { get; init; }
    public bool SortDesc { get; init; } = false;
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



