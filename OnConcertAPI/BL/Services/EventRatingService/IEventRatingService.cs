using OnConcert.BL.Models;
using OnConcert.BL.Models.Dtos.Event.Rating;

namespace OnConcert.BL.Services.EventRatingService
{
    public interface IEventRatingService
    {
        Task<ServiceResponse<EventRatingResponseDto>> Create(CreateEventRatingDto request);
        Task<EmptyServiceResponse> Delete(DeleteEventRatingDto request);
    }
}