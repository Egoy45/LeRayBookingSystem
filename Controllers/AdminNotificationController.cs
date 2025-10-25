using LeRayBookingSystem.Data;
using LeRayBookingSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LeRayBookingSystem.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class AdminNotificationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminNotificationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AdminNotifications
        public async Task<IActionResult> Index()
        {
            var notifications = await _context.AdminNotifications
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return View(notifications);
        }

        // POST: Mark as read
        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notification = await _context.AdminNotifications.FindAsync(id);
            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Optional: delete notification
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var notification = await _context.AdminNotifications.FindAsync(id);
            if (notification != null)
            {
                _context.AdminNotifications.Remove(notification);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
