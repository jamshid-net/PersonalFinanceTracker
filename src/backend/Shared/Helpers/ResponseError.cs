namespace Shared.Helpers;

public class ResponseError
{
    public ResponseError(int statusCode, string message)
    {
        StatusCode = statusCode;
        Message = message;
    }
    public ResponseError() { }
    public int? StatusCode { get; set; }
    public string? Message { get; set; }
}
