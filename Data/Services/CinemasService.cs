using eTickets.V8.Data.Base;
using eTickets.V8.Data.Dto;
using eTickets.V8.Models;

namespace eTickets.V8.Data.Services
{
    public class CinemasService : EntityBaseRepository<Cinema>, ICinemasService
    {
        private readonly AppDbContext _context;
        public CinemasService(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task AddAsync(CinemaDTO cinemaDTO)
        {
            var cinema = new Cinema
            {
                Name = cinemaDTO.Name,
                Logo = cinemaDTO.Logo,
                Description = cinemaDTO.Description,
            };

            await _context.Cinemas.AddAsync(cinema);
            await _context.SaveChangesAsync();
        }
    }
}
