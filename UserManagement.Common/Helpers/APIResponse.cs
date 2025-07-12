namespace UserManagement.Common.Helpers;
public class APIResponse
{
    public APIResponse(int statusCode, string message = null!, object data = null!)
    {
        StatusCode = statusCode;
        Message = message ?? GetDefaultMessage(statusCode);
        Data = data ?? new { };
    }

    public int StatusCode { get; set; }
    public string Message { get; set; }
    public object Data { get; set; } = null!;
    private string GetDefaultMessage(int statusCode)
    {
        return statusCode switch
        {
            400 => "Bad Request",
            401 => "Unauthorized",
            404 => "Not Found",
            500 => "Internal Server Error",
            _ => null!
        };
    }
}
