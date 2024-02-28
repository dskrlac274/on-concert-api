using OnConcert.BL.Models.Dtos.Band;
using OnConcert.BL.Models.Dtos.Organizer;
using OnConcert.BL.Models.Dtos.Place;
using OnConcert.BL.Models.Enums;
using Swashbuckle.AspNetCore.Filters;

namespace OnConcert.Api.Swagger.Examples
{
    internal class UserDetailsRequestExamples : IMultipleExamplesProvider<object>
    {
        public IEnumerable<SwaggerExample<object>> GetExamples()
        {
            var examples = new Dictionary<string, object>
            {
                { nameof(UserRole.Band), new BandDetailsDto() },
                { nameof(UserRole.Organizer), new OrganizerDetailsDto() },
                { nameof(UserRole.Place), new PlaceDetailsDto() },
            };

            foreach (var (roleName, responseExample) in examples)
            {
                yield return SwaggerExample.Create(roleName, responseExample);
            }
        }
    }
}