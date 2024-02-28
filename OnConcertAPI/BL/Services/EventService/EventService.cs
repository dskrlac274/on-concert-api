using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OnConcert.BL.Models;
using OnConcert.BL.Models.Dtos.Event;
using OnConcert.BL.Models.Dtos.Event.Rating;
using OnConcert.BL.Models.Dtos.Organizer;
using OnConcert.BL.Models.Dtos.Place;
using OnConcert.BL.Models.Enums;
using OnConcert.Core.Helpers;
using OnConcert.DAL.Entities;
using OnConcert.DAL.Enums;
using OnConcert.DAL.Generic;

namespace OnConcert.BL.Services.EventService
{
    public class EventService : IEventService
    {
        private readonly IRepository<Organizer> _organizerRepository;
        private readonly IRepository<Place> _placeRepository;
        private readonly IRepository<Event> _eventRepository;
        private readonly IRepository<EventRating> _eventRatingRepository;
        private readonly IRepository<Rating> _ratingRepository;
        private readonly IMapper _mapper;

        public EventService(
            IRepository<Organizer> organizerRepository,
            IRepository<Place> placeRepository,
            IRepository<Event> eventRepository,
            IRepository<EventRating> eventRatingRepository,
            IRepository<Rating> ratingRepository,
            IMapper mapper
        )
        {
            _organizerRepository = organizerRepository;
            _placeRepository = placeRepository;
            _eventRepository = eventRepository;
            _eventRatingRepository = eventRatingRepository;
            _ratingRepository = ratingRepository;
            _mapper = mapper;
        }

        public Task<ServiceResponse<List<EventResponseDto>>> Search(EventSearchFilter searchFilter)
            => searchFilter.Upcoming switch
            {
                true => SearchAllByName(searchFilter.Search, true),
                false => SearchAllByName(searchFilter.Search, false),
                _ => SearchAllByName(searchFilter.Search)
            };

        public async Task<ServiceResponse<EventResponseDto>> Create(CreateEventDto createEventDto)
        {
            var organizer = await GetOrganizerById(createEventDto.OrganizerId);
            if (organizer == null)
                return ServiceResponseBuilder.CreateErrorResponse<EventResponseDto>("Organizer not found.");

            var place = await GetPlaceById(createEventDto.PlaceId);
            if (place == null)
                return ServiceResponseBuilder.CreateErrorResponse<EventResponseDto>("Place not found.");

            if (!DateValidator.IsValidDateRange(createEventDto.DateFrom, createEventDto.DateTo))
                return ServiceResponseBuilder.CreateErrorResponse<EventResponseDto>("Date range invalid.");

            if (await IsPlaceReserved(createEventDto))
                return ServiceResponseBuilder.CreateErrorResponse<EventResponseDto>("Place date range already reserved.");

            var newEvent = _mapper.Map<Event>(createEventDto);
            _eventRepository.Add(newEvent);
            await _eventRepository.Save();

            return ServiceResponseBuilder.CreateSuccessResponse(_mapper.Map<EventResponseDto>(newEvent));
        }

        public async Task<ServiceResponse<EventResponseDto>> Patch(AllGenericPatchEventDto genericPatchEventDto)
        {
            var fetchedEvent = await GetEventById(genericPatchEventDto.Id);
            if (fetchedEvent == null)
                return ServiceResponseBuilder.CreateErrorResponse<EventResponseDto>("Event not found.");

            if (genericPatchEventDto.Role == UserRole.Organizer && fetchedEvent.OrganizerId != genericPatchEventDto.OrganizerId)
                return ServiceResponseBuilder.CreateErrorResponse<EventResponseDto>("Unauthorized to update this event.");

            if (genericPatchEventDto.Role == UserRole.Place && fetchedEvent.PlaceId != genericPatchEventDto.PlaceId)
                return ServiceResponseBuilder.CreateErrorResponse<EventResponseDto>("Unauthorized to update this event.");

            var eventUpdateResponse = genericPatchEventDto.Role == UserRole.Organizer
                ? await UpdateEventForOrganizer(genericPatchEventDto, fetchedEvent)
                : await UpdateEventForPlace(genericPatchEventDto, fetchedEvent);

            if (eventUpdateResponse.Success == false)
                return ServiceResponseBuilder.CreateErrorResponse<EventResponseDto>(eventUpdateResponse.Message);

            var updatedEvent = eventUpdateResponse.Data!;
            _eventRepository.Update(updatedEvent);
            await _eventRepository.Save();

            return ServiceResponseBuilder.CreateSuccessResponse(_mapper.Map<EventResponseDto>(updatedEvent));
        }

