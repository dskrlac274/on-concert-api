using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OnConcert.BL.Models;
using OnConcert.BL.Models.Dtos.Place;
using OnConcert.DAL.Entities;
using OnConcert.DAL.Generic;
using OnConcert.BL.Models.Dtos.Place.Rating;

namespace OnConcert.BL.Services.PlaceService
{
    public class PlaceService : IPlaceService
    {
        private readonly IRepository<Place> _placeRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<PlaceRating> _placeRatingRepository;
        private readonly IRepository<Rating> _ratingRepository;
        private readonly IMapper _mapper;

        public PlaceService(
            IRepository<Place> placeRepository,
            IRepository<User> userRepository,
            IRepository<PlaceRating> placeRatingRepository,
            IRepository<Rating> ratingRepository,
            IMapper mapper
        )
        {
            _placeRepository = placeRepository;
            _userRepository = userRepository;
            _placeRatingRepository = placeRatingRepository;
            _ratingRepository = ratingRepository;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<PlaceResponseDto>>> GetAll() =>
            ServiceResponseBuilder.CreateSuccessResponse(await GetPlacesWithRatings());

        public async Task<EmptyServiceResponse> Update(PlaceDetailsDto placeDetails)
        {
            var place = await GetPlaceById(placeDetails.Id);
            if (place is null)
                return EmptyServiceResponseBuilder.CreateErrorResponse("Place not found.");
            
            if (!string.IsNullOrWhiteSpace(placeDetails.Email) && await CheckUserExists(placeDetails.Email))
                return EmptyServiceResponseBuilder.CreateErrorResponse(
                    "A user with the given email already exists");

            _placeRepository.Update(_mapper.Map(placeDetails, place));
            _userRepository.Update(_mapper.Map(placeDetails, place.User));
            await _placeRepository.Save();

            return EmptyServiceResponseBuilder.CreateSuccessResponse();
        }

        private async Task<List<PlaceResponseDto>> GetPlacesWithRatings()
        {
            var placesWithRatings = await
                (from p in _placeRepository.GetAll().Include(p => p.User)
                 join er in _placeRatingRepository.GetAll() on p.Id equals er.PlaceId into placeRatings
                 from subEventRating in placeRatings.DefaultIfEmpty()
                 join r in _ratingRepository.GetAll() on subEventRating.RatingId equals r.Id into ratings
                 from subRating in ratings.DefaultIfEmpty()
                 select new { p, subEventRating, subRating })
                .ToListAsync();

            return placesWithRatings.GroupBy(x => x.p.Id).Select(group =>
            {
                var placeDto = _mapper.Map<PlaceResponseDto>(group.First().p);
                placeDto.Ratings = group
                    .Where(x => x.subRating != null)
                    .Select(x =>
                    {
                        var getPlaceRatingDto = _mapper.Map<GetPlaceRatingDto>(x.subEventRating);
                        getPlaceRatingDto.Score = x.subRating.Score;
                        getPlaceRatingDto.Review = x.subRating.Review;
                        return getPlaceRatingDto;
                    })
                    .ToList();
                return placeDto;
            }).ToList();
        }

        private Task<bool> CheckUserExists(string email) =>
            _userRepository.GetAll().AnyAsync(u => u.Email.ToLower().Equals(email.ToLower()));

        private Task<Place?> GetPlaceById(int id) =>
            _placeRepository
                .GetAll()
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);
    }
}