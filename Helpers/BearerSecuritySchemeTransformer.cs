using System;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace APIEcommerce.Helpers;

public class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
  public Task TransformAsync(
    OpenApiDocument document,
    OpenApiDocumentTransformerContext context,
    CancellationToken cancellationToken
  )
  {
    document.Components ??= new OpenApiComponents();

    OpenApiSecurityScheme securityScheme = new OpenApiSecurityScheme
    {
      Type = SecuritySchemeType.Http,
      Scheme = "bearer",
      BearerFormat = "JWT"
    };


    document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>()
    {
      ["Bearer"] = securityScheme
    };

    return Task.CompletedTask;
  }
}
