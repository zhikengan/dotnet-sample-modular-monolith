namespace App.Shared.Exceptions;

public class ErrorCodes
{
    public string Code { get; }
    public string Message { get; }

    public ErrorCodes(string code, string message)
    {
        Code = code;
        Message = message;
    }

    // General
    public static readonly ErrorCodes INTERNAL_SERVER_ERROR = new("GEN_001", "Internal Server Error");
    public static readonly ErrorCodes INVALID_REQUEST = new("GEN_002", "Invalid Request");
    
    // User Module
    public static readonly ErrorCodes USER_NOT_FOUND = new("USR_001", "User not found");
    public static readonly ErrorCodes USER_EXISTS = new("USR_002", "User already exists");
    
    // Ordering Module
    public static readonly ErrorCodes ORDER_NOT_FOUND = new("ORD_001", "Order not found");
    public static readonly ErrorCodes INSUFFICIENT_FUNDS = new("ORD_002", "Insufficient funds");

    public override string ToString() => $"{Code}: {Message}";
}
