using OnConcert.BL.Models;
using OnConcert.BL.Models.Dtos.Event.Message;

namespace OnConcert.BL.Services.EventMessageService
{
    public interface IEventMessageService
    {
        Task<ServiceResponse<EventMessageResponseDto>> Create(EventMessageRequestDto request);
        Task<ServiceResponse<List<EventMessageResponseDto>>> GetAll(EventMessageRequestDto request);
        Task<EmptyServiceResponse> Delete(DeleteEventMessageRequestDto request);
    }
}