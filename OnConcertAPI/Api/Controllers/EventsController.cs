using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnConcert.Api.Swagger.Examples;
using OnConcert.BL.Models;
using OnConcert.BL.Models.Dtos.Event;
using OnConcert.BL.Models.Dtos.Organizer;
using OnConcert.BL.Models.Enums;
using OnConcert.BL.Services.EventService;
using Swashbuckle.AspNetCore.Filters;
using System.Security.Claims;

namespace OnConcert.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<EventResponseDto>>>> SearchEvent(
            [FromQuery] string? search = null, [FromQuery] bool? upcoming = null
        )
        {
            var response = await _eventService.Search(new EventSearchFilter
            {
                Search = search ?? string.Empty,
                Upcoming = upcoming
            });
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost]
        [Authorize(Policy = "Organizer")]
        public async Task<ActionResult<ServiceResponse<EventResponseDto>>> CreateEvent(
            [FromBody] CreateEventDto createEventDto
        )
        {
            createEventDto.OrganizerId = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _eventService.Create(createEventDto);

            return response.Success ? StatusCode(201, response) : BadRequest(response);
        }

        [HttpPatch("{id}")]
        [Authorize(Policy = "Organizer && Place")]
        [SwaggerRequestExample(typeof(object), typeof(PatchEventRequestExamples))]
        public async Task<ActionResult<ServiceResponse<EventResponseDto>>> PatchEvent(
            int id, [FromBody] AllGenericPatchEventDto genericPatchEventDto
        )
        {
            genericPatchEventDto.Role = Enum.Parse<UserRole>(HttpContext.User.FindFirstValue(ClaimTypes.Role)!);
            genericPatchEventDto.Id = id;

            if (genericPatchEventDto.Role == UserRole.Organizer)
                genericPatchEventDto.OrganizerId = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            else
                genericPatchEventDto.PlaceId = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var response = await _eventService.Patch(genericPatchEventDto);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "Organizer")]
        public async Task<ActionResult<ServiceResponse<EventResponseDto>>> DeleteEvent(
            int id
        )
        {
            var response = await _eventService.Delete(new OrganizerDeleteEventDto
            {
                Id = id,
                OrganizerId = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!)
            });

            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}