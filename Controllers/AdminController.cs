
using LeRayBookingSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LeRayBookingSystem.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
         
        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var totalAppointments = await _context.Appointments.CountAsync();
            var pendingAppointments = await _context.Appointments.CountAsync(a => a.Status == "Pending");
            var approvedAppointments = await _context.Appointments.CountAsync(a => a.Status == "Approved");
            var cancelledAppointments = await _context.Appointments.CountAsync(a => a.Status == "Cancelled");

            var totalRevenue = await _context.Appointments
                .Where(a => a.Status == "Approved")
                .SumAsync(a => (decimal?)a.Service!.Price) ?? 0;

            var appointmentsByMonth = await _context.Appointments
                .GroupBy(a => new { a.AppointmentDate.Year, a.AppointmentDate.Month })
                .Select(g => new
                {
                    Month = $"{g.Key.Month}/{g.Key.Year}",
                    Count = g.Count()
                })
                .ToListAsync();

            ViewBag.TotalAppointments = totalAppointments;
            ViewBag.PendingAppointments = pendingAppointments;
            ViewBag.ApprovedAppointments = approvedAppointments;
            ViewBag.CancelledAppointments = cancelledAppointments;
            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.AppointmentsByMonth = appointmentsByMonth;

            return View();
        }
    }
}
