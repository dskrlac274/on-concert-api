using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnConcert.BL.Models;
using OnConcert.BL.Models.Dtos.User;
using OnConcert.BL.Services.OrganizerService;

namespace OnConcert.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class OrganizersController : ControllerBase
    {
        private readonly IOrganizerService _organizerService;

        public OrganizersController(IOrganizerService organizerService)
        {
            _organizerService = organizerService;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ServiceResponse<LoginUserResponseDto>>> GetById(
            int id
        )
        {
            var response = await _organizerService.GetById(id);

            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}