        public async Task<ServiceResponse<EventResponseDto>> Delete(OrganizerDeleteEventDto genericPatchEventDto)
        {
            var fetchedEvent = await GetEventById(genericPatchEventDto.Id);
            if (fetchedEvent == null)
                return ServiceResponseBuilder.CreateErrorResponse<EventResponseDto>("Event not found.");

            if (fetchedEvent.OrganizerId != genericPatchEventDto.OrganizerId)
                return ServiceResponseBuilder.CreateErrorResponse<EventResponseDto>("Unauthorized to delete this event.");

            _eventRepository.Delete(fetchedEvent);
            await _eventRepository.Save();

            return ServiceResponseBuilder.CreateSuccessResponse(_mapper.Map<EventResponseDto>(fetchedEvent));
        }

        private async Task<ServiceResponse<List<EventResponseDto>>> SearchAllByName(string name) =>
            ServiceResponseBuilder.CreateSuccessResponse(
                await GetEventsWithRatings(
                    CreateSearchByNameQuery(name)
                )
            );

        private async Task<ServiceResponse<List<EventResponseDto>>> SearchAllByName(string name, bool upcoming) =>
            ServiceResponseBuilder.CreateSuccessResponse(
                await GetEventsWithRatings(
                    CreateSearchByNameQuery(name).Where(
                        upcoming ? e => e.DateFrom > DateTime.Now : e => e.DateFrom <= DateTime.Now
                    )
                )
            );

        private async Task<List<EventResponseDto>> GetEventsWithRatings(IQueryable<Event> query)
        {
            var eventsWithRatings = await
                (from e in query
                    join er in _eventRatingRepository.GetAll() on e.Id equals er.EventId into eventRatings
                    from subEventRating in eventRatings.DefaultIfEmpty()
                    join r in _ratingRepository.GetAll() on subEventRating.RatingId equals r.Id into ratings
                    from subRating in ratings.DefaultIfEmpty()
                    select new { e, subEventRating, subRating })
                .ToListAsync();

            return eventsWithRatings.GroupBy(x => x.e.Id).Select(group =>
            {
                var eventDto = _mapper.Map<EventResponseDto>(group.First().e);
                eventDto.Ratings = group
                    .Where(x => x.subRating != null)
                    .Select(x =>
                    {
                        var getEventRatingDto = _mapper.Map<GetEventRatingDto>(x.subEventRating);
                        getEventRatingDto.Score = x.subRating.Score;
                        getEventRatingDto.Review = x.subRating.Review;
                        return getEventRatingDto;
                    })
                    .ToList();
                return eventDto;
            }).ToList();
        }

        private async Task<ServiceResponse<Event>> UpdateEventForOrganizer(AllGenericPatchEventDto allGenericPatchEventDto, Event fetchedEvent)
        {
            var patchedEvent = _mapper.Map(_mapper.Map<OrganizerPatchEventDto>(allGenericPatchEventDto), fetchedEvent);

            if (allGenericPatchEventDto.DateFrom != DateTime.MinValue || allGenericPatchEventDto.DateTo != DateTime.MinValue)
            {
                if (allGenericPatchEventDto.PlaceId != 0)
                {
                    if (await GetPlaceById(allGenericPatchEventDto.PlaceId) == null)
                        return ServiceResponseBuilder.CreateErrorResponse<Event>("Place not found");

                    patchedEvent.PlaceId = allGenericPatchEventDto.PlaceId;
                }
                else
                {
                    allGenericPatchEventDto.PlaceId = fetchedEvent.PlaceId;
                }

                var startDate = allGenericPatchEventDto.DateFrom != DateTime.MinValue
                    ? allGenericPatchEventDto.DateFrom
                    : fetchedEvent.DateFrom;
                var endDate = allGenericPatchEventDto.DateTo != DateTime.MinValue
                    ? allGenericPatchEventDto.DateTo
                    : fetchedEvent.DateTo;

                if (!DateValidator.IsValidDateRange(startDate, endDate))
                    return ServiceResponseBuilder.CreateErrorResponse<Event>("Date range invalid.");

                if (await IsElsePlaceReserved(allGenericPatchEventDto))
                    return ServiceResponseBuilder.CreateErrorResponse<Event>("Place date range already reserved.");

                patchedEvent.DateFrom = startDate;
                patchedEvent.DateTo = endDate;
                patchedEvent.PlaceReservationStatus = PlaceReservationStatus.Requested;
            }

            return ServiceResponseBuilder.CreateSuccessResponse(patchedEvent);
        }

