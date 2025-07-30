using eTickets.V8.Data;
using eTickets.V8.Data.Dto;
using eTickets.V8.Data.Services;
using eTickets.V8.Data.ViewModels;
using eTickets.V8.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eTickets.V8.Controllers
{
    [Authorize]
    public class CinemasController : Controller
    {
        private readonly ICinemasService _service;
        private readonly ICinemasService _cinemasService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CinemasController(ICinemasService service, ICinemasService cinemasService, IWebHostEnvironment webHostEnvironment)
        {
            _service = service;
            _cinemasService = cinemasService;
            _webHostEnvironment = webHostEnvironment;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CinemaVM cinemaVM)
        {
            if (!ModelState.IsValid)
            {
                return View(cinemaVM);
            }

            // Check if Logo file is provided
            if (cinemaVM.Logo == null || cinemaVM.Logo.Length == 0)
            {
                ModelState.AddModelError("Logo", "Please select a logo image");
                return View(cinemaVM);
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(cinemaVM.Logo.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
            {
                ModelState.AddModelError("Logo", "Only image files (jpg, jpeg, png, gif) are allowed");
                return View(cinemaVM);
            }

            //You can setting unique name
            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + fileExtension;

            // Create directory if it doesn't exist
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "cinemas");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Save file to local storage
            string imageFullPath = Path.Combine(uploadsFolder, newFileName);
            using (var stream = new FileStream(imageFullPath, FileMode.Create))
            {
                await cinemaVM.Logo.CopyToAsync(stream);
            }

            var cinema = new CinemaDTO
            {
                Name = cinemaVM.Name,
                Logo = "/images/cinemas/" + newFileName,
                Description = cinemaVM.Description,
            };

            await _cinemasService.AddAsync(cinema);
            return RedirectToAction(nameof(Index));
        }

        [AllowAnonymous]
        public async Task<IActionResult> Detail(int id)
        {
            var cinemaDetail = await _service.GetByIdAsync(id);
            return View(cinemaDetail);
        }

    }
}
