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
    public class TrackEventsController : Controller
    {
        private readonly CloverleafTrackDataContext db;

        public TrackEventsController(CloverleafTrackDataContext db)
        {
            this.db = db;
        }

        public async Task<IActionResult> Index()
        {
            return View(await db.TrackEvents.OrderBy(t => t.Gender).ThenBy(t => t.SortOrder).ToListAsync());
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trackEvent = await db.TrackEvents
                .FirstOrDefaultAsync(t => t.Id == id);
            
            if (trackEvent == null)
            {
                return NotFound();
            }

            return View(trackEvent);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Gender,RunningEvent,RelayEvent,SortOrder")] TrackEvent trackEvent)
        {
            if (!ModelState.IsValid)
            {
                return View(trackEvent);
            }

            trackEvent.Id = Guid.NewGuid();
            db.Add(trackEvent);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trackEvent = await db.TrackEvents.FindAsync(id);
            
            if (trackEvent == null)
            {
                return NotFound();
            }
            
            return View(trackEvent);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,Gender,RunningEvent,RelayEvent,SortOrder")] TrackEvent trackEvent)
        {
            if (id != trackEvent.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(trackEvent);
            }

            try
            {
                db.Update(trackEvent);
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TrackEventExists(trackEvent.Id))
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

            var trackEvent = await db.TrackEvents
                .FirstOrDefaultAsync(t => t.Id == id);
            
            if (trackEvent == null)
            {
                return NotFound();
            }

            return View(trackEvent);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var trackEvent = await db.TrackEvents.FindAsync(id);
            db.TrackEvents.Remove(trackEvent);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrackEventExists(Guid id)
        {
            return db.TrackEvents.Any(t => t.Id == id);
        }
    }
}
