namespace OnConcert.BL.Models.Dtos.Event.Application;

public class UpdateEventApplicationDto : UpdateEventApplicationBaseDto
{
    public int OrganizerId { get; set; }
    public int EventId { get; set; }
    public int ApplicationId { get; set; }
}