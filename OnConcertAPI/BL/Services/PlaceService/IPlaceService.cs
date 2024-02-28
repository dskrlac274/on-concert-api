using OnConcert.BL.Models;
using OnConcert.BL.Models.Dtos.Place;

namespace OnConcert.BL.Services.PlaceService
{
    public interface IPlaceService
    {
        Task<ServiceResponse<List<PlaceResponseDto>>> GetAll();
        Task<EmptyServiceResponse> Update(PlaceDetailsDto placeDetails);
    }
}