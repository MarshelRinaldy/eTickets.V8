using eTickets.V8.Data.Base;
using eTickets.V8.Data.Dto;
using eTickets.V8.Models;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace eTickets.V8.Data.Services
{
    public interface ICinemasService : IEntityBaseRepository<Cinema>
    {
        Task AddAsync(CinemaDTO cinemaDTO);
    }
}
