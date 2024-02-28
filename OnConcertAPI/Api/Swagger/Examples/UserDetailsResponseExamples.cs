using OnConcert.BL.Models;
using OnConcert.BL.Models.Dtos.Band;
using OnConcert.BL.Models.Dtos.Organizer;
using OnConcert.BL.Models.Dtos.Place;
using OnConcert.BL.Models.Enums;
using Swashbuckle.AspNetCore.Filters;

namespace OnConcert.Api.Swagger.Examples
{
    internal class UserDetailsResponseExamples : IMultipleExamplesProvider<ServiceResponse<object>>
    {
        public IEnumerable<SwaggerExample<ServiceResponse<object>>> GetExamples()
        {
            var examples = new Dictionary<string, object>
            {
                { nameof(UserRole.Band), new BandDetailsDto() },
                { nameof(UserRole.Organizer), new OrganizerDetailsDto() },
                { nameof(UserRole.Place), new PlaceDetailsDto() },
            };

            foreach (var (roleName, responseExample) in examples)
            {
                yield return SwaggerExample.Create(roleName, new ServiceResponse<object>
                {
                    Data = responseExample
                });
            }
        }
    }
}