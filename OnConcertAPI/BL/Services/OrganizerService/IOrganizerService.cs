using OnConcert.BL.Models;
using OnConcert.BL.Models.Dtos.Organizer;

namespace OnConcert.BL.Services.OrganizerService
{
    public interface IOrganizerService
    {
        Task<ServiceResponse<OrganizerDetailsDto>> GetById(int id);
        Task<EmptyServiceResponse> Update(OrganizerDetailsDto organizerDetails);
    }
}