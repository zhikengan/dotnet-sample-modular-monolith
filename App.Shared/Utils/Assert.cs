using App.Shared.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace App.Shared.Utils;

public static class Assert
{
    public static void NotNull([NotNull] object? obj, ErrorCodes errorCode)
    {
        if (obj == null)
        {
            throw new BusinessException(errorCode);
        }
    }

    public static void True(bool condition, ErrorCodes errorCode)
    {
        if (!condition)
        {
            throw new BusinessException(errorCode);
        }
    }

    public static void False(bool condition, ErrorCodes errorCode)
    {
        if (condition)
        {
            throw new BusinessException(errorCode);
        }
    }
    public static void Authentication(bool condition, string message = "Unauthorized access")
    {
        if (!condition)
        {
            throw new UnauthorizedAccessException(message);
        }
    }
}
