using eTickets.V8.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using eTickets.V8.Data.Services;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Rendering;
using eTickets.V8.Models;
using eTickets.V8.Data.ViewModels;
using eTickets.V8.Data.Enums;
using System.IO.Pipelines;
using Microsoft.AspNetCore.Authorization;

namespace eTickets.V8.Controllers
{
    [Authorize]
    public class MoviesController : Controller
    {
        private readonly IMoviesService _service;

        public MoviesController(IMoviesService service)
        {
            _service = service;
        }
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var allMovie = await _service.GetAllAsyncMovieAndCinema();
            return View(allMovie);
        }
        [AllowAnonymous]
        public async Task<IActionResult> Filter(string searchString)
        {
            var allMovie = await _service.GetAllAsyncMovieAndCinema();

            if (!string.IsNullOrEmpty(searchString))
            {
                var filteredResult = allMovie.Where(n => n.Name == searchString || n.Description.Contains(searchString)).ToList();

                return View("~/Views/Movies/Index.cshtml", filteredResult);
            }

            return View("~/Views/Movies/Index.cshtml", allMovie);
        }
        [AllowAnonymous]
        public async Task<IActionResult> Detail(int id)
        {
            var movieDetail = await _service.GetMovieByIdAsync(id);
            return View(movieDetail);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var movieDropdownsData = await _service.GetNewMovieDropdownsValues();
            ViewBag.CinemaId = new SelectList(movieDropdownsData.Cinemas, "Id", "Name");
            ViewBag.ProducerId = new SelectList(movieDropdownsData.Producers, "Id", "FullName");
            ViewBag.ActorId = new SelectList(movieDropdownsData.Actors, "Id", "FullName");
            ViewBag.MovieCategory = new SelectList(Enum.GetValues(typeof(eTickets.V8.Data.Enums.MovieCategory)));
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(NewMovieVM movie)
        {
            // Cek nilai CinemaId
            Console.WriteLine("CinemaId: " + movie.CinemaId);
            await _service.AddNewMovieAsync(movie);
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int id)
        {
            var movieDetails = await _service.GetMovieByIdAsync(id);

            var response = new NewMovieVM()
            {
                Id = id,
                Name = movieDetails.Name,
                Description = movieDetails.Description,
                Price = movieDetails.Price,
                ImageURL = movieDetails.ImageURL,
                CinemaId = movieDetails.CinemaId,
                StartDate = movieDetails.StartDate,
                EndDate = movieDetails.EndDate,
                MovieCategory = movieDetails.MovieCategory,
                ProducerId = movieDetails.ProducerId,
                ActorIds = movieDetails.Actors_Movies.Select(n => n.ActorId).ToList(),
            };

            var movieDropdownsData = await _service.GetNewMovieDropdownsValues();
            ViewBag.CinemaId = new SelectList(movieDropdownsData.Cinemas, "Id", "Name");
            ViewBag.ProducerId = new SelectList(movieDropdownsData.Producers, "Id", "FullName");
            ViewBag.ActorId = new SelectList(movieDropdownsData.Actors, "Id", "FullName");
            ViewBag.MovieCategory = new SelectList(Enum.GetValues(typeof(eTickets.V8.Data.Enums.MovieCategory)));
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, NewMovieVM movie)
        {

            await _service.UpdateMovieAsync(movie);
            return RedirectToAction(nameof(Index));
        }



    }
}

//public string Name { get; set; }
//public string Description { get; set; }
//public double Price { get; set; }
//public string ImageURL { get; set; }
//public int CinemaId { get; set; }
//public DateTime StartDate { get; set; }
//public DateTime EndDate { get; set; }
//public MovieCategory MovieCategory { get; set; }
//public int ProducerId { get; set; }
//public List<int> ActorIds { get; set; } = new List<int>();