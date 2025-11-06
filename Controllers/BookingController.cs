using System.Security.Claims;
using System.IO; 
using LeRayBookingSystem.Data;
using LeRayBookingSystem.Models.DTOs;
using LeRayBookingSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting; // REQUIRED for IWebHostEnvironment

namespace LeRayBookingSystem.Controllers
{
    // Inject IWebHostEnvironment to correctly access wwwroot folder.
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BookingsController(
        ApplicationDbContext context, 
        ILogger<BookingsController> logger,
        IWebHostEnvironment webHostEnvironment) : ControllerBase // Inject IWebHostEnvironment here
    {
        private readonly ApplicationDbContext _context = context;
        private readonly ILogger<BookingsController> _logger = logger;
        private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment; // Store the injected environment

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                                      throw new UnauthorizedAccessException("User ID not found in token");

        [HttpGet("my-bookings")]
        public async Task<ActionResult<IEnumerable<BookingDetailsDto>>> GetUserBookings()
        {
            var userId = GetUserId();

            var bookings = await _context.Appointments
                .Where(b => b.UserId == userId && b.Status == "Confirmed" && b.AppointmentDate > DateTime.UtcNow)
                .Include(b => b.Service)
                .Select(b => new BookingDetailsDto
                {
                    Id = b.Id,
                    StartTime = b.AppointmentDate,
                    EndTime = b.Service != null ? b.AppointmentDate.Add(b.Service.Duration) : b.AppointmentDate,
                    Status = b.Status,
                    Notes = b.Notes,
                    Service = (b.Service == null) ? null : new ServiceDto
                    {
                        Id = b.Service!.Id, // Added !
                        Name = b.Service.ServiceName,
                        Description = b.Service.Description,
                        Price = b.Service.Price,
                        DurationInMinutes = (int)b.Service.Duration.TotalMinutes
                    }
                })
                .ToListAsync();

            return Ok(bookings);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromForm] CreateBookingDto createBookingDto) // Add [FromForm]
        {
            // --- 1. Validation Check
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // --- 2. Service and User Retrieval
            var userId = GetUserId();
            var service = await _context.Services.FindAsync(createBookingDto.ServiceId);

            if (service == null)
            {
                return BadRequest("Invalid service ID.");
            }
            
            var startTime = createBookingDto.StartTime; 
            
            if (startTime == DateTime.MinValue)
            {
                return BadRequest("Invalid date or time format in the request.");
            }
            
            var endTime = startTime.Add(service.Duration);

            // --- 3. Overlapping Check
            var overlappingBooking = await _context.Appointments
                .Include(b => b.Service) 
                .AnyAsync(b => b.Status == "Confirmed" &&
                               b.ServiceId == createBookingDto.ServiceId &&
                               b.Service != null && 
                               (startTime < b.AppointmentDate.Add(b.Service.Duration)) &&
                               (endTime > b.AppointmentDate));

            if (overlappingBooking)
            {
                return Conflict("The selected time slot for this service is no longer available.");
            }
            
            // --- 4. Handle File Upload (REQUIRED STEP) ---
            
            // Check if file was provided and is not null
            if (createBookingDto.PaymentReceiptFile == null || createBookingDto.PaymentReceiptFile.Length == 0)
            {
                 // Handle case where file is mandatory but missing
                 return BadRequest("Payment receipt file is mandatory for booking.");
            }

            // 4.1. Setup path - Using IWebHostEnvironment for stable path
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "receipts");
            
            if (!Directory.Exists(uploadsFolder)) 
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // 4.2. Create unique file name and path
            // The null-forgiving operator (!) is used as we checked for null/empty above.
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + createBookingDto.PaymentReceiptFile!.FileName; 
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // 4.3. Save the file
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await createBookingDto.PaymentReceiptFile.CopyToAsync(fileStream);
            }

            // 4.4. Set the path to save in the database
            string receiptPath = $"/receipts/{uniqueFileName}"; 

            // --- 5. Create Appointment Model
            var booking = new Appointment
            {
                UserId = userId,
                ServiceId = service.Id,
                AppointmentDate = startTime,
                Status = "Pending Verification", 
                Notes = createBookingDto.Notes, 
                
                ServiceName = service.ServiceName,
                PaymentMethod = "Online", 
                PaymentStatus = "Pending Verification", 
                
                PaymentReceiptPath = receiptPath, 
                
                CreatedBy = userId,
                UpdatedBy = userId, 
                LastUpdatedBy = userId,
            };

            // --- 6. Save to Database
            await _context.Appointments.AddAsync(booking);
            await _context.SaveChangesAsync();

            _logger.LogInformation("New appointment created: {BookingId} for user {UserId}", booking.Id, userId);

            // --- 7. Return Result
            return CreatedAtAction(nameof(GetUserBookings), new { id = booking.Id }, new BookingDetailsDto
            {
                Id = booking.Id,
                StartTime = booking.AppointmentDate,
                EndTime = endTime,
                Status = booking.Status,
                Notes = booking.Notes,
                Service = new ServiceDto
                {
                    Id = service.Id,
                    Name = service.ServiceName,
                    Description = service.Description,
                    Price = service.Price,
                    DurationInMinutes = (int)service.Duration.TotalMinutes
                }
            });
        }
    }
}