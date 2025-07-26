using eTickets.V8.Data.Base;
using eTickets.V8.Models;
using eTickets.V8.Data.Dto;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace eTickets.V8.Data.Services
{
    public interface IActorsService : IEntityBaseRepository<Actor>
    {
        Task AddAsync(ActorDTO actorDTO);
        Task UpdateAsync(int id, ActorDTO actorDTO);
    }
}
