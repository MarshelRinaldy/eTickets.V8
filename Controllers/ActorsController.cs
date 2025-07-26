using eTickets.V8.Models;
using Microsoft.AspNetCore.Mvc;
using eTickets.V8.Data.Services;
using System.Diagnostics;
using eTickets.V8.Data.Dto;
using eTickets.V8.Data.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace eTickets.V8.Controllers
{
    [Authorize]
    public class ActorsController : Controller
    {
        private readonly IActorsService _service;

        public ActorsController(IActorsService service)
        {
            _service = service;
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

            var actor = new ActorDTO
            {
                FullName = actorVM.FullName,
                ProfilePictureURL = actorVM.ProfilePictureURL,
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

            return View(actorDetails);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,ProfilePictureURL,Bio")] Actor actor)
        {

            await _service.UpdateAsync(id, actor);
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
