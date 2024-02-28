using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnConcert.BL.Models;
using OnConcert.BL.Services.EventMessageService;
using System.Security.Claims;
using OnConcert.BL.Models.Dtos.Event.Message;
using OnConcert.BL.Models.Enums;

namespace OnConcert.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Tags("Events")]
    [Route("api/v1/Events/{id:int}/message")]
    public class EventsMessageController : ControllerBase
    {
        private readonly IEventMessageService _eventMessageService;

        public EventsMessageController(IEventMessageService messageService)
        {
            _eventMessageService = messageService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<EventMessageResponseDto>>>> GetEventMessages(
            int id
        )
        {
            var role = Enum.Parse<UserRole>(HttpContext.User.FindFirstValue(ClaimTypes.Role)!);

            var getEventMessagesDto = new EventMessageRequestDto
            {
                Role = role,
                OrganizerId = role == UserRole.Organizer ? int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!) : 0,
                BandId = role != UserRole.Organizer ? int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!) : 0,
                EventId = id
            };

            var response = await _eventMessageService.GetAll(getEventMessagesDto);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost]
        [Authorize(Policy = "Organizer && Band")]
        public async Task<ActionResult<ServiceResponse<EventMessageResponseDto>>> CreateEventMessage(
            int id, [FromBody] EventMessageRequestDto createEventMessageDto
        )
        {
            var userId = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            createEventMessageDto.Role = Enum.Parse<UserRole>(HttpContext.User.FindFirstValue(ClaimTypes.Role)!);
            createEventMessageDto.EventId = id;

            if (createEventMessageDto.Role == UserRole.Organizer)
                createEventMessageDto.OrganizerId = userId;
            else
                createEventMessageDto.BandId = userId;

            var response = await _eventMessageService.Create(createEventMessageDto);

            return response.Success ? StatusCode(201, response) : BadRequest(response);
        }

        [HttpDelete("{messageId:int}")]
        [Authorize(Policy = "Organizer && Band")]
        public async Task<ActionResult<EmptyServiceResponse>> DeleteEventMessage(
            int id, int messageId
        )
        {
            var role = Enum.Parse<UserRole>(HttpContext.User.FindFirstValue(ClaimTypes.Role)!);

            var response = await _eventMessageService.Delete(new DeleteEventMessageRequestDto
            {
                Id = messageId,
                Role = role,
                OrganizerId = role == UserRole.Organizer ? int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!) : 0,
                BandId = role != UserRole.Organizer ? int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!) : 0,
                EventId = id
            });

            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}