using CloverleafTrack.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System;
using System.Threading.Tasks;
using CloverleafTrack.Managers;

namespace CloverleafTrack.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AthletesController : Controller
    {
        private readonly IAthleteManager athleteManager;

        public AthletesController(IAthleteManager athleteManager)
        {
            this.athleteManager = athleteManager;
        }

        public async Task<IActionResult> Index()
        {
            return View(await athleteManager.GetOrderedAthletesAsync());
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var athlete = await athleteManager.GetAthleteByIdAsync(id.Value);
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

            await athleteManager.CreateAthleteAsync(athlete);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var athlete = await athleteManager.GetAthleteByIdAsync(id.Value);
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
                await athleteManager.EditAthleteAsync(id, athlete.FirstName, athlete.LastName, athlete.Gender, athlete.GraduationYear);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!athleteManager.AthleteExists(athlete.Id))
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

            var athlete = await athleteManager.GetAthleteByIdAsync(id.Value);
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
            await athleteManager.DeleteAthleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
