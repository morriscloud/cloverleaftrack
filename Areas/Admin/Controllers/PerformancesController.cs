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
    public class PerformancesController : Controller
    {
        private readonly CloverleafTrackDataContext db;

        public PerformancesController(CloverleafTrackDataContext db)
        {
            this.db = db;
        }

        public async Task<IActionResult> Index()
        {
            var cloverleafTrackDataContext = db.Performances
                .Include(p => p.Athlete)
                .Include(p => p.Meet)
                .Include(p => p.TrackEvent)
                .OrderBy(p => p.Athlete.FirstName)
                .ThenBy(p => p.Athlete.LastName)
                .ThenBy(p => p.TrackEvent.SortOrder)
                .ThenBy(p => p.Meet.Date);

            return View(await cloverleafTrackDataContext.ToListAsync());
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var performance = await db.Performances
                .Include(p => p.Athlete)
                .Include(p => p.Meet)
                .Include(p => p.TrackEvent)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (performance == null)
            {
                return NotFound();
            }

            return View(performance);
        }

        public IActionResult Create()
        {
            ViewData[nameof(Performance.TrackEventId)] = new SelectList(db.TrackEvents.OrderBy(t => t.Gender).ThenBy(t => t.SortOrder), nameof(TrackEvent.Id), nameof(TrackEvent.DisplayName));
            ViewData[nameof(Performance.AthleteId)] = new SelectList(db.Athletes.OrderBy(a => a.FirstName).ThenBy(a => a.LastName), nameof(Athlete.Id), nameof(Athlete.Name));
            ViewData[nameof(Performance.MeetId)] = new SelectList(db.Meets.OrderBy(m => m.Name), nameof(Meet.Id), nameof(Meet.Name));
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventName,RunningEvent,Minutes,Seconds,Milliseconds,Feet,Inches,FractionalInches,Place,TrackEventId,AthleteId,MeetId")] Performance performance)
        {
            if (ModelState.IsValid)
            {
                performance.Id = Guid.NewGuid();
                db.Add(performance);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            ViewData[nameof(Performance.TrackEventId)] = new SelectList(db.TrackEvents.OrderBy(t => t.Gender).ThenBy(t => t.SortOrder), nameof(TrackEvent.Id), nameof(TrackEvent.DisplayName), performance.TrackEventId);
            ViewData[nameof(Performance.AthleteId)] = new SelectList(db.Athletes.OrderBy(a => a.FirstName).ThenBy(a => a.LastName), nameof(Athlete.Id), nameof(Athlete.Name), performance.AthleteId);
            ViewData[nameof(Performance.MeetId)] = new SelectList(db.Meets.OrderBy(m => m.Name), nameof(Meet.Id), nameof(Meet.Name), performance.MeetId);
            
            return View(performance);
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var performance = await db.Performances
                .Include(p => p.Athlete)
                .Include(p => p.Meet)
                .Include(p => p.TrackEvent)
                .FirstOrDefaultAsync(p => p.Id == id);
            
            if (performance == null)
            {
                return NotFound();
            }
            ViewData[nameof(Performance.TrackEventId)] = new SelectList(db.TrackEvents.OrderBy(t => t.Gender).ThenBy(t => t.SortOrder), nameof(TrackEvent.Id), nameof(TrackEvent.DisplayName), performance.TrackEventId);
            ViewData[nameof(Performance.AthleteId)] = new SelectList(db.Athletes.OrderBy(a => a.FirstName).ThenBy(a => a.LastName), nameof(Athlete.Id), nameof(Athlete.Name), performance.AthleteId);
            ViewData[nameof(Performance.MeetId)] = new SelectList(db.Meets.OrderBy(m => m.Name), nameof(Meet.Id), nameof(Meet.Name), performance.MeetId);
            return View(performance);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,EventName,RunningEvent,Minutes,Seconds,Milliseconds,Feet,Inches,FractionalInches,Place,TrackEventId,AthleteId,MeetId")] Performance performance)
        {
            if (id != performance.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(performance);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PerformanceExists(performance.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData[nameof(Performance.TrackEventId)] = new SelectList(db.TrackEvents.OrderBy(t => t.Gender).ThenBy(t => t.SortOrder), nameof(TrackEvent.Id), nameof(TrackEvent.DisplayName), performance.TrackEventId);
            ViewData[nameof(Performance.AthleteId)] = new SelectList(db.Athletes.OrderBy(a => a.FirstName).ThenBy(a => a.LastName), nameof(Athlete.Id), nameof(Athlete.Name), performance.AthleteId);
            ViewData[nameof(Performance.MeetId)] = new SelectList(db.Meets.OrderBy(m => m.Name), nameof(Meet.Id), nameof(Meet.Name), performance.MeetId);
            return View(performance);
        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var performance = await db.Performances
                .Include(p => p.Athlete)
                .Include(p => p.Meet)
                .Include(p => p.TrackEvent)
                .FirstOrDefaultAsync(p => p.Id == id);
            
            if (performance == null)
            {
                return NotFound();
            }

            return View(performance);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var performance = await db.Performances.FindAsync(id);
            db.Performances.Remove(performance);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PerformanceExists(Guid id)
        {
            return db.Performances.Any(p => p.Id == id);
        }
    }
}
