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
    public class SchoolsController : Controller
    {
        private readonly CloverleafTrackDataContext db;

        public SchoolsController(CloverleafTrackDataContext db)
        {
            this.db = db;
        }

        public async Task<IActionResult> Index()
        {
            return View(await db.Schools.ToListAsync());
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var school = await db.Schools
                .FirstOrDefaultAsync(s => s.Id == id);
            
            if (school == null)
            {
                return NotFound();
            }

            return View(school);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] School school)
        {
            if (!ModelState.IsValid)
            {
                return View(school);
            }

            school.Id = Guid.NewGuid();
            db.Add(school);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var school = await db.Schools.FindAsync(id);
            
            if (school == null)
            {
                return NotFound();
            }
            
            return View(school);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name")] School school)
        {
            if (id != school.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(school);
            }

            try
            {
                db.Update(school);
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SchoolExists(school.Id))
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

            var school = await db.Schools
                .FirstOrDefaultAsync(s => s.Id == id);
            if (school == null)
            {
                return NotFound();
            }

            return View(school);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var school = await db.Schools.FindAsync(id);
            db.Schools.Remove(school);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SchoolExists(Guid id)
        {
            return db.Schools.Any(s => s.Id == id);
        }
    }
}
