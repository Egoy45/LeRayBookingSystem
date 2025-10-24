using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LeRayBookingSystem.Data;
using LeRayBookingSystem.Models;
using LeRayBookingSystem.Services;

namespace LeRayBookingSystem.Controllers
{
    [Authorize]
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AppointmentsController(ApplicationDbContext context, IEmailService emailService, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _emailService = emailService;
            _userManager = userManager;
        }

        // ✅ ADMIN / STAFF: View all appointments
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Index()
        {
            var appointments = await _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Service)
                .Include(a => a.Promo)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();

            return View(appointments);
        }

        // ✅ CLIENT: View own appointments
        [Authorize(Roles = "Client,Admin,SuperAdmin")]
        public async Task<IActionResult> MyAppointments()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var myAppointments = await _context.Appointments
                .Include(a => a.Service)
                .Include(a => a.Promo)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();

            return View(myAppointments);
        }

        // ✅ DETAILS
        [Authorize(Roles = "Client,Admin,SuperAdmin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var appointment = await _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Service)
                .Include(a => a.Promo)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (appointment == null)
                return NotFound();

            var currentUserId = _userManager.GetUserId(User);
            if (User.IsInRole("Client") && appointment.UserId != currentUserId)
                return Forbid();

            return View(appointment);
        }

        // ✅ GET: Create
        [Authorize(Roles = "Client,Admin,SuperAdmin")]
        public IActionResult Create()
        {
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Title");
            ViewData["PromoId"] = new SelectList(_context.Promos, "Id", "Title");

            if (User.IsInRole("Admin") || User.IsInRole("SuperAdmin"))
                ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName");

            return View();
        }

        // ✅ POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Client,Admin,SuperAdmin")]
        public async Task<IActionResult> Create(Appointment appointment)
        {
            Console.WriteLine("✅ [DEBUG] Appointment Create POST triggered");

            var currentUserId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(currentUserId))
            {
                Console.WriteLine("❌ [ERROR] No logged-in user detected.");
                return Unauthorized();
            }

            // ✅ Assign user automatically for clients
            if (User.IsInRole("Client") || string.IsNullOrEmpty(appointment.UserId))
                appointment.UserId = currentUserId;

            // ✅ Load related Service
            var service = await _context.Services.FirstOrDefaultAsync(s => s.Id == appointment.ServiceId);
            if (service == null)
            {
                Console.WriteLine("❌ [ERROR] Invalid or missing ServiceId.");
                ModelState.AddModelError("ServiceId", "Please select a valid service.");
            }

            // ✅ Fix required fields BEFORE validation
            appointment.ServiceName = service?.Title ?? appointment.ServiceName ?? "Unnamed Service";
            appointment.PaymentMethod = string.IsNullOrWhiteSpace(appointment.PaymentMethod) ? "Unspecified" : appointment.PaymentMethod;

            // ✅ Set system-managed fields
            appointment.PaymentId = Guid.NewGuid().ToString();
            appointment.Status ??= "Pending";
            appointment.PaymentStatus ??= "Unpaid";
            appointment.CreatedAt = DateTime.Now;
            appointment.UpdatedAt = DateTime.Now;
            appointment.LastUpdatedAt = DateTime.Now;
            appointment.CreatedBy = appointment.UserId ?? currentUserId;
            appointment.UpdatedBy = appointment.UserId ?? currentUserId;
            appointment.LastUpdatedBy = appointment.UserId ?? currentUserId;

            if (appointment.AppointmentDate == default)
                appointment.AppointmentDate = DateTime.Now.AddDays(1);

            // ✅ Remove existing model errors for fields we just fixed
            ModelState.Remove("ServiceName");
            ModelState.Remove("PaymentMethod");

            // ✅ Revalidate model after fixing fields
            ModelState.Clear();
            TryValidateModel(appointment);

            foreach (var key in ModelState.Keys)
            {
                foreach (var error in ModelState[key].Errors)
                    Console.WriteLine($"❌ [Validation Error] {key}: {error.ErrorMessage}");
            }

            if (ModelState.IsValid)
            {
                Console.WriteLine("✅ [DEBUG] ModelState valid — saving appointment.");

                _context.Add(appointment);
                await _context.SaveChangesAsync();

                var user = await _context.Users.FindAsync(appointment.UserId);
                if (user != null && service != null && !string.IsNullOrWhiteSpace(user.Email))
                {
                    var subject = "Appointment Confirmation";
                    var body = $@"
                Hello {user.FullName},<br/><br/>
                Your appointment for <strong>{service.Title}</strong> on 
                <strong>{appointment.AppointmentDate:MMMM dd, yyyy hh:mm tt}</strong> 
                has been successfully booked.<br/><br/>
                Thank you,<br/>LeRay Booking System";

                    await _emailService.SendEmailAsync(user.Email, subject, body);
                }

                TempData["Success"] = "Your appointment has been successfully created!";
                return (User.IsInRole("Admin") || User.IsInRole("SuperAdmin"))
                    ? RedirectToAction(nameof(Index))
                    : RedirectToAction(nameof(MyAppointments));
            }

            Console.WriteLine("❌ [DEBUG] ModelState invalid — appointment not saved.");

            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Title", appointment.ServiceId);
            ViewData["PromoId"] = new SelectList(_context.Promos, "Id", "Title", appointment.PromoId);

            return View(appointment);
        }

        // ✅ GET: Edit
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
                return NotFound();

            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName", appointment.UserId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Title", appointment.ServiceId);
            ViewData["PromoId"] = new SelectList(_context.Promos, "Id", "Title", appointment.PromoId);

            return View(appointment);
        }

        // ✅ POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Edit(int id, Appointment appointment)
        {
            if (id != appointment.Id)
                return NotFound();

            var currentUserId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            if (!ModelState.IsValid)
            {
                ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName", appointment.UserId);
                ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Title", appointment.ServiceId);
                ViewData["PromoId"] = new SelectList(_context.Promos, "Id", "Title", appointment.PromoId);
                return View(appointment);
            }

            try
            {
                appointment.UpdatedAt = DateTime.Now;
                appointment.LastUpdatedAt = DateTime.Now;
                appointment.UpdatedBy = currentUserId;
                appointment.LastUpdatedBy = currentUserId;

                _context.Update(appointment);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppointmentExists(appointment.Id))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // ✅ DELETE
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var appointment = await _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Service)
                .Include(a => a.Promo)
                .FirstOrDefaultAsync(m => m.Id == id);

            return appointment == null ? NotFound() : View(appointment);
        }

        // ✅ POST: DeleteConfirmed
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.Id == id);
        }
    }
}
