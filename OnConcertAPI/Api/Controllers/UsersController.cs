using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnConcert.Api.Swagger.Examples;
using OnConcert.BL.Models;
using OnConcert.BL.Models.Dtos.User;
using OnConcert.BL.Models.Enums;
using OnConcert.BL.Services.UserService;
using Swashbuckle.AspNetCore.Filters;

namespace OnConcert.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(UserDetailsResponseExamples))]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<UserResponseDto>>>> GetAllUsers()
        {
            var response = await _userService.GetAll();

            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("current")]
        public async Task<ActionResult<ServiceResponse<object>>> GetCurrentUser()
        {
            var response = await _userService.GetCurrentUser(new GetUserDto
            {
                Id = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!),
                Role = Enum.Parse<UserRole>(HttpContext.User.FindFirstValue(ClaimTypes.Role)!)
            });

            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPatch("current")]
        [Authorize(Policy = "Organizer && Place && Band")]
        [SwaggerRequestExample(typeof(object), typeof(UserDetailsRequestExamples))]
        public async Task<ActionResult<ServiceResponse<object>>> PatchCurrentUser(
            [FromBody] AllUserDetailsDto requestDto
        )
        {
            requestDto.Id = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            requestDto.Role = Enum.Parse<UserRole>(HttpContext.User.FindFirstValue(ClaimTypes.Role)!);

            var response = await _userService.UpdateCurrentUser(requestDto);

            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}