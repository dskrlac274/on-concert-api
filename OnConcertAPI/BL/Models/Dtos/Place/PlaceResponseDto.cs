using OnConcert.BL.Models.Dtos.Place.Rating;

namespace OnConcert.BL.Models.Dtos.Place
{
    public class PlaceResponseDto : PlaceDetailsDto
    {
        public List<GetPlaceRatingDto>? Ratings { get; set; }
    }
}