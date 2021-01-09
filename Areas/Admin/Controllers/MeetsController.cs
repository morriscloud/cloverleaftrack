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
    public class MeetsController : Controller
    {
        private readonly ILogger<MeetsController> logger;
        private readonly CloverleafTrackDataContext db;

        public MeetsController(ILogger<MeetsController> logger, CloverleafTrackDataContext db)
        {
            this.logger = logger;
            this.db = db;
        }

        // GET: Meets
        public async Task<IActionResult> Index()
        {
            var cloverleafTrackDataContext = db.Meets
                .Include(m => m.Season)
                .Include(m => m.MeetResult)
                .OrderByDescending(x => x.Date);
            return View(await cloverleafTrackDataContext.ToListAsync());
        }

        // GET: Meets/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var meet = await db.Meets
                .Include(m => m.Season)
                .Include(x => x.MeetResult)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (meet == null)
            {
                return NotFound();
            }

            return View(meet);
        }

        // GET: Meets/Create
        public IActionResult Create()
        {
            ViewData[nameof(Meet.SeasonId)] = new SelectList(db.Seasons.OrderBy(x => x.Name), nameof(Season.Id), nameof(Season.Name));
            return View();
        }

        // POST: Meets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Date,Name,SeasonId")] Meet meet)
        {
            if (ModelState.IsValid)
            {
                meet.Id = Guid.NewGuid();
                db.Add(meet);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData[nameof(Meet.SeasonId)] = new SelectList(db.Seasons.OrderBy(x => x.Name), nameof(Season.Id), nameof(Season.Name), meet.SeasonId);
            return View(meet);
        }

        // GET: Meets/Edit/5
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
            ViewData[nameof(Meet.SeasonId)] = new SelectList(db.Seasons.OrderBy(x => x.Name), nameof(Season.Id), nameof(Season.Name), meet.SeasonId);
            return View(meet);
        }

        // POST: Meets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Date,Name,SeasonId")] Meet meet)
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
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData[nameof(Meet.SeasonId)] = new SelectList(db.Seasons.OrderBy(x => x.Name), nameof(Season.Id), nameof(Season.Name), meet.SeasonId);
            return View(meet);
        }

        // GET: Meets/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var meet = await db.Meets
                .Include(m => m.Season)
                .Include(x => x.MeetResult)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (meet == null)
            {
                return NotFound();
            }

            return View(meet);
        }

        // POST: Meets/Delete/5
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
            return db.Meets.Any(e => e.Id == id);
        }
    }
}
