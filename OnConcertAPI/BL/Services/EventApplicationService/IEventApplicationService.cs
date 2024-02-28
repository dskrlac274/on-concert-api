using OnConcert.BL.Models.Dtos.Event.Application;
using OnConcert.BL.Models;

namespace OnConcert.BL.Services.EventApplicationService
{
    public interface IEventApplicationService
    {
        Task<ServiceResponse<List<EventApplicationResponseDto>>> GetAll(GetEventApplicationsDto request);
        Task<ServiceResponse<EventApplicationResponseDto>> Create(CreateEventApplicationDto request);
        Task<ServiceResponse<EventApplicationResponseDto>> Update(UpdateEventApplicationDto request);
    }
}
