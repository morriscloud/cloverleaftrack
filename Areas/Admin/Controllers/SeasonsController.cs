using System;
using System.Linq;
using System.Threading.Tasks;
using CloverleafTrack.Data;
using CloverleafTrack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CloverleafTrack.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SeasonsController : Controller
    {
        private readonly CloverleafTrackDataContext db;

        public SeasonsController(CloverleafTrackDataContext db)
        {
            this.db = db;
        }

        public async Task<IActionResult> Index()
        {
            return View(await db.Seasons.ToListAsync());
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var season = await db.Seasons
                .FirstOrDefaultAsync(s => s.Id == id);
            
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

            season.Id = Guid.NewGuid();
            db.Add(season);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var season = await db.Seasons.FindAsync(id);
            
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
                db.Update(season);
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SeasonExists(season.Id))
                {
                    return NotFound();
                }

                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var season = await db.Seasons
                .FirstOrDefaultAsync(s => s.Id == id);
            
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
            var season = await db.Seasons.FindAsync(id);
            db.Seasons.Remove(season);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SeasonExists(Guid id)
        {
            return db.Seasons.Any(s => s.Id == id);
        }
    }
}
