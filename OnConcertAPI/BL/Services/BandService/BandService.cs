using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OnConcert.BL.Models;
using OnConcert.BL.Models.Dtos.Band;
using OnConcert.BL.Models.Dtos.Band.Rating;
using OnConcert.DAL.Entities;
using OnConcert.DAL.Generic;

namespace OnConcert.BL.Services.BandService
{
    public class BandService : IBandService
    {
        private readonly IRepository<Band> _bandRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IReadOnlyRepository<BandRating> _bandRatingRepository;
        private readonly IMapper _mapper;

        public BandService(
            IRepository<Band> bandRepository,
            IRepository<User> userRepository,
            IReadOnlyRepository<BandRating> bandRatingRepository,
            IMapper mapper
        )
        {
            _bandRepository = bandRepository;
            _userRepository = userRepository;
            _bandRatingRepository = bandRatingRepository;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<BandResponseDto>>> GetAll()
        {
            var query = from bandRating in _bandRatingRepository.GetAll()
                    .Include(br => br.Rating)
                    .Include(br => br.Band)
                    .ThenInclude(b => b.User)
                group bandRating by bandRating.Band.Id
                into bandRatings
                select new { BandId = bandRatings.Key, BandRatings = bandRatings.ToList() };
            var grouping = await query.ToListAsync();

            var response = grouping.Select(group =>
            {
                var bandResponseDto = _mapper.Map<BandResponseDto>(
                    _mapper.Map<BandDetailsDto>(group.BandRatings.First().Band)
                );
                bandResponseDto.Ratings = _mapper.Map<List<BandRating>, List<GetBandRatingDto>>(group.BandRatings);
                return bandResponseDto;
            }).ToList();

            var bandWithRatingsIds = response.Select(b => b.Id);
            var otherBands = await _bandRepository
                .GetAll()
                .Include(b => b.User)
                .Where(b => !bandWithRatingsIds.Contains(b.Id)).ToListAsync();
            
            response.AddRange(_mapper.Map<List<BandResponseDto>>(
                _mapper.Map<List<BandDetailsDto>>(otherBands)
            ));

            return ServiceResponseBuilder.CreateSuccessResponse(response);
        }

        public async Task<EmptyServiceResponse> Update(BandDetailsDto bandDetails)
        {
            var band = await GetBandById(bandDetails.Id);
            if (band is null)
                return EmptyServiceResponseBuilder.CreateErrorResponse("Band not found.");

            if (!string.IsNullOrWhiteSpace(bandDetails.Email) && await CheckUserExists(bandDetails.Email))
                return EmptyServiceResponseBuilder.CreateErrorResponse(
                    "A user with the given email already exists");

            _bandRepository.Update(_mapper.Map(bandDetails, band));
            _userRepository.Update(_mapper.Map(bandDetails, band.User));
            await _bandRepository.Save();

            return EmptyServiceResponseBuilder.CreateSuccessResponse();
        }

        private Task<bool> CheckUserExists(string email) =>
            _userRepository.GetAll().AnyAsync(u => u.Email.ToLower().Equals(email.ToLower()));

        private Task<Band?> GetBandById(int id) =>
            _bandRepository
                .GetAll()
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == id);
    }
}