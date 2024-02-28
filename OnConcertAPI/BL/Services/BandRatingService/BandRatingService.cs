using AutoMapper;
using OnConcert.BL.Models;
using OnConcert.DAL.Entities;
using OnConcert.DAL.Generic;
using Microsoft.EntityFrameworkCore;
using OnConcert.BL.Models.Dtos.Band.Rating;

namespace OnConcert.BL.Services.BandRatingService
{
    public class BandRatingService : IBandRatingService
    {
        private readonly IRepository<Rating> _ratingRepository;
        private readonly IRepository<BandRating> _bandRatingRepository;
        private readonly IRepository<Band> _bandRepository;
        private readonly IMapper _mapper;

        public BandRatingService(
            IRepository<Rating> ratingRepository,
            IRepository<BandRating> bandRatingRepository,
            IRepository<Band> bandRepository,
            IMapper mapper
        )
        {
            _ratingRepository = ratingRepository;
            _bandRatingRepository = bandRatingRepository;
            _bandRepository = bandRepository;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<BandRatingResponseDto>> Create(CreateBandRatingDto createBandRatingDto)
        {
            var response = await CreateRatingValidationResponse(createBandRatingDto);

            if (!response.Success)
                return ServiceResponseBuilder.CreateErrorResponse<BandRatingResponseDto>(response.Message);

            var rating = await CreateRating(createBandRatingDto);

            var bandRating = _mapper.Map<BandRating>(createBandRatingDto);
            bandRating.RatingId = rating.Id;

            _bandRatingRepository.Add(bandRating);
            await _bandRatingRepository.Save();

            var bandRatingResponseDto = _mapper.Map<BandRatingResponseDto>(bandRating);

            bandRatingResponseDto.Review = rating.Review;
            bandRatingResponseDto.Score = rating.Score;

            return ServiceResponseBuilder.CreateSuccessResponse(bandRatingResponseDto);
        }

        public async Task<EmptyServiceResponse> Delete(DeleteBandRatingDto deleteBandRatingDto)
        {
            var bandRating = await GetBandRatingById(deleteBandRatingDto.Id);
            if (bandRating is null || bandRating.BandId != deleteBandRatingDto.BandId)
                return EmptyServiceResponseBuilder.CreateErrorResponse("Band rating not found.");

            if (deleteBandRatingDto.OrganizerId is not null && bandRating.OrganizerId != deleteBandRatingDto.OrganizerId
                || deleteBandRatingDto.VisitorId is not null && bandRating.VisitorId != deleteBandRatingDto.VisitorId)
                return EmptyServiceResponseBuilder.CreateErrorResponse("Unauthorized to delete this band rating.");

            _ratingRepository.Delete(bandRating.Rating);
            await _ratingRepository.Save();

            return EmptyServiceResponseBuilder.CreateSuccessResponse();
        }

        private async Task<EmptyServiceResponse> CreateRatingValidationResponse(CreateBandRatingDto createBandRatingDto)
        {
            var fetchedBand = await GetBandById(createBandRatingDto.BandId);

            if (fetchedBand is null)
                return EmptyServiceResponseBuilder.CreateErrorResponse("Band not found.");

            if (await CheckRatingExists(createBandRatingDto))
                return EmptyServiceResponseBuilder.CreateErrorResponse("Rating already exists.");

            return EmptyServiceResponseBuilder.CreateSuccessResponse();
        }

        private Task<Band?> GetBandById(int id) => _bandRepository.GetAll().FirstOrDefaultAsync(b => b.Id == id);

        private async Task<bool> CheckRatingExists(CreateBandRatingDto createBandRatingDto) =>
            await CheckOrganizerRatingExist(createBandRatingDto) || await CheckVisitorRatingExist(createBandRatingDto);

        private Task<bool> CheckOrganizerRatingExist(CreateBandRatingDto createBandRatingDto)
        {
            if (createBandRatingDto.OrganizerId is null) return Task.FromResult(false);
            return _bandRatingRepository.GetAll().AnyAsync(br =>
                br.OrganizerId == createBandRatingDto.OrganizerId &&
                br.BandId == createBandRatingDto.BandId
            );
        }

        private Task<bool> CheckVisitorRatingExist(CreateBandRatingDto createBandRatingDto)
        {
            if (createBandRatingDto.VisitorId is null) return Task.FromResult(false);
            return _bandRatingRepository.GetAll().AnyAsync(br =>
                br.VisitorId == createBandRatingDto.VisitorId &&
                br.BandId == createBandRatingDto.BandId
            );
        }

        private async Task<Rating> CreateRating(CreateBandRatingDto createBandRatingDto)
        {
            var rating = _mapper.Map<Rating>(createBandRatingDto);

            _ratingRepository.Add(rating);
            await _ratingRepository.Save();

            return rating;
        }

        private Task<BandRating?> GetBandRatingById(int id) =>
            _bandRatingRepository
                .GetAll()
                .Include(br => br.Rating)
                .FirstOrDefaultAsync(br => br.RatingId == id);
    }
}