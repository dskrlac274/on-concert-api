using OnConcert.BL.Models.Dtos.Band.Rating;

namespace OnConcert.BL.Models.Dtos.Band
{
    public class BandResponseDto : BandDetailsDto
    {
        public List<GetBandRatingDto> Ratings { get; set; } = new();
    }
}