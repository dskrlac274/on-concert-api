using OnConcert.BL.Models.Dtos.Event;
using OnConcert.BL.Models;
using OnConcert.BL.Models.Dtos.Organizer;

namespace OnConcert.BL.Services.EventService
{
    public interface IEventService
    {
        Task<ServiceResponse<List<EventResponseDto>>> Search(EventSearchFilter searchFilter);
        Task<ServiceResponse<EventResponseDto>> Create(CreateEventDto createEventDto);
        Task<ServiceResponse<EventResponseDto>> Patch(AllGenericPatchEventDto genericPatchEventDto);
        Task<ServiceResponse<EventResponseDto>> Delete(OrganizerDeleteEventDto genericPatchEventDto);
    }
}