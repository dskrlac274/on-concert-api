using OnConcert.BL.Models.Dtos.Organizer;
using OnConcert.BL.Models.Dtos.Place;
using OnConcert.BL.Models.Enums;
using Swashbuckle.AspNetCore.Filters;

namespace OnConcert.Api.Swagger.Examples
{
    public class PatchEventRequestExamples : IMultipleExamplesProvider<object>
    {
        public IEnumerable<SwaggerExample<object>> GetExamples()
        {
            var examples = new Dictionary<string, object>
            {
                { nameof(UserRole.Organizer), new OrganizerPatchEventDto() },
                { nameof(UserRole.Place), new PlacePatchEventDto() },
            };

            foreach (var (roleName, responseExample) in examples)
            {
                yield return SwaggerExample.Create(roleName, responseExample);
            }
        }
    }
}