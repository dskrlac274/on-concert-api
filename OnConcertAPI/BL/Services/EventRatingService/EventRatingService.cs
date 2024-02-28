using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OnConcert.BL.Extensions;
using OnConcert.BL.Models;
using OnConcert.BL.Models.Dtos.Event.Rating;
using OnConcert.DAL.Entities;
using OnConcert.DAL.Enums;
using OnConcert.DAL.Generic;

namespace OnConcert.BL.Services.EventRatingService
{
    public class EventRatingService : IEventRatingService
    {
        private readonly IRepository<Rating> _ratingRepository;
        private readonly IRepository<EventRating> _eventRatingRepository;
        private readonly IRepository<Event> _eventRepository;
        private readonly IRepository<EventApplication> _eventApplicationRepository;
        private readonly IMapper _mapper;

        public EventRatingService(
            IRepository<Rating> ratingRepository,
            IRepository<EventRating> eventRatingRepository,
            IRepository<Event> eventRepository,
            IRepository<EventApplication> eventApplicationRepository,
            IMapper mapper
        )
        {
            _ratingRepository = ratingRepository;
            _eventRatingRepository = eventRatingRepository;
            _eventRepository = eventRepository;
            _eventApplicationRepository = eventApplicationRepository;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<EventRatingResponseDto>> Create(CreateEventRatingDto createEventRatingDto)
        {
            var response = await CreateRatingValidationResponse(createEventRatingDto);

            if (!response.Success) return response;

            var rating = await CreateRating(createEventRatingDto);
            var eventRating = _mapper.Map<EventRating>(createEventRatingDto);
            eventRating.RatingId = rating.Id;

            _eventRatingRepository.Add(eventRating);
            await _eventRatingRepository.Save();

            var eventRatingResponseDto = _mapper.Map<EventRatingResponseDto>(eventRating);
            eventRatingResponseDto.Review = rating.Review;
            eventRatingResponseDto.Score = rating.Score;

            return ServiceResponseBuilder.CreateSuccessResponse(eventRatingResponseDto);
        }

        public async Task<EmptyServiceResponse> Delete(DeleteEventRatingDto deleteEventRatingDto)
        {
            if (await GetEventById(deleteEventRatingDto.EventId) == null)
                return EmptyServiceResponseBuilder.CreateErrorResponse("Event not found.");

            var fetchedEventRating = await GetEventRatingById(deleteEventRatingDto.Id);
            if (fetchedEventRating == null)
                return EmptyServiceResponseBuilder.CreateErrorResponse("Event rating not found.");

            if (fetchedEventRating.BandId != deleteEventRatingDto.BandId)
                return EmptyServiceResponseBuilder.CreateErrorResponse("Unauthorized to delete this event rating.");

            var fetchedRating = await GetRatingById(fetchedEventRating.RatingId);
            if (fetchedRating == null)
                return EmptyServiceResponseBuilder.CreateErrorResponse("Rating not found.");

            _ratingRepository.Delete(fetchedRating);
            await _ratingRepository.Save();

            return EmptyServiceResponseBuilder.CreateSuccessResponse();
        }

        private async Task<Rating> CreateRating(CreateEventRatingDto createEventRatingDto)
        {
            var rating = _mapper.Map<Rating>(createEventRatingDto);

            _ratingRepository.Add(rating);
            await _ratingRepository.Save();

            return rating;
        }

        private async Task<ServiceResponse<EventRatingResponseDto>> CreateRatingValidationResponse(CreateEventRatingDto createEventRatingDto)
        {
            var fetchedEvent = await GetEventById(createEventRatingDto.EventId);
            if (fetchedEvent == null)
                return ServiceResponseBuilder.CreateErrorResponse<EventRatingResponseDto>("Event not found.");

            if (!fetchedEvent.HasFinished())
                return ServiceResponseBuilder.CreateErrorResponse<EventRatingResponseDto>("Given event is not finished.");

            if (!await CheckBandCanVote(createEventRatingDto))
                return ServiceResponseBuilder.CreateErrorResponse<EventRatingResponseDto>("Unauthorized to add rating.");

            if (await CheckBandRatingExist(createEventRatingDto))
                return ServiceResponseBuilder.CreateErrorResponse<EventRatingResponseDto>("Rating already exists.");

            return ServiceResponseBuilder.CreateSuccessResponse(new EventRatingResponseDto());
        }

        private Task<Event?> GetEventById(int eventId) => _eventRepository.GetAll().FirstOrDefaultAsync(e => e.Id == eventId);

        private Task<EventRating?> GetEventRatingById(int eventRatingId) =>
            _eventRatingRepository.GetAll().FirstOrDefaultAsync(e => e.RatingId == eventRatingId);

        private Task<Rating?> GetRatingById(int ratingId) => _ratingRepository.GetAll().FirstOrDefaultAsync(e => e.Id == ratingId);

        private Task<bool> CheckBandCanVote(CreateEventRatingDto createEventRatingDto) =>
            _eventApplicationRepository.GetAll().AnyAsync(e =>
                e.BandId == createEventRatingDto.BandId &&
                e.EventId == createEventRatingDto.EventId &&
                e.BandApplicationStatus == BandApplicationStatus.Approved);

        private Task<bool> CheckBandRatingExist(CreateEventRatingDto createEventRatingDto) =>
            _eventRatingRepository.GetAll().AnyAsync(e =>
                e.BandId == createEventRatingDto.BandId && e.EventId == createEventRatingDto.EventId);
    }
}