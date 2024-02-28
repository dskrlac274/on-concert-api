using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnConcert.BL.Models;
using OnConcert.BL.Services.PlaceService;
using OnConcert.BL.Models.Dtos.Place;

namespace OnConcert.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PlacesController : ControllerBase
    {
        private readonly IPlaceService _placeService;

        public PlacesController(IPlaceService placeService)
        {
            _placeService = placeService;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<PlaceResponseDto>>>> GetPlaces()
        {
            var response = await _placeService.GetAll();

            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}