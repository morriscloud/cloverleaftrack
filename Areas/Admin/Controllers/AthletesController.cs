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
    public class AthletesController : Controller
    {
        private readonly ILogger<AthletesController> logger;
        private readonly CloverleafTrackDataContext db;

        public AthletesController(ILogger<AthletesController> logger, CloverleafTrackDataContext db)
        {
            this.logger = logger;
            this.db = db;
        }

        // GET: Athletes
        public async Task<IActionResult> Index()
        {
            return View(await db.Athletes.ToListAsync());
        }

        // GET: Athletes/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var athlete = await db.Athletes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (athlete == null)
            {
                return NotFound();
            }

            return View(athlete);
        }

        // GET: Athletes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Athletes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Gender,GraduationYear")] Athlete athlete)
        {
            if (ModelState.IsValid)
            {
                athlete.Id = Guid.NewGuid();
                db.Add(athlete);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(athlete);
        }

        // GET: Athletes/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var athlete = await db.Athletes.FindAsync(id);
            if (athlete == null)
            {
                return NotFound();
            }
            return View(athlete);
        }

        // POST: Athletes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,FirstName,LastName,Gender,GraduationYear")] Athlete athlete)
        {
            if (id != athlete.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(athlete);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AthleteExists(athlete.Id))
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
            return View(athlete);
        }

        // GET: Athletes/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var athlete = await db.Athletes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (athlete == null)
            {
                return NotFound();
            }

            return View(athlete);
        }

        // POST: Athletes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var athlete = await db.Athletes.FindAsync(id);
            db.Athletes.Remove(athlete);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AthleteExists(Guid id)
        {
            return db.Athletes.Any(e => e.Id == id);
        }
    }
}
