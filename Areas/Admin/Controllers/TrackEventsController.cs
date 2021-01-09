using System;
using System.Linq;
using System.Threading.Tasks;

using CloverleafTrack.Data;
using CloverleafTrack.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CloverleafTrack.Controllers
{
    [Area("Admin")]
    public class TrackEventsController : Controller
    {
        private readonly ILogger<TrackEventsController> logger;
        private readonly CloverleafTrackDataContext db;

        public TrackEventsController(ILogger<TrackEventsController> logger, CloverleafTrackDataContext db)
        {
            this.logger = logger;
            this.db = db;
        }

        // GET: TrackEvents
        public async Task<IActionResult> Index()
        {
            return View(await db.TrackEvents.OrderBy(x => x.Gender).ThenBy(x => x.Name).ToListAsync());
        }

        // GET: TrackEvents/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trackEvent = await db.TrackEvents
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trackEvent == null)
            {
                return NotFound();
            }

            return View(trackEvent);
        }

        // GET: TrackEvents/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TrackEvents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Gender,RunningEvent,RelayEvent")] TrackEvent trackEvent)
        {
            if (ModelState.IsValid)
            {
                trackEvent.Id = Guid.NewGuid();
                db.Add(trackEvent);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(trackEvent);
        }

        // GET: TrackEvents/Edit/5
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

        // POST: TrackEvents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,Gender,RunningEvent,RelayEvent")] TrackEvent trackEvent)
        {
            if (id != trackEvent.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
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
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(trackEvent);
        }

        // GET: TrackEvents/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trackEvent = await db.TrackEvents
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trackEvent == null)
            {
                return NotFound();
            }

            return View(trackEvent);
        }

        // POST: TrackEvents/Delete/5
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
            return db.TrackEvents.Any(e => e.Id == id);
        }
    }
}
