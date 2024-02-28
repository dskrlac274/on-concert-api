namespace OnConcert.BL.Models.Dtos.Band.Rating
{
    public class DeleteBandRatingDto
    {
        public int Id { get; set; }
        public int BandId { get; set; }
        public int? OrganizerId { get; set; }
        public int? VisitorId { get; set; }
    }
}