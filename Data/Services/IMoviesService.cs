using eTickets.V8.Data.Base;
using eTickets.V8.Data.ViewModels;
using eTickets.V8.Models;

namespace eTickets.V8.Data.Services
{
    public interface IMoviesService : IEntityBaseRepository<Movie>
    {
        Task<IEnumerable<Movie>> GetAllAsyncMovieAndCinema();
        Task<Movie> GetMovieByIdAsync(int id);
        Task<NewMoviewDropdownsVM> GetNewMovieDropdownsValues();
        Task AddNewMovieAsync(NewMovieVM movie);
        Task UpdateMovieAsync(NewMovieVM movie);
    }
}
