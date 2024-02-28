using AutoMapper;
using OnConcert.BL.Models;
using OnConcert.DAL.Entities;
using OnConcert.DAL.Generic;
using Microsoft.EntityFrameworkCore;
using OnConcert.BL.Models.Dtos.Place.Rating;

namespace OnConcert.BL.Services.PlaceRatingService
{
    public class PlaceRatingService : IPlaceRatingService
    {
        private readonly IRepository<Rating> _ratingRepository;
        private readonly IRepository<PlaceRating> _placeRatingRepository;
        private readonly IRepository<Place> _placeRepository;
        private readonly IMapper _mapper;

        public PlaceRatingService(
            IRepository<Rating> ratingRepository,
            IRepository<PlaceRating> placeRatingRepository,
            IRepository<Place> placeRepository,
            IMapper mapper
        )
        {
            _ratingRepository = ratingRepository;
            _placeRatingRepository = placeRatingRepository;
            _placeRepository = placeRepository;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<PlaceRatingResponseDto>> Create(CreatePlaceRatingDto createPlaceRatingDto)
        {
            var response = await CreateRatingValidationResponse(createPlaceRatingDto);

            if (!response.Success) return response;

            var rating = await CreateRating(createPlaceRatingDto);
            var placeRating = _mapper.Map<PlaceRating>(createPlaceRatingDto);
            placeRating.RatingId = rating.Id;

            _placeRatingRepository.Add(placeRating);
            await _placeRatingRepository.Save();

            var placeRatingResponseDto = _mapper.Map<PlaceRatingResponseDto>(placeRating);
            placeRatingResponseDto.Review = rating.Review;
            placeRatingResponseDto.Score = rating.Score;

            return ServiceResponseBuilder.CreateSuccessResponse(placeRatingResponseDto);
        }

        public async Task<EmptyServiceResponse> Delete(DeletePlaceRatingDto deletePlaceRatingDto)
        {
            if (await GetPlaceById(deletePlaceRatingDto.PlaceId) == null)
                return EmptyServiceResponseBuilder.CreateErrorResponse("Place not found.");

            var fetchedPlaceRating = await GetPlaceRatingById(deletePlaceRatingDto.Id);
            if (fetchedPlaceRating == null)
                return EmptyServiceResponseBuilder.CreateErrorResponse("Place rating not found.");

            if (fetchedPlaceRating.BandId != deletePlaceRatingDto.BandId && fetchedPlaceRating.BandId is not null
                || fetchedPlaceRating.VisitorId != deletePlaceRatingDto.VisitorId && fetchedPlaceRating.VisitorId is not null)
                return EmptyServiceResponseBuilder.CreateErrorResponse("Unauthorized to delete this place rating.");

            var fetchedRating = await GetRatingById(fetchedPlaceRating.RatingId);
            if (fetchedRating == null)
                return EmptyServiceResponseBuilder.CreateErrorResponse("Rating not found.");

            _ratingRepository.Delete(fetchedRating);
            await _ratingRepository.Save();

            return EmptyServiceResponseBuilder.CreateSuccessResponse();
        }

        private async Task<Rating> CreateRating(CreatePlaceRatingDto createPlaceRatingDto)
        {
            var rating = _mapper.Map<Rating>(createPlaceRatingDto);

            _ratingRepository.Add(rating);
            await _ratingRepository.Save();

            return rating;
        }

        private async Task<ServiceResponse<PlaceRatingResponseDto>> CreateRatingValidationResponse(CreatePlaceRatingDto createPlaceRatingDto)
        {
            var fetchedPlace = await GetPlaceById(createPlaceRatingDto.PlaceId);

            if (fetchedPlace == null)
                return ServiceResponseBuilder.CreateErrorResponse<PlaceRatingResponseDto>("Place not found.");

            if (await CheckBandRatingExist(createPlaceRatingDto) || await CheckVisitorRatingExist(createPlaceRatingDto))  
                return ServiceResponseBuilder.CreateErrorResponse<PlaceRatingResponseDto>("Rating already exists.");            

            return ServiceResponseBuilder.CreateSuccessResponse(new PlaceRatingResponseDto());
        }

        private Task<Place?> GetPlaceById(int placeId) => _placeRepository.GetAll().FirstOrDefaultAsync(p => p.Id == placeId);

        private Task<PlaceRating?> GetPlaceRatingById(int placeRatingId) =>
            _placeRatingRepository.GetAll().FirstOrDefaultAsync(p => p.RatingId == placeRatingId);

        private Task<Rating?> GetRatingById(int ratingId) => _ratingRepository.GetAll().FirstOrDefaultAsync(r => r.Id == ratingId);

        private Task<bool> CheckBandRatingExist(CreatePlaceRatingDto createPlaceRatingDto) =>
            _placeRatingRepository.GetAll().AnyAsync(p =>
                p.BandId == createPlaceRatingDto.BandId && p.PlaceId == createPlaceRatingDto.PlaceId &&
            createPlaceRatingDto.BandId != null);

        private Task<bool> CheckVisitorRatingExist(CreatePlaceRatingDto createPlaceRatingDto) =>
            _placeRatingRepository.GetAll().AnyAsync(p =>
                p.VisitorId == createPlaceRatingDto.VisitorId && p.PlaceId == createPlaceRatingDto.PlaceId &&
            createPlaceRatingDto.VisitorId != null);
    }
}