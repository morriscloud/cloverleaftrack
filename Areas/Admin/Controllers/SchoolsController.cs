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
    public class SchoolsController : Controller
    {
        private readonly ILogger<SchoolsController> logger;
        private readonly CloverleafTrackDataContext db;

        public SchoolsController(ILogger<SchoolsController> logger, CloverleafTrackDataContext db)
        {
            this.logger = logger;
            this.db = db;
        }

        // GET: Schools
        public async Task<IActionResult> Index()
        {
            return View(await db.Schools.ToListAsync());
        }

        // GET: Schools/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var school = await db.Schools
                .FirstOrDefaultAsync(m => m.Id == id);
            if (school == null)
            {
                return NotFound();
            }

            return View(school);
        }

        // GET: Schools/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Schools/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] School school)
        {
            if (ModelState.IsValid)
            {
                school.Id = Guid.NewGuid();
                db.Add(school);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(school);
        }

        // GET: Schools/Edit/5
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

        // POST: Schools/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name")] School school)
        {
            if (id != school.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
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
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(school);
        }

        // GET: Schools/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var school = await db.Schools
                .FirstOrDefaultAsync(m => m.Id == id);
            if (school == null)
            {
                return NotFound();
            }

            return View(school);
        }

        // POST: Schools/Delete/5
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
            return db.Schools.Any(e => e.Id == id);
        }
    }
}
