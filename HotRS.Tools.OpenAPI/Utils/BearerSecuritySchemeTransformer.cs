using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace HotRS.Tools.OpenAPI.Utils;

public sealed class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        const string schemeName = "Bearer";

        document.Components ??= new OpenApiComponents();

        document.Components.SecuritySchemes =
            new Dictionary<string, IOpenApiSecurityScheme>
            {
                [schemeName] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    In = ParameterLocation.Header,
                    BearerFormat = "JWT",
                    Description = "Enter JWT Bearer token only"
                }
            };

        document.Extensions ??=
            new Dictionary<string, IOpenApiExtension>();

        return Task.CompletedTask;
    }
}