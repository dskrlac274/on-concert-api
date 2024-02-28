using OnConcert.BL.Models;
using OnConcert.BL.Models.Dtos.Band;

namespace OnConcert.BL.Services.BandService
{
    public interface IBandService
    {
        Task<ServiceResponse<List<BandResponseDto>>> GetAll();
        Task<EmptyServiceResponse> Update(BandDetailsDto bandDetails);
    }
}