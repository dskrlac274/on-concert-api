using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnConcert.BL.Models.Enums;
using OnConcert.BL.Models;
using OnConcert.BL.Services.PlaceRatingService;
using System.Security.Claims;
using OnConcert.BL.Models.Dtos.Place.Rating;

namespace OnConcert.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Tags("Places")]
    [Route("api/v1/Places/{id:int}/rating")]
    public class PlacesRatingController : ControllerBase
    {
        private readonly IPlaceRatingService _placeRatingService;

        public PlacesRatingController(IPlaceRatingService placeRatingService)
        {
            _placeRatingService = placeRatingService;
        }

        [HttpPost]
        [Authorize(Policy = "Band && Visitor")]
        public async Task<ActionResult<ServiceResponse<PlaceRatingResponseDto>>> RatePlace(
            int id, [FromBody] CreatePlaceRatingDto createPlaceRatingDto
        )
        {
            createPlaceRatingDto.PlaceId = id;
            string role = Enum.Parse<UserRole>(HttpContext.User.FindFirstValue(ClaimTypes.Role)!).ToString();

            if (role.Equals("Band"))
            {
                createPlaceRatingDto.BandId = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                createPlaceRatingDto.VisitorId = null;
            }
            else if (role.Equals("Visitor"))
            {
                createPlaceRatingDto.VisitorId = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                createPlaceRatingDto.BandId = null;
            }

            var response = await _placeRatingService.Create(createPlaceRatingDto);

            return response.Success ? StatusCode(201, response) : BadRequest(response);
        }

        [HttpDelete("{ratingId:int}")]
        [Authorize(Policy = "Band && Visitor")]
        public async Task<ActionResult<EmptyServiceResponse>> DeletePlaceRating(
           int id, int ratingId
        )
        {
            string role = Enum.Parse<UserRole>(HttpContext.User.FindFirstValue(ClaimTypes.Role)!).ToString();

            DeletePlaceRatingDto deletePlaceRatingDto = new()
            {
                Id = ratingId,
                PlaceId = id
            };

            if (role.Equals("Band"))
            {
                deletePlaceRatingDto.BandId = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);                
            }
            else if (role.Equals("Visitor"))
            {
                deletePlaceRatingDto.VisitorId = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            }

            var response = await _placeRatingService.Delete(deletePlaceRatingDto);

            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}