using eTickets.V8.Models;
using Microsoft.AspNetCore.Mvc;
using eTickets.V8.Data.Services;
using System.Diagnostics;
using eTickets.V8.Data.Dto;
using eTickets.V8.Data.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using System;

namespace eTickets.V8.Controllers
{
    [Authorize]
    public class ActorsController : Controller
    {
        private readonly IActorsService _service;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ActorsController(IActorsService service, IWebHostEnvironment webHostEnvironment)
        {
            _service = service;
            _webHostEnvironment = webHostEnvironment;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var data = await _service.GetAllAsync();
            return View(data);
        }

        // GET: Actors/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Actors/Create
        //Ini pengkodean yang baik menggunakan VM dan DTO
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ActorVM actorVM)
        {
            if (!ModelState.IsValid)
            {
                return View(actorVM);
            }

            if (actorVM.ProfilePictureURL == null || actorVM.ProfilePictureURL.Length == 0)
            {
                ModelState.AddModelError("ProfilePictureURL", "The image file is required");
                return View(actorVM);
            }

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(actorVM.ProfilePictureURL.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
            {
                ModelState.AddModelError("ProfilePictureURL", "Only image files (jpg, jpeg, png, gif) are allowed");
                return View(actorVM);
            }

            // Generate unique filename
            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + fileExtension;

            // Create directory if it doesn't exist
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "actors");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Save file to local storage
            string imageFullPath = Path.Combine(uploadsFolder, newFileName);
            using (var stream = new FileStream(imageFullPath, FileMode.Create))
            {
                await actorVM.ProfilePictureURL.CopyToAsync(stream);
            }

            var actor = new ActorDTO
            {
                FullName = actorVM.FullName,
                ProfilePictureURL = "/images/actors/" + newFileName, // Store relative path for web access
                Bio = actorVM.Bio
            };

            await _service.AddAsync(actor);
            return RedirectToAction(nameof(Index));
        }

        [AllowAnonymous]
        public async Task<IActionResult> Detail(int id)
        {
            var actorDetails = await _service.GetByIdAsync(id);
            return View(actorDetails);
        }

        // GET: Actors/Edit/1
        public async Task<IActionResult> Edit(int id)
        {
            var actorDetails = await _service.GetByIdAsync(id);

            var editVM = new ActorEditVM
            {
                Id = actorDetails.Id,
                FullName = actorDetails.FullName,
                CurrentProfilePictureURL = actorDetails.ProfilePictureURL,
                Bio = actorDetails.Bio
            };

            return View(editVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ActorEditVM editVM)
        {
            if (!ModelState.IsValid)
            {
                return View(editVM);
            }

            string profilePictureURL = editVM.CurrentProfilePictureURL;

            // If a new file is uploaded, process it
            if (editVM.NewProfilePictureURL != null && editVM.NewProfilePictureURL.Length > 0)
            {
                // Validate file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(editVM.NewProfilePictureURL.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("NewProfilePictureURL", "Only image files (jpg, jpeg, png, gif) are allowed");
                    return View(editVM);
                }

                // Generate unique filename
                string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + fileExtension;

                // Create directory if it doesn't exist
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "actors");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Save new file to local storage
                string imageFullPath = Path.Combine(uploadsFolder, newFileName);
                using (var stream = new FileStream(imageFullPath, FileMode.Create))
                {
                    await editVM.NewProfilePictureURL.CopyToAsync(stream);
                }

                // Delete old file if it exists and is in our uploads folder
                if (!string.IsNullOrEmpty(editVM.CurrentProfilePictureURL) &&
                    editVM.CurrentProfilePictureURL.StartsWith("/images/actors/"))
                {
                    string oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath,
                        editVM.CurrentProfilePictureURL.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                profilePictureURL = "/images/actors/" + newFileName;
            }

            var actorDTO = new ActorDTO
            {
                FullName = editVM.FullName,
                ProfilePictureURL = profilePictureURL,
                Bio = editVM.Bio
            };

            await _service.UpdateAsync(id, actorDTO);
            return RedirectToAction(nameof(Index));
        }

        // GET: Actors/Delete/1
        public async Task<IActionResult> Delete(int id)
        {
            var actorDetails = await _service.GetByIdAsync(id);

            return View(actorDetails);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //written down here just for infomartion of ID, i use that for know the Id in parameter.
            Console.WriteLine($"ID yang akan dihapus: {id}");
            Debugger.Break();
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

    }
}
