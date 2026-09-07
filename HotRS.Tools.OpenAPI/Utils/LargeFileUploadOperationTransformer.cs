using HotRS.Tools.OpenAPI.Attributes;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace HotRS.Tools.OpenAPI.Utils;

public sealed class LargeFileUploadOperationTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(
        OpenApiOperation operation,
        OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        if (context.Description.ActionDescriptor is not ControllerActionDescriptor action)
        {
            return Task.CompletedTask;
        }

        var attribute = action.MethodInfo
            .GetCustomAttributes(typeof(LargeFileUploadAttribute), inherit: true)
            .OfType<LargeFileUploadAttribute>()
            .FirstOrDefault();

        if (attribute is null)
        {
            return Task.CompletedTask;
        }

        var schema = new OpenApiSchema
        {
            Type = JsonSchemaType.Object,
            Properties = new Dictionary<string, IOpenApiSchema>
            {
                [attribute.ParameterName] = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Format = "binary"
                }
            }
        };

        if (attribute.Required)
        {
            schema.Required = new HashSet<string>
            {
                attribute.ParameterName
            };
        }

        operation.RequestBody = new OpenApiRequestBody
        {
            Required = attribute.Required,
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["multipart/form-data"] = new OpenApiMediaType
                {
                    Schema = schema
                }
            }
        };

        return Task.CompletedTask;
    }
}