using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using eTickets.V8.Data.Base;
using eTickets.V8.Data.ViewModels;
using eTickets.V8.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace eTickets.V8.Data.Services
{
    public class MoviesService : EntityBaseRepository<Movie>, IMoviesService
    {
        private readonly AppDbContext _context;
        public MoviesService(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task AddNewMovieAsync(NewMovieVM movieVM)
        {

            var newMovie = new Movie()
            {
                Name = movieVM.Name,
                Description = movieVM.Description,
                Price = movieVM.Price,
                ImageURL = movieVM.ImageURL,
                CinemaId = movieVM.CinemaId,
                StartDate = DateTime.SpecifyKind(movieVM.StartDate, DateTimeKind.Utc),
                EndDate = DateTime.SpecifyKind(movieVM.EndDate, DateTimeKind.Utc),
                MovieCategory = movieVM.MovieCategory,
                ProducerId = movieVM.ProducerId,
            };

            await _context.Movies.AddAsync(newMovie);
            await _context.SaveChangesAsync();



            // Add Movie Actors
            if (movieVM.ActorIds != null)
            {
                foreach (var actorId in movieVM.ActorIds)
                {
                    var newActorMovie = new Actor_Movie()
                    {
                        MovieId = newMovie.Id,
                        ActorId = actorId,
                    };
                    await _context.Actors_Movies.AddAsync(newActorMovie);
                }
            }


            await _context.SaveChangesAsync();
        }



        public async Task<IEnumerable<Movie>> GetAllAsyncMovieAndCinema()
        {
            return await _context.Movies.Include(m => m.Cinema).ToListAsync();
        }

        public async Task<Movie> GetMovieByIdAsync(int id)
        {
            var movieDetails = _context.Movies.Include(c => c.Cinema).Include(p => p.Producer).Include(am => am.Actors_Movies).ThenInclude(a => a.Actor).FirstOrDefaultAsync(n => n.Id == id);

            return await movieDetails;
        }

        public async Task<NewMoviewDropdownsVM> GetNewMovieDropdownsValues()
        {
            var response = new NewMoviewDropdownsVM();
            response.Actors = await _context.Actors.OrderBy(n => n.FullName).ToListAsync();
            response.Cinemas = await _context.Cinemas.OrderBy(n => n.Name).ToListAsync();
            response.Producers = await _context.Producers.OrderBy(n => n.FullName).ToListAsync();

            return response;
        }

        public async Task UpdateMovieAsync(NewMovieVM movieVM)
        {

            var dbMovie = await _context.Movies.FirstOrDefaultAsync(n => n.Id == movieVM.Id);


            dbMovie.Name = movieVM.Name;
            dbMovie.Description = movieVM.Description;
            dbMovie.Price = movieVM.Price;
            dbMovie.ImageURL = movieVM.ImageURL;
            dbMovie.CinemaId = movieVM.CinemaId;
            dbMovie.StartDate = DateTime.SpecifyKind(movieVM.StartDate, DateTimeKind.Utc);
            dbMovie.EndDate = DateTime.SpecifyKind(movieVM.EndDate, DateTimeKind.Utc);
            dbMovie.MovieCategory = movieVM.MovieCategory;
            dbMovie.ProducerId = movieVM.ProducerId;
            await _context.SaveChangesAsync();


            //Remove existing Actors_Movie
            var existingActorsDb = _context.Actors_Movies.Where(n => n.MovieId == movieVM.Id).ToList();
            _context.Actors_Movies.RemoveRange(existingActorsDb);
            await _context.SaveChangesAsync();


            // Add Movie Actors
            if (movieVM.ActorIds != null)
            {
                foreach (var actorId in movieVM.ActorIds)
                {
                    var newActorMovie = new Actor_Movie()
                    {
                        MovieId = movieVM.Id,
                        ActorId = actorId,
                    };
                    await _context.Actors_Movies.AddAsync(newActorMovie);
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}
