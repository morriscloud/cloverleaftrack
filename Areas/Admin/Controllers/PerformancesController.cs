using System;
using System.Linq;
using System.Threading.Tasks;

using CloverleafTrack.Data;
using CloverleafTrack.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CloverleafTrack.Controllers
{
    [Area("Admin")]
    public class PerformancesController : Controller
    {
        private readonly ILogger<PerformancesController> logger;
        private readonly CloverleafTrackDataContext db;

        public PerformancesController(ILogger<PerformancesController> logger, CloverleafTrackDataContext db)
        {
            this.logger = logger;
            this.db = db;
        }

        // GET: Performances
        public async Task<IActionResult> Index()
        {
            var cloverleafTrackDataContext = db.Performances.Include(p => p.Athlete).Include(p => p.Meet).Include(p => p.TrackEvent);
            return View(await cloverleafTrackDataContext.ToListAsync());
        }

        // GET: Performances/Details/5
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
                .FirstOrDefaultAsync(m => m.Id == id);
            if (performance == null)
            {
                return NotFound();
            }

            return View(performance);
        }

        // GET: Performances/Create
        public IActionResult Create()
        {
            ViewData[nameof(Performance.TrackEventId)] = new SelectList(db.TrackEvents.OrderBy(x => x.Gender).ThenBy(x => x.Name), nameof(TrackEvent.Id), nameof(TrackEvent.DisplayName));
            ViewData[nameof(Performance.AthleteId)] = new SelectList(db.Athletes.OrderBy(x => x.FirstName).ThenBy(x => x.LastName), nameof(Athlete.Id), nameof(Athlete.Name));
            ViewData[nameof(Performance.MeetId)] = new SelectList(db.Meets.OrderBy(x => x.Name), nameof(Meet.Id), nameof(Meet.Name));
            return View();
        }

        // POST: Performances/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
            ViewData[nameof(Performance.TrackEventId)] = new SelectList(db.TrackEvents.OrderBy(x => x.Gender).ThenBy(x => x.Name), nameof(TrackEvent.Id), nameof(TrackEvent.DisplayName), performance.TrackEventId);
            ViewData[nameof(Performance.AthleteId)] = new SelectList(db.Athletes.OrderBy(x => x.FirstName).ThenBy(x => x.LastName), nameof(Athlete.Id), nameof(Athlete.Name), performance.AthleteId);
            ViewData[nameof(Performance.MeetId)] = new SelectList(db.Meets.OrderBy(x => x.Name), nameof(Meet.Id), nameof(Meet.Name), performance.MeetId);
            return View(performance);
        }

        // GET: Performances/Edit/5
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
                .FirstOrDefaultAsync(x => x.Id == id);
            if (performance == null)
            {
                return NotFound();
            }
            ViewData[nameof(Performance.TrackEventId)] = new SelectList(db.TrackEvents.OrderBy(x => x.Gender).ThenBy(x => x.Name), nameof(TrackEvent.Id), nameof(TrackEvent.DisplayName), performance.TrackEventId);
            ViewData[nameof(Performance.AthleteId)] = new SelectList(db.Athletes.OrderBy(x => x.FirstName).ThenBy(x => x.LastName), nameof(Athlete.Id), nameof(Athlete.Name), performance.AthleteId);
            ViewData[nameof(Performance.MeetId)] = new SelectList(db.Meets.OrderBy(x => x.Name), nameof(Meet.Id), nameof(Meet.Name), performance.MeetId);
            return View(performance);
        }

        // POST: Performances/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData[nameof(Performance.TrackEventId)] = new SelectList(db.TrackEvents.OrderBy(x => x.Gender).ThenBy(x => x.Name), nameof(TrackEvent.Id), nameof(TrackEvent.DisplayName), performance.TrackEventId);
            ViewData[nameof(Performance.AthleteId)] = new SelectList(db.Athletes.OrderBy(x => x.FirstName).ThenBy(x => x.LastName), nameof(Athlete.Id), nameof(Athlete.Name), performance.AthleteId);
            ViewData[nameof(Performance.MeetId)] = new SelectList(db.Meets.OrderBy(x => x.Name), nameof(Meet.Id), nameof(Meet.Name), performance.MeetId);
            return View(performance);
        }

        // GET: Performances/Delete/5
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
                .FirstOrDefaultAsync(m => m.Id == id);
            if (performance == null)
            {
                return NotFound();
            }

            return View(performance);
        }

        // POST: Performances/Delete/5
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
            return db.Performances.Any(e => e.Id == id);
        }
    }
}
