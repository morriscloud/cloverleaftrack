using CloverleafTrack.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System;
using System.Threading.Tasks;
using CloverleafTrack.Managers;

namespace CloverleafTrack.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SeasonsController : Controller
    {
        private readonly ISeasonManager seasonManager;

        public SeasonsController(ISeasonManager seasonManager)
        {
            this.seasonManager = seasonManager;
        }

        public async Task<IActionResult> Index()
        {
            return View(await seasonManager.GetAsync());
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var season = await seasonManager.GetByIdAsync(id.Value);
            if (season == null)
            {
                return NotFound();
            }

            return View(season);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Season season)
        {
            if (!ModelState.IsValid)
            {
                return View(season);
            }

            await seasonManager.CreateAsync(season);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var season = await seasonManager.GetByIdAsync(id.Value);
            if (season == null)
            {
                return NotFound();
            }

            return View(season);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name")] Season season)
        {
            if (id != season.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(season);
            }

            try
            {
                await seasonManager.EditAsync(id, season.Name);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!seasonManager.CheckExistenceById(season.Id))
                {
                    return NotFound();
                }

                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var season = await seasonManager.GetByIdAsync(id.Value);
            if (season == null)
            {
                return NotFound();
            }

            return View(season);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await seasonManager.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
