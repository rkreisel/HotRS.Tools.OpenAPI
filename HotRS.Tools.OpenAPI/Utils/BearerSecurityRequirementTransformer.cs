using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace HotRS.Tools.OpenAPI.Utils;

public sealed class BearerSecurityRequirementTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        var metadata = context.Description.ActionDescriptor.EndpointMetadata;

        // Framework [AllowAnonymous]
        if (metadata.OfType<IAllowAnonymous>().Any())
        {
            return Task.CompletedTask;
        }

        // Framework [Authorize]
        var requiresAuthorization = metadata.OfType<IAuthorizeData>().Any();

        if (!requiresAuthorization)
        {
            return Task.CompletedTask;
        }

        operation.Security ??= [];

        operation.Security.Add(
            new OpenApiSecurityRequirement
            {
                [
                    new OpenApiSecuritySchemeReference(
                        "Bearer",
                        context.Document)
                ] = []
            });

        return Task.CompletedTask;
    }
}