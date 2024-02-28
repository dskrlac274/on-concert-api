using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OnConcert.BL.Extensions;
using OnConcert.BL.Models;
using OnConcert.BL.Models.Dtos.Event.Application;
using OnConcert.DAL.Entities;
using OnConcert.DAL.Enums;
using OnConcert.DAL.Generic;

namespace OnConcert.BL.Services.EventApplicationService
{
    public class EventApplicationService : IEventApplicationService
    {
        private readonly IRepository<Event> _eventRepository;
        private readonly IRepository<EventApplication> _eventApplicationRepository;
        private readonly IReadOnlyRepository<Band> _bandRepository;
        private readonly IReadOnlyRepository<Visitor> _visitorRepository;
        private readonly IMapper _mapper;

        public EventApplicationService(
            IRepository<Event> eventRepository,
            IRepository<EventApplication> eventApplicationRepository,
            IReadOnlyRepository<Band> bandRepository,
            IReadOnlyRepository<Visitor> visitorRepository,
            IMapper mapper
        )
        {
            _eventRepository = eventRepository;
            _eventApplicationRepository = eventApplicationRepository;
            _bandRepository = bandRepository;
            _visitorRepository = visitorRepository;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<EventApplicationResponseDto>> Create(CreateEventApplicationDto request)
        {
            var fetchedEvent = await GetEventById(request.EventId, true);
            if (fetchedEvent == null)
                return ServiceResponseBuilder.CreateErrorResponse<EventApplicationResponseDto>("Event not found.");

            if (fetchedEvent.PlaceReservationStatus != PlaceReservationStatus.Approved)
                return ServiceResponseBuilder.CreateErrorResponse<EventApplicationResponseDto>("Event currently not applicable.");

            if(fetchedEvent.HasStarted())
                return ServiceResponseBuilder.CreateErrorResponse<EventApplicationResponseDto>("Event applications finished.");

            if (await CheckApplicationExists(request))
                return ServiceResponseBuilder.CreateErrorResponse<EventApplicationResponseDto>("Application already exists!");

            var applicationResponse = await CreateEventApplication(request, fetchedEvent);
            if (!applicationResponse.Success)
                return ServiceResponseBuilder.CreateErrorResponse<EventApplicationResponseDto>(applicationResponse.Message);

            _eventApplicationRepository.Add(applicationResponse.Data!);
            await _eventApplicationRepository.Save();

            return ServiceResponseBuilder.CreateSuccessResponse(
                _mapper.Map<EventApplicationResponseDto>(await GetApplication(request))
            );
        }

        public async Task<ServiceResponse<List<EventApplicationResponseDto>>> GetAll(GetEventApplicationsDto request)
        {
            var fetchedEvent = await GetEventById(request.EventId);
            if (fetchedEvent == null)
                return ServiceResponseBuilder.CreateErrorResponse<List<EventApplicationResponseDto>>("Event not found.");
            
            var eventApplications = await GetEventApplications(request.EventId);
            
            return ServiceResponseBuilder.CreateSuccessResponse(
                eventApplications.Select(_mapper.Map<EventApplicationResponseDto>).ToList()
            );
        }

        public async Task<ServiceResponse<EventApplicationResponseDto>> Update(UpdateEventApplicationDto request)
        {
            var fetchedEvent = await GetEventById(request.EventId);
            if (fetchedEvent == null || fetchedEvent.OrganizerId != request.OrganizerId)
                return ServiceResponseBuilder.CreateErrorResponse<EventApplicationResponseDto>("Event not found.");

            if (fetchedEvent.PlaceReservationStatus != PlaceReservationStatus.Approved)
                return ServiceResponseBuilder.CreateErrorResponse<EventApplicationResponseDto>(
                    "Event currently not updateable.");

            if (fetchedEvent.HasStarted())
                return ServiceResponseBuilder.CreateErrorResponse<EventApplicationResponseDto>(
                    "The event has already started.");
            
            var fetchedEventApplication = await GetEventApplication(request.ApplicationId);
            if (fetchedEventApplication == null || fetchedEventApplication.EventId != fetchedEvent.Id)
                return ServiceResponseBuilder.CreateErrorResponse<EventApplicationResponseDto>(
                    "The event application not found.");

            if (fetchedEventApplication.BandId == null)
                return ServiceResponseBuilder.CreateErrorResponse<EventApplicationResponseDto>(
                    "Only band applications are updateable.");

            fetchedEventApplication.BandApplicationStatus =
                request.AcceptBand ? BandApplicationStatus.Approved : BandApplicationStatus.Declined;

            _eventApplicationRepository.Update(fetchedEventApplication);
            await _eventApplicationRepository.Save();

            return ServiceResponseBuilder.CreateSuccessResponse(
                _mapper.Map<EventApplicationResponseDto>(await GetEventApplication(fetchedEventApplication.Id))
            );
        }

        private async Task<ServiceResponse<EventApplication>> CreateEventApplication(
            CreateEventApplicationDto eventApplication, Event eventEntity
        )
        {
            var application = new EventApplication { EventId = eventApplication.EventId };

            if (eventApplication.BandId != null)
            {
                if (!await CheckBandExists(eventApplication.BandId.Value))
                    return ServiceResponseBuilder.CreateErrorResponse<EventApplication>("Band not found");

                application.BandId = eventApplication.BandId.Value;
                application.BandApplicationStatus = BandApplicationStatus.Requested;
            }

            if (eventApplication.VisitorId != null)
            {
                if (!await CheckVisitorExists(eventApplication.VisitorId.Value))
                    return ServiceResponseBuilder.CreateErrorResponse<EventApplication>("Visitor not found");

                if (!await CheckEventPlaceHasCapacity(eventEntity))
                    return ServiceResponseBuilder.CreateErrorResponse<EventApplication>("The event place is full.");
                
                application.BandApplicationStatus = BandApplicationStatus.Approved;
                application.VisitorId = eventApplication.VisitorId.Value;
            }

            return ServiceResponseBuilder.CreateSuccessResponse(application);
        }
        
        private Task<EventApplication?> GetApplication(CreateEventApplicationDto eventApplication)
        {
            if (eventApplication.BandId != null)
                return _eventApplicationRepository.GetAll().FirstOrDefaultAsync(a =>
                    a.EventId == eventApplication.EventId && a.BandId == eventApplication.BandId
                );

            return _eventApplicationRepository.GetAll().FirstOrDefaultAsync(a =>
                a.EventId == eventApplication.EventId && a.VisitorId == eventApplication.VisitorId
            );
        }

        private Task<Event?> GetEventById(int id, bool includePlace = false)
        {
            var query = _eventRepository.GetAll();
            if (includePlace) query = query.Include(e => e.Place);
            return query.FirstOrDefaultAsync(e => e.Id == id);
        }

        private Task<int> GetEventApplicationsCount(int eventId) =>
            _eventApplicationRepository.GetAll()
                .Where(a => a.EventId == eventId && a.VisitorId != default)
                .CountAsync();

        private Task<EventApplication?> GetEventApplication(int applicationId) =>
            _eventApplicationRepository.GetAll().FirstOrDefaultAsync(a => a.Id == applicationId);
        
        private Task<List<EventApplication>> GetEventApplications(int eventId) =>
            _eventApplicationRepository.GetAll()
                .Where(a => a.EventId == eventId)
                .ToListAsync();

        private async Task<bool> CheckEventPlaceHasCapacity(Event eventEntity) =>
            await GetEventApplicationsCount(eventEntity.Id) < eventEntity.Place.Capacity;

        private async Task<bool> CheckApplicationExists(CreateEventApplicationDto eventApplication) =>
            await GetApplication(eventApplication) != null;

        private async Task<bool> CheckBandExists(int id) =>
            await GetBandById(id) != null;

        private async Task<bool> CheckVisitorExists(int id) =>
            await GetVisitorById(id) != null;

        private Task<Band?> GetBandById(int id) =>
            _bandRepository.GetAll().FirstOrDefaultAsync(b => b.Id == id);

        private Task<Visitor?> GetVisitorById(int id) =>
            _visitorRepository.GetAll().FirstOrDefaultAsync(v => v.Id == id);
    }
}
