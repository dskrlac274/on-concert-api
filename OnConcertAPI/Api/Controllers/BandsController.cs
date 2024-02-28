using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnConcert.BL.Models;
using OnConcert.BL.Models.Dtos.Band;
using OnConcert.BL.Services.BandService;

namespace OnConcert.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BandsController : ControllerBase
    {
        private readonly IBandService _bandService;

        public BandsController(IBandService bandService)
        {
            _bandService = bandService;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<BandResponseDto>>>> GetBands()
        {
            var response = await _bandService.GetAll();

            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}