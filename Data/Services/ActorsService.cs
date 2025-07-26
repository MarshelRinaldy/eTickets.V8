using eTickets.V8.Data.Base;
using eTickets.V8.Models;
using eTickets.V8.Data.Dto;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace eTickets.V8.Data.Services
{
    public class ActorsService : EntityBaseRepository<Actor>, IActorsService
    {
        private readonly AppDbContext _context;
        public ActorsService(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task AddAsync(ActorDTO actorDTO)
        {
            var actor = new Actor
            {
                FullName = actorDTO.FullName,
                ProfilePictureURL = actorDTO.ProfilePictureURL,
                Bio = actorDTO.Bio
            };
            await _context.Actors.AddAsync(actor);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(int id, ActorDTO actorDTO)
        {
            var existingActor = await _context.Actors.FirstOrDefaultAsync(n => n.Id == id);
            if (existingActor != null)
            {
                existingActor.FullName = actorDTO.FullName;
                existingActor.ProfilePictureURL = actorDTO.ProfilePictureURL;
                existingActor.Bio = actorDTO.Bio;
                await _context.SaveChangesAsync();
            }
        }
    }
}
