using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LeRayBookingSystem.Data;
using LeRayBookingSystem.Models;
using LeRayBookingSystem.Services;

namespace LeRayBookingSystem.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public AppointmentsController(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // GET: Appointments
        public async Task<IActionResult> Index()
        {
            var appointments = _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Service);
            return View(await appointments.ToListAsync());
        }

        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var appointment = await _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Service)
                .FirstOrDefaultAsync(m => m.Id == id);

            return appointment == null ? NotFound() : View(appointment);
        }

        // GET: Appointments/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName");
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Title");
            return View();
        }

        // POST: Appointments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,AppointmentDate,ServiceId,Status,Notes,CreatedAt")] Appointment appointment)
        {
            if (!ModelState.IsValid)
            {
                ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName", appointment.UserId);
                ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Title", appointment.ServiceId);
                return View(appointment);
            }

            appointment.CreatedAt = DateTime.Now;
            _context.Add(appointment);
            await _context.SaveChangesAsync();

            // Send confirmation email
            var user = await _context.Users.FindAsync(appointment.UserId);
            var service = await _context.Services.FindAsync(appointment.ServiceId);

            if (user != null && service != null && !string.IsNullOrWhiteSpace(user.Email))
            {
                var subject = "Appointment Confirmation";
                var body = $@"
                    Hello {user.FullName},<br/><br/>
                    Your appointment for <strong>{service.Title}</strong> on <strong>{appointment.AppointmentDate:MMMM dd, yyyy hh:mm tt}</strong> has been booked successfully.<br/><br/>
                    Thank you,<br/>
                    LeRay Booking System";

                await _emailService.SendEmailAsync(user.Email, subject, body);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
                return NotFound();

            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName", appointment.UserId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Title", appointment.ServiceId);
            return View(appointment);
        }

        // POST: Appointments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,AppointmentDate,ServiceId,Status,Notes,CreatedAt")] Appointment appointment)
        {
            if (id != appointment.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName", appointment.UserId);
                ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Title", appointment.ServiceId);
                return View(appointment);
            }

            try
            {
                appointment.UpdatedAt = DateTime.Now;
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

        // GET: Appointments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var appointment = await _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Service)
                .FirstOrDefaultAsync(m => m.Id == id);

            return appointment == null ? NotFound() : View(appointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
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
