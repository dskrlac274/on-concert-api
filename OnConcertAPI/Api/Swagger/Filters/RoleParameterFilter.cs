using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using OnConcert.BL.Models.Enums;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OnConcert.Api.Swagger.Filters
{
    internal class RoleParameterFilter : IParameterFilter
    {
        public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
        {
            if (!parameter.Name.Equals("role", StringComparison.InvariantCultureIgnoreCase)) return;
            parameter.Schema.Enum = Enum.GetNames(typeof(UserRole))
                .Select(role => new OpenApiString(role))
                .ToList<IOpenApiAny>();
        }
    }
}