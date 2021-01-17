using System;
using System.Linq;
using System.Threading.Tasks;

using CloverleafTrack.Data;
using CloverleafTrack.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CloverleafTrack.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MeetsController : Controller
    {
        private readonly CloverleafTrackDataContext db;

        public MeetsController(CloverleafTrackDataContext db)
        {
            this.db = db;
        }

        public async Task<IActionResult> Index()
        {
            var cloverleafTrackDataContext = db.Meets
                .Include(m => m.Season)
                .Include(m => m.MeetResult)
                .OrderByDescending(m => m.Date);

            return View(await cloverleafTrackDataContext.ToListAsync());
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var meet = await db.Meets
                .Include(m => m.Season)
                .Include(m => m.MeetResult)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (meet == null)
            {
                return NotFound();
            }

            return View(meet);
        }

        public IActionResult Create()
        {
            ViewData[nameof(Meet.SeasonId)] = new SelectList(db.Seasons.OrderBy(s => s.Name), nameof(Season.Id), nameof(Season.Name));

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Date,Name,SeasonId,AllResultsIn")] Meet meet)
        {
            if (ModelState.IsValid)
            {
                meet.Id = Guid.NewGuid();
                db.Add(meet);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData[nameof(Meet.SeasonId)] = new SelectList(db.Seasons.OrderBy(s => s.Name), nameof(Season.Id), nameof(Season.Name), meet.SeasonId);

            return View(meet);
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var meet = await db.Meets.FindAsync(id);
            if (meet == null)
            {
                return NotFound();
            }

            ViewData[nameof(Meet.SeasonId)] = new SelectList(db.Seasons.OrderBy(s => s.Name), nameof(Season.Id), nameof(Season.Name), meet.SeasonId);

            return View(meet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Date,Name,SeasonId,AllResultsIn")] Meet meet)
        {
            if (id != meet.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(meet);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MeetExists(meet.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData[nameof(Meet.SeasonId)] = new SelectList(db.Seasons.OrderBy(s => s.Name), nameof(Season.Id), nameof(Season.Name), meet.SeasonId);

            return View(meet);
        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var meet = await db.Meets
                .Include(m => m.Season)
                .Include(m => m.MeetResult)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (meet == null)
            {
                return NotFound();
            }

            return View(meet);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var meet = await db.Meets.FindAsync(id);
            db.Meets.Remove(meet);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MeetExists(Guid id)
        {
            return db.Meets.Any(m => m.Id == id);
        }
    }
}
