using App.Shared.Attributes;
using App.Shared.Models;
using Microsoft.AspNetCore.Http;
using System.Reflection;

namespace App.Shared.Infrastructure;

public class ResponseWrapperFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        // 1. Check for SkipWrapping attribute on the endpoint metadata
        if (context.HttpContext.GetEndpoint()?.Metadata.GetMetadata<SkipWrappingAttribute>() is not null)
        {
            return await next(context);
        }

        var result = await next(context);

        // 2. Handle PagedResult<T> -> StdPagedResponse<T>
        if (result is not null && IsPagedResult(result, out var items, out var page, out var pageSize, out var totalCount))
        {
            var resultType = result.GetType();
            if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(PagedResult<>))
            {
                var itemType = resultType.GetGenericArguments()[0];
                var responseType = typeof(StdPagedResponse<>).MakeGenericType(itemType);
                
                var method = responseType.GetMethod("Ok", BindingFlags.Public | BindingFlags.Static);
                return method?.Invoke(null, new object[] { items!, page, pageSize, totalCount });
            }
        }

        // 3. Handle void/Task (no result) -> StdResponse.Ok()
        if (result is null)
        {
            return StdResponse.Ok();
        }

        // 4. Handle standard result -> StdResponse<T>.Ok(result)
        if (result is IResult) return result;

        var genericResponseType = typeof(StdResponse<>).MakeGenericType(result.GetType());
        var okMethod = genericResponseType.GetMethod("Ok", BindingFlags.Public | BindingFlags.Static);
        return okMethod?.Invoke(null, new[] { result });
    }

    private static bool IsPagedResult(object result, out object? items, out int page, out int pageSize, out int totalCount)
    {
        items = null;
        page = 0;
        pageSize = 0;
        totalCount = 0;

        var type = result.GetType();
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(PagedResult<>))
        {
            items = type.GetProperty("Items")?.GetValue(result);
            page = (int)(type.GetProperty("Page")?.GetValue(result) ?? 0);
            pageSize = (int)(type.GetProperty("PageSize")?.GetValue(result) ?? 0);
            totalCount = (int)(type.GetProperty("TotalCount")?.GetValue(result) ?? 0);
            return true;
        }
        return false;
    }
}
