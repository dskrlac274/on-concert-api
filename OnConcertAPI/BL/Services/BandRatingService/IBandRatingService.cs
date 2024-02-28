using OnConcert.BL.Models;
using OnConcert.BL.Models.Dtos.Band.Rating;

namespace OnConcert.BL.Services.BandRatingService
{
    public interface IBandRatingService
    {
        Task<ServiceResponse<BandRatingResponseDto>> Create(CreateBandRatingDto createBandRatingDto);
        Task<EmptyServiceResponse> Delete(DeleteBandRatingDto deleteBandRatingDto);
    }
}