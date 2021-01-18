using CloverleafTrack.Data;
using CloverleafTrack.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace CloverleafTrack.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AthletesController : Controller
    {
        private readonly CloverleafTrackDataContext db;

        public AthletesController(CloverleafTrackDataContext db)
        {
            this.db = db;
        }

        public async Task<IActionResult> Index()
        {
            return View(await db.Athletes.OrderBy(a => a.FirstName).ThenBy(a => a.LastName).ThenBy(a => a.GraduationYear).ToListAsync());
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var athlete = await db.Athletes.FirstOrDefaultAsync(a => a.Id == id);

            if (athlete == null)
            {
                return NotFound();
            }

            return View(athlete);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Gender,GraduationYear")] Athlete athlete)
        {
            if (!ModelState.IsValid)
            {
                return View(athlete);
            }

            athlete.Id = Guid.NewGuid();
            db.Add(athlete);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,FirstName,LastName,Gender,GraduationYear")] Athlete athlete)
        {
            if (id != athlete.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(athlete);
            }

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

            var athlete = await db.Athletes.FirstOrDefaultAsync(a => a.Id == id);

            if (athlete == null)
            {
                return NotFound();
            }

            return View(athlete);
        }

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
            return db.Athletes.Any(a => a.Id == id);
        }
    }
}
