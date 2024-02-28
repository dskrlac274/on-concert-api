using System.Collections.Immutable;
using System.Reflection;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OnConcert.Api.Swagger.Filters
{
    internal class AuthorizationMarkupFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var roles = GetRolesFromPolicy(context.MethodInfo);

            var sb = new StringBuilder("<div><b>Allowed roles:</b><ul>");

            if (roles.Any())
            {
                foreach (var role in roles.OrderBy(r => r))
                    sb.Append($"<li>{HttpUtility.HtmlEncode(role)}</li>");
            }
            else
            {
                sb.Append("<li>All</li>");
            }

            sb.Append("</ul></div>");

            operation.Description += sb.ToString();
        }

        private static ImmutableHashSet<string> GetRolesFromPolicy(MemberInfo memberInfo)
        {
            var attributes = memberInfo.GetCustomAttributes<AuthorizeAttribute>(true).ToHashSet();
            if (memberInfo.DeclaringType is not null)
                attributes
                    .UnionWith(memberInfo.DeclaringType.GetTypeInfo()
                        .GetCustomAttributes<AuthorizeAttribute>(true));

            var roles = from a in attributes
                where !string.IsNullOrWhiteSpace(a.Policy)
                select a.Policy?.Split("&&");

            return roles.SelectMany(r => r).ToImmutableHashSet();
        }
    }
}