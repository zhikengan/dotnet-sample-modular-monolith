namespace App.Shared.Exceptions;

public class BusinessException : Exception
{
    public ErrorCodes ErrorCode { get; }

    public BusinessException(ErrorCodes errorCode) : base(errorCode.Message)
    {
        ErrorCode = errorCode;
    }
}
