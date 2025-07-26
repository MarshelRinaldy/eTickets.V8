using eTickets.V8.Data;
using eTickets.V8.Data.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eTickets.V8.Controllers
{
    [Authorize]
    public class CinemasController : Controller
    {
        private readonly ICinemasService _service;

        public CinemasController(ICinemasService service)
        {
            _service = service;
        }
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var AllCinemas = await _service.GetAllAsync();
            return View(AllCinemas);
        }
        public IActionResult Create()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> Detail(int id)
        {
            var cinemaDetail = await _service.GetByIdAsync(id);
            return View(cinemaDetail);
        }

    }
}
