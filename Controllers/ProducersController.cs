using eTickets.V8.Data;
using eTickets.V8.Data.Services;
using eTickets.V8.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace eTickets.V8.Controllers
{
    [Authorize]
    public class ProducersController : Controller
    {

        private readonly IProducersService _service;

        public ProducersController(IProducersService service)
        {
            _service = service;
        }


        // public async Task<IActionResult> Index()
        // {
        //     var allProducer = await _context.Producers.ToListAsync();
        //     return View(allProducer);
        // }
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var data = await _service.GetAllAsync();
            return View(data);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FullName,ProfilePictureURL,Bio")] Producer producer)
        {
            await _service.AddAsync(producer);
            return RedirectToAction(nameof(Index));
        }
        [AllowAnonymous]
        public async Task<IActionResult> Detail(int id)
        {
            var producerDetails = await _service.GetByIdAsync(id);
            if (producerDetails == null)
            {
                return View("NotFound");
            }

            //merupakan cara lain untuk menampilkan halaman lain
            return View("~/Views/Producers/Detail.cshtml", producerDetails);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var producersDetails = await _service.GetByIdAsync(id);
            Debugger.Break();
            return View("~/Views/Producers/Edit.cshtml", producersDetails);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,ProfilePictureURL,Bio")] Producer producer)
        {
            await _service.UpdateAsync(id, producer);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            Debugger.Break();
            var producerDetails = await _service.GetByIdAsync(id);
            return View(producerDetails);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Console.WriteLine($"ID yang akan dihapus: {id}");
            //Debugger.Break();
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

    }
}
