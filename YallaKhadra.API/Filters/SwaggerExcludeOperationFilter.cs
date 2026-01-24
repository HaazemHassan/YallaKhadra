using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using YallaKhadra.Core.Attributes;

namespace YallaKhadra.Api.Filters;

/// <summary>
/// Removes properties marked with <see cref="SwaggerExcludeAttribute"/> from the request body schema
/// in Swagger UI for a specific endpoint.
/// 
/// This helps hide internal or non-editable fields from API documentation
/// and ensures clients see only the fields they are allowed to send.
/// 
/// Note: This affects Swagger documentation only, not runtime validation.
/// </summary>
public class SwaggerExcludeOperationFilter : IOperationFilter {
    public void Apply(OpenApiOperation operation, OperationFilterContext context) {
        if (operation.RequestBody?.Content == null)
            return;

        // Get the parameter types from the action method
        var parametersToExclude = context.MethodInfo.GetParameters()
            .SelectMany(p => p.ParameterType.GetProperties())
            .Where(prop => prop.GetCustomAttribute<SwaggerExcludeAttribute>() != null)
            .Select(prop => prop.Name)
            .ToList();

        if (!parametersToExclude.Any())
            return;

        // Process each content type (application/json, multipart/form-data, etc.)
        foreach (var contentType in operation.RequestBody.Content.Values) {
            if (contentType.Schema?.Properties == null)
                continue;

            // Remove excluded properties from the schema
            foreach (var propertyName in parametersToExclude) {
                var keyToRemove = contentType.Schema.Properties.Keys
                    .FirstOrDefault(k => string.Equals(k, propertyName, StringComparison.OrdinalIgnoreCase));

                if (keyToRemove != null) {
                    contentType.Schema.Properties.Remove(keyToRemove);

                    // Also remove from required list if present
                    if (contentType.Schema.Required != null && contentType.Schema.Required.Contains(keyToRemove)) {
                        contentType.Schema.Required.Remove(keyToRemove);
                    }
                }
            }
        }
    }
}