        private async Task<ServiceResponse<Event>> UpdateEventForPlace(AllGenericPatchEventDto allGenericPatchEventDto, Event fetchedEvent)
        {
            var patchedEvent = _mapper.Map(_mapper.Map<PlacePatchEventDto>(allGenericPatchEventDto), fetchedEvent);

            if (allGenericPatchEventDto.PlaceReservationStatus != PlaceReservationStatus.Requested)
            {
                if (allGenericPatchEventDto.PlaceReservationStatus == PlaceReservationStatus.Approved)
                {
                    allGenericPatchEventDto.DateFrom = fetchedEvent.DateFrom;
                    allGenericPatchEventDto.DateTo = fetchedEvent.DateTo;

                    if (await IsElsePlaceReserved(allGenericPatchEventDto))
                        return ServiceResponseBuilder.CreateErrorResponse<Event>("Place date range already reserved.");
                }

                patchedEvent.PlaceReservationStatus = allGenericPatchEventDto.PlaceReservationStatus;
            }

            return ServiceResponseBuilder.CreateSuccessResponse(patchedEvent);
        }

        private async Task<bool> IsPlaceReserved(CreateEventDto createEventDto) =>
            (await GetEventsAtPlace(createEventDto.PlaceId)).Any(e =>
                DateValidator.DateRangesOverlap(createEventDto.DateFrom, createEventDto.DateTo, e.DateFrom, e.DateTo));

        private async Task<bool> IsElsePlaceReserved(AllGenericPatchEventDto allGenericPatchEventDto) =>
            (await GetElseEventsAtPlace(allGenericPatchEventDto)).Any(e =>
                DateValidator.DateRangesOverlap(allGenericPatchEventDto.DateFrom, allGenericPatchEventDto.DateTo,
                    e.DateFrom, e.DateTo));

        private Task<Event?> GetEventById(int id) =>
            _eventRepository.GetAll().FirstOrDefaultAsync(e => e.Id == id);

        private Task<List<Event>> GetEventsAtPlace(int placeId) =>
            _eventRepository.GetAll().Where(e =>
                    e.PlaceId == placeId &&
                    e.PlaceReservationStatus == PlaceReservationStatus.Approved)
                .ToListAsync();

        private Task<List<Event>> GetElseEventsAtPlace(AllGenericPatchEventDto allGenericPatchEventDto) =>
            _eventRepository.GetAll().Where(e =>
                    e.PlaceId == allGenericPatchEventDto.PlaceId &&
                    e.Id != allGenericPatchEventDto.Id &&
                    e.PlaceReservationStatus == PlaceReservationStatus.Approved &&
                    e.DateTo > DateTime.Now)
                .ToListAsync();

        private Task<Place?> GetPlaceById(int id) =>
            _placeRepository.GetAll().FirstOrDefaultAsync(p => p.Id == id);

        private Task<Organizer?> GetOrganizerById(int id) =>
            _organizerRepository.GetAll().FirstOrDefaultAsync(o => o.Id == id);

        private IQueryable<Event> CreateSearchByNameQuery(string name) =>
            _eventRepository.GetAll().Where(e => e.Name.ToLower().Contains(name.ToLower()));
    }
}