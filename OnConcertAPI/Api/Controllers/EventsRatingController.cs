using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnConcert.BL.Models;
using OnConcert.BL.Models.Dtos.Event.Rating;
using OnConcert.BL.Services.EventRatingService;
using System.Security.Claims;

namespace OnConcert.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Tags("Events")]
    [Route("api/v1/Events/{id:int}/rating")]
    public class EventsRatingController : ControllerBase
    {
        private readonly IEventRatingService _eventRatingService;

        public EventsRatingController(IEventRatingService eventRatingService)
        {
            _eventRatingService = eventRatingService;
        }

        [HttpPost]
        [Authorize(Policy = "Band")]
        public async Task<ActionResult<ServiceResponse<EventRatingResponseDto>>> RateEvent(
            int id, [FromBody] CreateEventRatingDto createEventRatingDto
        )
        {
            createEventRatingDto.EventId = id;
            createEventRatingDto.BandId = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var response = await _eventRatingService.Create(createEventRatingDto);

            return response.Success ? StatusCode(201, response) : BadRequest(response);
        }

        [HttpDelete("{ratingId:int}")]
        [Authorize(Policy = "Band")]
        public async Task<ActionResult<EmptyServiceResponse>> DeleteEventRating(
            int id, int ratingId
        )
        {
            var response = await _eventRatingService.Delete(new DeleteEventRatingDto
            {
                Id = ratingId,
                EventId = id,
                BandId = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!)
            });

            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}