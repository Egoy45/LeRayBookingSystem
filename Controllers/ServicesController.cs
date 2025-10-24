using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LeRayBookingSystem.Data;
using LeRayBookingSystem.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LeRayBookingSystem.Controllers
{
    [Authorize] // Only authenticated users can access
    public class ServicesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ServicesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ✅ GET: Services
        [AllowAnonymous] // Allow public viewing
        public async Task<IActionResult> Index()
        {
            return View(await _context.Services.ToListAsync());
        }

        // ✅ GET: Details
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var service = await _context.Services.FirstOrDefaultAsync(m => m.Id == id);
            if (service == null)
                return NotFound();

            return View(service);
        }

        // ✅ ADMIN-ONLY: GET Create
        [Authorize(Roles = "Admin,SuperAdmin")]
        public IActionResult Create()
        {
            return View();
        }

        // ✅ ADMIN-ONLY: POST Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Create([Bind("ServiceName,Description,Category,Price,Duration,IsActive")] Service service)
        {
            // Step 1: Get current logged-in user's ID
            var currentUserId = _userManager.GetUserId(User) ?? "System";

            // Step 2: Fill system-managed fields
            service.CreatedBy = currentUserId;
            service.UpdatedBy = currentUserId;
            service.LastUpdatedBy = currentUserId;
            service.CreatedAt = DateTime.Now;
            service.UpdatedAt = DateTime.Now;
            service.LastUpdatedAt = DateTime.Now;

            // Auto-fill Title if missing
            if (string.IsNullOrWhiteSpace(service.Title))
                service.Title = service.ServiceName;

            // Allow null ImagePath (optional upload)
            service.ImagePath ??= null;

            // Step 3: Clear model state and re-validate with new values
            ModelState.Clear();
            TryValidateModel(service);

            if (ModelState.IsValid)
            {
                _context.Add(service);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Log validation errors (for debug)
            foreach (var key in ModelState.Keys)
            {
                foreach (var error in ModelState[key].Errors)
                {
                    Console.WriteLine($"Validation Error - {key}: {error.ErrorMessage}");
                }
            }

            return View(service);
        }

        // ✅ ADMIN-ONLY: GET Edit
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var service = await _context.Services.FindAsync(id);
            if (service == null)
                return NotFound();

            return View(service);
        }

        // ✅ ADMIN-ONLY: POST Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ServiceName,Description,Category,Price,Duration,IsActive,CreatedBy,ImagePath,Title")] Service service)
        {
            if (id != service.Id)
                return NotFound();

            // Step 1: Get current logged-in user's ID
            var currentUserId = _userManager.GetUserId(User) ?? "System";

            // Step 2: Auto-update audit fields
            service.UpdatedBy = currentUserId;
            service.LastUpdatedBy = currentUserId;
            service.UpdatedAt = DateTime.Now;
            service.LastUpdatedAt = DateTime.Now;

            // Auto-fill Title if missing
            if (string.IsNullOrWhiteSpace(service.Title))
                service.Title = service.ServiceName;

            // Allow ImagePath to remain null until image is uploaded
            if (string.IsNullOrWhiteSpace(service.ImagePath))
                service.ImagePath = null;

            // Step 3: Re-validate
            ModelState.Clear();
            TryValidateModel(service);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(service);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Services.Any(e => e.Id == service.Id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            // Log validation errors
            foreach (var key in ModelState.Keys)
            {
                foreach (var error in ModelState[key].Errors)
                {
                    Console.WriteLine($"Edit Validation Error - {key}: {error.ErrorMessage}");
                }
            }

            return View(service);
        }

        // ✅ ADMIN-ONLY: GET Delete
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var service = await _context.Services.FirstOrDefaultAsync(m => m.Id == id);
            if (service == null)
                return NotFound();

            return View(service);
        }

        // ✅ ADMIN-ONLY: POST Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service != null)
            {
                _context.Services.Remove(service);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
