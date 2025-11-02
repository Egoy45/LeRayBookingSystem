using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LeRayBookingSystem.Data;
using LeRayBookingSystem.Models;
using Microsoft.AspNetCore.Identity;
using LeRayBookingSystem.Services;
using Microsoft.AspNetCore.Authorization;

namespace LeRayBookingSystem.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class PromosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuditLogService _auditService;

        public PromosController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, AuditLogService auditService)
        {
            _context = context;
            _userManager = userManager;
            _auditService = auditService;
        }

        // ✅ VIEW ALL PROMOS
        public async Task<IActionResult> Index()
        {
            var promos = await _context.Promos
                .Include(p => p.CreatedByUser)
                .ToListAsync();
            return View(promos);
        }

        // ✅ VIEW DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var promo = await _context.Promos
                .Include(p => p.CreatedByUser)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (promo == null)
                return NotFound();

            return View(promo);
        }

        // ✅ CREATE (GET)
        public IActionResult Create()
        {
            var promo = new Promos
            {
                PromoCode = GeneratePromoCode(),
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(30),
                IsActive = true
            };
            return View(promo);
        }

        // ✅ CREATE (POST) — with Audit Log
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,Price,PromoCode,StartDate,EndDate,IsActive")] Promos promo)
        {
            if (ModelState.IsValid)
            {
                promo.CreatedAt = DateTime.Now;
                promo.CreatedBy = _userManager.GetUserId(User);

                if (string.IsNullOrWhiteSpace(promo.PromoCode))
                    promo.PromoCode = GeneratePromoCode();

                _context.Add(promo);
                await _context.SaveChangesAsync();

                // 🧾 Log creation
                await _auditService.LogAsync(
                    "Promo",
                    "Created",
                    promo.Id.ToString(),
                    $"Promo '{promo.Title}' created with code {promo.PromoCode}."
                );

                return RedirectToAction(nameof(Index));
            }
            return View(promo);
        }

        // ✅ EDIT (GET)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var promo = await _context.Promos.FindAsync(id);
            if (promo == null)
                return NotFound();

            return View(promo);
        }

        // ✅ EDIT (POST) — with Audit Log
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Price,PromoCode,StartDate,EndDate,IsActive,CreatedBy,CreatedAt")] Promos promo)
        {
            if (id != promo.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(promo);
                    await _context.SaveChangesAsync();

                    // 🧾 Log update
                    await _auditService.LogAsync(
                        "Promo",
                        "Updated",
                        promo.Id.ToString(),
                        $"Promo '{promo.Title}' updated (Active: {promo.IsActive})."
                    );
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PromosExists(promo.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(promo);
        }

        // ✅ DELETE (GET)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var promo = await _context.Promos
                .Include(p => p.CreatedByUser)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (promo == null)
                return NotFound();

            return View(promo);
        }

        // ✅ DELETE (POST) — with Audit Log
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var promo = await _context.Promos.FindAsync(id);
            if (promo != null)
            {
                _context.Promos.Remove(promo);
                await _context.SaveChangesAsync();

                // 🧾 Log deletion
                await _auditService.LogAsync(
                    "Promo",
                    "Deleted",
                    id.ToString(),
                    $"Promo '{promo.Title}' deleted from system."
                );
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PromosExists(int id)
        {
            return _context.Promos.Any(e => e.Id == id);
        }

        // ✅ Helper: Generate unique promo code
        private string GeneratePromoCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var suffix = new string(Enumerable.Repeat(chars, 4)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            return $"LeRay-{suffix}";
        }
    }
}
