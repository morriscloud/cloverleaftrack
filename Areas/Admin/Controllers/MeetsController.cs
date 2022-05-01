using CloverleafTrack.Data;
using CloverleafTrack.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using System;
using System.Linq;
using System.Threading.Tasks;
using CloverleafTrack.Managers;

namespace CloverleafTrack.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MeetsController : Controller
    {
        private readonly IMeetManager meetManager;
        private readonly ISeasonManager seasonManager;

        public MeetsController(IMeetManager meetManager, ISeasonManager seasonManager)
        {
            this.meetManager = meetManager;
            this.seasonManager = seasonManager;
        }

        public async Task<IActionResult> Index()
        {       
            return View(await meetManager.GetAsync());
        }

        public async Task<IActionResult> Done(Guid? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            await meetManager.DoneAsync(id.Value);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var meet = await meetManager.GetByIdAsync(id.Value);
            if (meet == null)
            {
                return NotFound();
            }

            return View(meet);
        }

        public async Task<IActionResult> Create()
        {
            var seasons = await seasonManager.GetAsync();
            ViewData[nameof(Meet.SeasonId)] = new SelectList(seasons, nameof(Season.Id), nameof(Season.Name));

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Date,Name,SeasonId,Outdoor,Location,AllResultsIn,HandTimed")] Meet meet)
        {
            if (ModelState.IsValid)
            {
                meet.Id = Guid.NewGuid();
                db.Add(meet);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var seasons = await seasonManager.GetAsync();
            ViewData[nameof(Meet.SeasonId)] = new SelectList(seasons, nameof(Season.Id), nameof(Season.Name), meet.SeasonId);

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

            var seasons = await seasonManager.GetAsync();
            ViewData[nameof(Meet.SeasonId)] = new SelectList(seasons, nameof(Season.Id), nameof(Season.Name), meet.SeasonId);

            return View(meet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Date,Name,SeasonId,Outdoor,Location,AllResultsIn,HandTimed")] Meet meet)
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

            var seasons = await seasonManager.GetAsync();
            ViewData[nameof(Meet.SeasonId)] = new SelectList(seasons, nameof(Season.Id), nameof(Season.Name), meet.SeasonId);

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
