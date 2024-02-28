using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnConcert.BL.Models.Enums;
using OnConcert.BL.Models;
using OnConcert.BL.Services.BandRatingService;
using System.Security.Claims;
using OnConcert.BL.Models.Dtos.Band.Rating;

namespace OnConcert.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Tags("Bands")]
    [Route("api/v1/Bands/{id:int}/rating")]
    public class BandsRatingController : ControllerBase
    {
        private readonly IBandRatingService _bandRatingService;

        public BandsRatingController(IBandRatingService bandRatingService)
        {
            _bandRatingService = bandRatingService;
        }

        [HttpPost]
        [Authorize(Policy = "Organizer && Visitor")]
        public async Task<ActionResult<ServiceResponse<BandRatingResponseDto>>> RateBand(
            int id, [FromBody] CreateBandRatingDto createBandRatingDto
        )
        {
            var userId = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var userRole = Enum.Parse<UserRole>(HttpContext.User.FindFirstValue(ClaimTypes.Role)!);

            createBandRatingDto.BandId = id;
            createBandRatingDto.OrganizerId = userRole == UserRole.Organizer ? userId : null;
            createBandRatingDto.VisitorId = userRole == UserRole.Visitor ? userId : null;

            var response = await _bandRatingService.Create(createBandRatingDto);

            return response.Success ? StatusCode(201, response) : BadRequest(response);
        }

        [HttpDelete("{ratingId:int}")]
        [Authorize(Policy = "Organizer && Visitor")]
        public async Task<ActionResult<EmptyServiceResponse>> DeleteBandRating(
            int id, int ratingId
        )
        {
            var userId = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var userRole = Enum.Parse<UserRole>(HttpContext.User.FindFirstValue(ClaimTypes.Role)!);

            var response = await _bandRatingService.Delete(new DeleteBandRatingDto
            {
                Id = ratingId,
                BandId = id,
                OrganizerId = userRole == UserRole.Organizer ? userId : null,
                VisitorId = userRole == UserRole.Visitor ? userId : null
            });

            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}