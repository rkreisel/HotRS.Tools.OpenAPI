using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using System.Reflection;
using System.Text.Json.Nodes;

namespace HotRS.Tools.OpenAPI.Utils;

public sealed class ScalarDefaultValueOperationTransformer
    : IOpenApiOperationTransformer
{
    public Task TransformAsync(
        OpenApiOperation operation,
        OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        if (context.Description.ActionDescriptor
            is not ControllerActionDescriptor controllerAction)
        {
            return Task.CompletedTask;
        }

        foreach (var parameterInfo in controllerAction.MethodInfo.GetParameters())
        {
            var attribute =
                parameterInfo.GetCustomAttribute<ScalarDefaultValueAttribute>();

            if (attribute is null)
                continue;

            var parameter = operation.Parameters?
                .FirstOrDefault(p =>
                    string.Equals(
                        p.Name,
                        parameterInfo.Name,
                        StringComparison.OrdinalIgnoreCase));

            if (parameter is not OpenApiParameter openApiParameter)
                continue;

            openApiParameter.Example =
                CreateValue(attribute.Value);
        }

        return Task.CompletedTask;
    }

    private static JsonNode? CreateValue(object? value)
    {
        if (value is null)
            return null;

        // Works for ANY enum
        if (value.GetType().IsEnum)
            return JsonValue.Create(value.ToString());

        return JsonValue.Create(value);
    }
}