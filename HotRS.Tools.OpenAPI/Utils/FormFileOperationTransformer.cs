using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace HotRS.Tools.OpenAPI.Utils;

public sealed class FormFileOperationTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        if (context.Description.ActionDescriptor is not ControllerActionDescriptor action)
        {
            return Task.CompletedTask;
        }

        var formFileParameter = action.MethodInfo
            .GetParameters()
            .FirstOrDefault(p => p.ParameterType == typeof(IFormFile));

        if (formFileParameter is null)
            return Task.CompletedTask;

        var fieldName = formFileParameter.Name ?? "file";

        operation.RequestBody = new OpenApiRequestBody
        {
            Required = true,
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["multipart/form-data"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type = JsonSchemaType.Object,

                        Properties =
                            new Dictionary<string, IOpenApiSchema>
                            {
                                [fieldName] = new OpenApiSchema
                                {
                                    Type = JsonSchemaType.String,
                                    Format = "binary"
                                }
                            },

                        Required = new HashSet<string>
                        {
                            fieldName
                        }
                    }
                }
            }
        };

        return Task.CompletedTask;
    }
}