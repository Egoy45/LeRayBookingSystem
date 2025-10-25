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

        // ✅ GET: Services (publicly viewable)
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Services.ToListAsync());
        }

        // ✅ GET: Service Details (publicly viewable)
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
        public async Task<IActionResult> Create([Bind("ServiceName,Description,Category,Price,Duration,IsActive,Title,ImagePath")] Service service)
        {
            var currentUserId = _userManager.GetUserId(User) ?? "System";

            // Set audit fields
            service.CreatedBy = currentUserId;
            service.UpdatedBy = currentUserId;
            service.LastUpdatedBy = currentUserId;
            service.CreatedAt = DateTime.Now;
            service.UpdatedAt = DateTime.Now;
            service.LastUpdatedAt = DateTime.Now;

            // Auto-fill missing title
            if (string.IsNullOrWhiteSpace(service.Title))
                service.Title = service.ServiceName;

            // Allow null image path
            service.ImagePath ??= null;

            ModelState.Clear();
            TryValidateModel(service);

            if (ModelState.IsValid)
            {
                _context.Add(service);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Log validation errors (optional)
            foreach (var key in ModelState.Keys)
            {
                foreach (var error in ModelState[key]!.Errors)
                {
                    Console.WriteLine($"Create Validation Error - {key}: {error.ErrorMessage}");
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,ServiceName,Description,Category,Price,Duration,IsActive,CreatedBy,ImagePath,Title,CreatedAt")] Service service)
        {
            if (id != service.Id)
                return NotFound();

            var currentUserId = _userManager.GetUserId(User) ?? "System";

            // Update audit fields
            service.UpdatedBy = currentUserId;
            service.LastUpdatedBy = currentUserId;
            service.UpdatedAt = DateTime.Now;
            service.LastUpdatedAt = DateTime.Now;

            // Auto-fill missing title
            if (string.IsNullOrWhiteSpace(service.Title))
                service.Title = service.ServiceName;

            // Allow null image path
            if (string.IsNullOrWhiteSpace(service.ImagePath))
                service.ImagePath = null;

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

            // Log validation errors (optional)
            foreach (var key in ModelState.Keys)
            {
                foreach (var error in ModelState[key]!.Errors)
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
