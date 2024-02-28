using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OnConcert.BL.Models;
using OnConcert.BL.Models.Dtos.Band;
using OnConcert.BL.Models.Dtos.Organizer;
using OnConcert.BL.Models.Dtos.Place;
using OnConcert.BL.Models.Dtos.User;
using OnConcert.BL.Models.Dtos.Visitor;
using OnConcert.BL.Models.Enums;
using OnConcert.BL.Services.AuthService;

namespace OnConcert.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IExternalAuthService _externalAuthService;
        private readonly IMapper _mapper;

        public AuthController(IAuthService authService, IExternalAuthService externalAuthService, IMapper mapper)
        {
            _authService = authService;
            _externalAuthService = externalAuthService;
            _mapper = mapper;
        }

        [HttpPost("login/{role}")]
        public async Task<ActionResult<ServiceResponse<LoginUserResponseDto>>> Login(
            [FromRoute] string role,
            [FromBody] LoginUserBaseDto request
        )
        {
            if (!Enum.TryParse(role, ignoreCase: true, out UserRole parsedRole))
            {
                return new ServiceResponse<LoginUserResponseDto>
                {
                    Success = false,
                    Message = "Unrecognized role."
                };
            }

            var loginRequest = _mapper.Map<LoginUserDto>(request);
            loginRequest.Role = parsedRole;
            var response = await _authService.Login(loginRequest);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("oauth")]
        public async Task<ActionResult<ServiceResponse<object>>> Login(
            [FromBody] VisitorDto request
        )
        {
            var authResponse = await _externalAuthService.Authenticate(request.Token);
            if (!authResponse.Success) return authResponse;

            var response = await _authService.Login(_mapper.Map<VisitorDto>(request));

            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("register/place")]
        public async Task<ActionResult<ServiceResponse<EmptyServiceResponse>>> RegisterPlace(
            [FromBody] RegisterPlaceDto request
        )
        {
            var response = await _authService.Register(request);

            return response.Success ? StatusCode(201, response) : BadRequest(response);
        }

        [HttpPost("register/organizer")]
        public async Task<ActionResult<ServiceResponse<EmptyServiceResponse>>> RegisterOrganizer(
            [FromBody] RegisterOrganizerDto request
        )
        {
            var response = await _authService.Register(request);

            return response.Success ? StatusCode(201, response) : BadRequest(response);
        }

        [HttpPost("register/band")]
        public async Task<ActionResult<ServiceResponse<EmptyServiceResponse>>> RegisterBand(
            [FromBody] RegisterBandDto request
        )
        {
            var response = await _authService.Register(request);

            return response.Success ? StatusCode(201, response) : BadRequest(response);
        }
    }
}