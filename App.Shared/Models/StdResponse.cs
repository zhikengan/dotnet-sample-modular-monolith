using System.Text.Json.Serialization;

namespace App.Shared.Models;

public class StdResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ErrorCode { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ErrorMessage { get; set; }

    public static StdResponse<T> Ok(T data) => new() { Success = true, Data = data };
    public static StdResponse<T> Fail(string code, string message) => new() { Success = false, ErrorCode = code, ErrorMessage = message };
}

public class StdResponse
{
    public bool Success { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ErrorCode { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ErrorMessage { get; set; }

    public static StdResponse Ok() => new() { Success = true };
    public static StdResponse Fail(string code, string message) => new() { Success = false, ErrorCode = code, ErrorMessage = message };
}
