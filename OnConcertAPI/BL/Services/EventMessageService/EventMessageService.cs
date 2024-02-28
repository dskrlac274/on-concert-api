using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OnConcert.BL.Models;
using OnConcert.BL.Models.Dtos.Event.Message;
using OnConcert.BL.Models.Enums;
using OnConcert.DAL.Entities;
using OnConcert.DAL.Enums;
using OnConcert.DAL.Generic;

namespace OnConcert.BL.Services.EventMessageService
{
    public class EventMessageService : IEventMessageService
    {
        private readonly IRepository<Message> _eventMessageRepository;
        private readonly IRepository<Event> _eventRepository;
        private readonly IRepository<Band> _bandRepository;
        private readonly IRepository<Organizer> _organizerRepository;
        private readonly IRepository<EventApplication> _eventApplicationRepository;
        private readonly IMapper _mapper;

        public EventMessageService(
            IRepository<Message> messageRepository,
            IRepository<Event> eventRepository,
            IRepository<EventApplication> eventApplicationRepository,
            IRepository<Band> bandRepository,
            IRepository<Organizer> organizerRepository,
            IMapper mapper
        )
        {
            _eventMessageRepository = messageRepository;
            _eventRepository = eventRepository;
            _eventApplicationRepository = eventApplicationRepository;
            _bandRepository = bandRepository;
            _organizerRepository = organizerRepository;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<EventMessageResponseDto>>> GetAll(EventMessageRequestDto getEventMessagesDto)
        {
            var validationResponse = await CreateMessageValidationResponse(getEventMessagesDto);
            if (!validationResponse.Success)
                return ServiceResponseBuilder.CreateErrorResponse<List<EventMessageResponseDto>>(validationResponse.Message);

            var eventMessages = await GetEventMessages(getEventMessagesDto.EventId);

            var eventMessageResponseDtos = new List<EventMessageResponseDto>();
            foreach (var eventMessage in eventMessages)
            {
                var dto = _mapper.Map<EventMessageResponseDto>(eventMessage);

                if (eventMessage.BandId != null)
                {
                    var band = await GetBandById(eventMessage.BandId.Value);
                    if (band != null)
                        dto.UserId = band.Id;
                }

                if (eventMessage.OrganizerId != null)
                {
                    var organizer = await GetOrganizerById(eventMessage.OrganizerId.Value);
                    if (organizer != null)
                        dto.UserId = organizer.Id;
                }

                eventMessageResponseDtos.Add(dto);
            }

            return ServiceResponseBuilder.CreateSuccessResponse(eventMessageResponseDtos);
        }

        public async Task<ServiceResponse<EventMessageResponseDto>> Create(EventMessageRequestDto createEventMessageDto)
        {
            var response = await CreateMessageValidationResponse(createEventMessageDto);

            if (!response.Success)
                return ServiceResponseBuilder.CreateErrorResponse<EventMessageResponseDto>(response.Message);

            return ServiceResponseBuilder.CreateSuccessResponse(
                _mapper.Map<EventMessageResponseDto>(await CreateMessage(createEventMessageDto))
            );
        }

        public async Task<EmptyServiceResponse> Delete(DeleteEventMessageRequestDto deleteEventMessageDto)
        {
            var fetchedEvent = await GetEventById(deleteEventMessageDto.EventId);
            if (fetchedEvent == null)
                return EmptyServiceResponseBuilder.CreateErrorResponse("Event not found.");

            var fetchedMessage = await GetMessageById(deleteEventMessageDto.Id);
            if (fetchedMessage == null)
                return EmptyServiceResponseBuilder.CreateErrorResponse("Message not found.");

            if ((deleteEventMessageDto.Role == UserRole.Organizer && deleteEventMessageDto.OrganizerId != fetchedMessage.OrganizerId) ||
                (deleteEventMessageDto.Role == UserRole.Band && deleteEventMessageDto.BandId != fetchedMessage.BandId))
                return EmptyServiceResponseBuilder.CreateErrorResponse("Unauthorized to access this resource.");

            _eventMessageRepository.Delete(fetchedMessage);
            await _eventMessageRepository.Save();

            return EmptyServiceResponseBuilder.CreateSuccessResponse();
        }

        private async Task<EmptyServiceResponse> CreateMessageValidationResponse(EventMessageRequestDto eventMessageRequestDto)
        {
            var fetchedEvent = await GetEventById(eventMessageRequestDto.EventId);
            if (fetchedEvent == null)
                return EmptyServiceResponseBuilder.CreateErrorResponse("Event not found.");

            var bandEventApplication = eventMessageRequestDto.BandId != default
                ? await GetBandEventApplication(eventMessageRequestDto.EventId, eventMessageRequestDto.BandId) : null;

            if ((eventMessageRequestDto.Role == UserRole.Organizer && eventMessageRequestDto.OrganizerId != fetchedEvent.OrganizerId) ||
                (eventMessageRequestDto.Role == UserRole.Band && eventMessageRequestDto.BandId != bandEventApplication?.BandId))
                return EmptyServiceResponseBuilder.CreateErrorResponse("Unauthorized to access this resource.");

            return EmptyServiceResponseBuilder.CreateSuccessResponse();
        }

        private async Task<Message> CreateMessage(EventMessageRequestDto createEventMessageDto)
        {
            var message = _mapper.Map<Message>(createEventMessageDto);
            message.CreatedAt = DateTime.Now;

            _eventMessageRepository.Add(message);
            await _eventMessageRepository.Save();

            return message;
        }

        private Task<Event?> GetEventById(int eventId) => _eventRepository.GetAll().FirstOrDefaultAsync(e => e.Id == eventId);

        private Task<Band?> GetBandById(int bandId) => _bandRepository.GetAll().FirstOrDefaultAsync(b => b.Id == bandId);

        private Task<Organizer?> GetOrganizerById(int organizerId) => _organizerRepository.GetAll().FirstOrDefaultAsync(o => o.Id == organizerId);

        private Task<Message?> GetMessageById(int messageId) =>
            _eventMessageRepository.GetAll().FirstOrDefaultAsync(e => e.Id == messageId);

        private Task<EventApplication?> GetBandEventApplication(int eventId, int bandId) =>
            _eventApplicationRepository.GetAll()
            .FirstOrDefaultAsync(a => a.EventId == eventId && a.BandApplicationStatus == BandApplicationStatus.Approved && a.BandId == bandId);

        private Task<List<Message>> GetEventMessages(int eventId) => _eventMessageRepository.GetAll().Where(a => a.EventId == eventId).ToListAsync();
    }
}