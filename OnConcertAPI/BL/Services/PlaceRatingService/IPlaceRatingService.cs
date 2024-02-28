using OnConcert.BL.Models;
using OnConcert.BL.Models.Dtos.Place.Rating;

namespace OnConcert.BL.Services.PlaceRatingService
{
    public interface IPlaceRatingService
    {
        Task<ServiceResponse<PlaceRatingResponseDto>> Create(CreatePlaceRatingDto createPlaceRatingDto);
        Task<EmptyServiceResponse> Delete(DeletePlaceRatingDto deletePlaceRatingDto);
    }
}