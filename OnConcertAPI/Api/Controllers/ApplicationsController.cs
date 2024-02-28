using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnConcert.BL.Models.Dtos.Event.Application;
using OnConcert.BL.Models.Enums;
using OnConcert.BL.Models;
using System.Security.Claims;
using OnConcert.BL.Services.EventApplicationService;

namespace OnConcert.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Tags("Events")]
    [Route("api/v1/Events/{id:int}/[controller]")]
    public class ApplicationsController : ControllerBase
    {
        private readonly IEventApplicationService _eventApplicationService;

        public ApplicationsController(IEventApplicationService eventApplicationService)
        {
            _eventApplicationService = eventApplicationService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<EventApplicationResponseDto>>>> GetEventApplications(
            int id
        )
        {
            var response = await _eventApplicationService.GetAll(new GetEventApplicationsDto
            {
                EventId = id
            });

            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost]
        [Authorize(Policy = "Band && Visitor")]
        public async Task<ActionResult<ServiceResponse<EventApplicationResponseDto>>> CreateEventApplication(
            int id
        )
        {
            var userId = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var userRole = Enum.Parse<UserRole>(HttpContext.User.FindFirstValue(ClaimTypes.Role)!);

            var eventApplication = new CreateEventApplicationDto { EventId = id };
            if (userRole == UserRole.Band) eventApplication.BandId = userId;
            else eventApplication.VisitorId = userId;

            var response = await _eventApplicationService.Create(eventApplication);

            return response.Success ? StatusCode(201, response) : BadRequest(response);
        }
        
        [HttpPatch("{applicationId:int}")]
        [Authorize(Policy = "Organizer")]
        public async Task<ActionResult<ServiceResponse<EventApplicationResponseDto>>> PatchEventApplication(
            int id, int applicationId, [FromBody] UpdateEventApplicationBaseDto request
        )
        {
            var response = await _eventApplicationService.Update(new UpdateEventApplicationDto
            {
                OrganizerId = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!),
                EventId = id,
                ApplicationId = applicationId,
                AcceptBand = request.AcceptBand
            });

            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}
