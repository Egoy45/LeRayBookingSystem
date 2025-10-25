using LeRayBookingSystem.Data;
using LeRayBookingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeRayBookingSystem.Services
{
    public class AdminNotificationService
    {
        private readonly ApplicationDbContext _context;

        public AdminNotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Create a notification
        public async Task<AdminNotification> CreateAsync(
            string message,
            string recipientRole,
            string? relatedEntity = null,
            int? relatedEntityId = null,
            string? link = null)
        {
            var notification = new AdminNotification
            {
                Message = message,
                RecipientRole = recipientRole,
                RelatedEntity = relatedEntity,
                RelatedEntityId = relatedEntityId,
                Link = link,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.AdminNotifications.Add(notification);
            await _context.SaveChangesAsync();
            return notification;
        }

        // Get all notifications for a role (latest first)
        public async Task<List<AdminNotification>> GetAllAsync(string recipientRole)
        {
            return await _context.AdminNotifications
                .AsNoTracking()
                .Where(n => n.RecipientRole == recipientRole)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        // Mark notification as read
        public async Task MarkAsReadAsync(int id)
        {
            var notification = await _context.AdminNotifications.FindAsync(id);
            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}
