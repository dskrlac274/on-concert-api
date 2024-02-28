using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OnConcert.BL.Models;
using OnConcert.BL.Models.Dtos.Organizer;
using OnConcert.DAL.Entities;
using OnConcert.DAL.Generic;

namespace OnConcert.BL.Services.OrganizerService
{
    public class OrganizerService : IOrganizerService
    {
        private readonly IRepository<Organizer> _organizerRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IMapper _mapper;

        public OrganizerService(
            IRepository<Organizer> organizerRepository,
            IRepository<User> userRepository,
            IMapper mapper
        )
        {
            _organizerRepository = organizerRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<OrganizerDetailsDto>> GetById(int id)
        {
            var organizer = _mapper.Map<OrganizerDetailsDto?>(
                await _organizerRepository.GetAll().Include(o => o.User)
                    .FirstOrDefaultAsync(o => o.Id == id)
            );

            return organizer is not null
                ? ServiceResponseBuilder.CreateSuccessResponse(organizer)
                : ServiceResponseBuilder.CreateErrorResponse<OrganizerDetailsDto>("Organizer not found.");
        }

        public async Task<EmptyServiceResponse> Update(OrganizerDetailsDto organizerDetails)
        {
            var organizer = await GetOrganizerById(organizerDetails.Id);
            if (organizer is null)
                return EmptyServiceResponseBuilder.CreateErrorResponse("Organizer not found.");

            if (!string.IsNullOrWhiteSpace(organizerDetails.Email) && await CheckUserExists(organizerDetails.Email))
                return EmptyServiceResponseBuilder.CreateErrorResponse(
                    "A user with the given email already exists");

            if (!string.IsNullOrWhiteSpace(organizerDetails.Oib) && await CheckOibExists(organizerDetails.Oib))
                return EmptyServiceResponseBuilder.CreateErrorResponse(
                    "An organizer with the given OIB is already registered.");

            _organizerRepository.Update(_mapper.Map(organizerDetails, organizer));
            _userRepository.Update(_mapper.Map(organizerDetails, organizer.User));
            await _organizerRepository.Save();

            return EmptyServiceResponseBuilder.CreateSuccessResponse();
        }

        private Task<bool> CheckOibExists(string oib) =>
            _organizerRepository.GetAll().AnyAsync(o => o.Oib.ToLower().Equals(oib.ToLower()));

        private Task<bool> CheckUserExists(string email) =>
            _userRepository.GetAll().AnyAsync(u => u.Email.ToLower().Equals(email.ToLower()));

        private Task<Organizer?> GetOrganizerById(int id) =>
            _organizerRepository
                .GetAll()
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id);
    }
}