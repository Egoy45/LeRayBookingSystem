using LeRayBookingSystem.Data;
using LeRayBookingSystem.Models;
using LeRayBookingSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace LeRayBookingSystem.Controllers
{
    public class ServicesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AuditLogService _auditService;

        public ServicesController(ApplicationDbContext context, AuditLogService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        // ✅ PUBLIC: View all services
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var services = await _context.Services.ToListAsync();
            return View(services);
        }

        // ✅ PUBLIC: View single service details
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

        // ✅ ADMIN-ONLY: Create (GET)
        [Authorize(Roles = "Admin,SuperAdmin")]
        public IActionResult Create()
        {
            return View();
        }

        // ✅ ADMIN-ONLY: Create (POST) — with Audit Logging
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Create(Service service)
        {
            if (ModelState.IsValid)
            {
                _context.Add(service);
                await _context.SaveChangesAsync();

                // Log creation
                await _auditService.LogAsync(
                    entityType: "Service",
                    action: "Created",
                    entityId: service.Id.ToString(),
                    details: $"Service '{service.ServiceName}' was created."
                );

                return RedirectToAction(nameof(Index));
            }
            return View(service);
        }

        // ✅ ADMIN-ONLY: Edit (GET)
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

        // ✅ ADMIN-ONLY: Edit (POST) — with Audit Logging
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Edit(int id, Service service)
        {
            if (id != service.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(service);
                    await _context.SaveChangesAsync();

                    // Log update
                    await _auditService.LogAsync(
                        entityType: "Service",
                        action: "Updated",
                        entityId: service.Id.ToString(),
                        details: $"Service '{service.ServiceName}' was updated."
                    );
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
            return View(service);
        }

        // ✅ ADMIN-ONLY: Delete (GET)
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

        // ✅ ADMIN-ONLY: Delete (POST) — with Audit Logging
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

                // Log deletion
                await _auditService.LogAsync(
                    entityType: "Service",
                    action: "Deleted",
                    entityId: id.ToString(),
                    details: $"Service '{service.ServiceName}' was deleted."
                );
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
