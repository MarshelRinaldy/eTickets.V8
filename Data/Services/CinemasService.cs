using eTickets.V8.Data.Base;
using eTickets.V8.Models;

namespace eTickets.V8.Data.Services
{
    public class CinemasService : EntityBaseRepository<Cinema>, ICinemasService
    {
        private readonly AppDbContext _context;
        public CinemasService(AppDbContext context) : base(context) { }
    }
}
