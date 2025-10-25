using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LeRayBookingSystem.Data;
using LeRayBookingSystem.Models;

namespace LeRayBookingSystem.Controllers
{
    public class PromosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PromosController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Promos
        public async Task<IActionResult> Index()
        {
            var promos = await _context.Promos
                .Include(p => p.CreatedByUser)
                .ToListAsync();

            return View(promos);
        }

        // GET: Promos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var promo = await _context.Promos
                .Include(p => p.CreatedByUser)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (promo == null) return NotFound();

            return View(promo);
        }

        // GET: Promos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Promos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,Price,Duration,PromoCode,StartDate,EndDate,IsActive")] Promos promo)
        {
            if (!ModelState.IsValid)
            {
                return View(promo);
            }

            promo.CreatedAt = DateTime.Now;

            // Automatically set CreatedBy to the logged-in user's Id
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                promo.CreatedBy = user.Id;
            }

            // Ensure Duration doesn't exceed 24 hours for MySQL
            if (promo.Duration.TotalHours >= 24)
            {
                promo.Duration = new TimeSpan(23, 59, 59);
            }

            _context.Add(promo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Promos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var promo = await _context.Promos.FindAsync(id);
            if (promo == null) return NotFound();

            return View(promo);
        }

        // POST: Promos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Price,Duration,PromoCode,StartDate,EndDate,IsActive,CreatedBy,CreatedAt")] Promos promo)
        {
            if (id != promo.Id) return NotFound();

            if (promo.Duration.TotalHours >= 24)
            {
                promo.Duration = new TimeSpan(23, 59, 59);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(promo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PromosExists(promo.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(promo);
        }

        // GET: Promos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var promo = await _context.Promos
                .Include(p => p.CreatedByUser)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (promo == null) return NotFound();

            return View(promo);
        }

        // POST: Promos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var promo = await _context.Promos.FindAsync(id);
            if (promo != null)
            {
                _context.Promos.Remove(promo);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PromosExists(int id)
        {
            return _context.Promos.Any(e => e.Id == id);
        }
    }
}
