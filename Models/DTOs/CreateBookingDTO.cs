using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http; // Required for IFormFile

using System.Globalization; // Required for robust date/time parsing

namespace LeRayBookingSystem.Models.DTOs
{
    public class CreateBookingDto
    {
        // --- Step 1 Fields (Populated by JavaScript) ---
        
        [Required(ErrorMessage = "A service ID is required for booking.")]
        public int ServiceId { get; set; }

        public decimal TotalPrice { get; set; }
        
        // --- Step 2 Fields (The raw string inputs from the Razor page) ---

        // The date picker sends a string (e.g., "MM/DD/YYYY")
        [Required(ErrorMessage = "Please select an appointment date.")]
        public string AppointmentDateString { get; set; } = string.Empty;

        // The time slot button sends a string (e.g., "11:00 AM - 12:00 PM")
        [Required(ErrorMessage = "Please select an available time slot.")]
        public string TimeSlotString { get; set; } = string.Empty;
        
        // Notes field (assuming this is used, though not explicitly in your HTML)
        public string? Notes { get; set; }
        

        // --- Step 4 Field (File Upload) ---

        // Matches the asp-for="PaymentReceiptFile" from your form (enctype="multipart/form-data")
        [Required(ErrorMessage = "Please upload a clear payment receipt.")]
        public IFormFile PaymentReceiptFile { get; set; } = default!;

        
        // --- Computed Property for API Controller Use ---

        // The API controller needs a single DateTime for the appointment start time.
        // We compute this by combining the date string and the time slot string.
        public DateTime StartTime
        {
            get
            {
                if (string.IsNullOrEmpty(AppointmentDateString) || string.IsNullOrEmpty(TimeSlotString))
                {
                    // This DTO property should only be accessed after ModelState validation passes.
                    return DateTime.MinValue; 
                }

                // 1. Parse Date (e.g., "MM/DD/YYYY")
                if (!DateTime.TryParseExact(AppointmentDateString, "MM/dd/yyyy", CultureInfo.InvariantCulture, 
                    DateTimeStyles.None, out DateTime datePart))
                {
                    // If parsing fails, return min value.
                    return DateTime.MinValue; 
                }

                // 2. Extract and Parse Time (e.g., from "11:00 AM - 12:00 PM" take "11:00 AM")
                var startTimeString = TimeSlotString.Split('-')[0].Trim();
                
                // Use TryParseExact for robust time parsing, including AM/PM
                if (TimeSpan.TryParseExact(startTimeString, new[] { "h:mm tt", "hh:mm tt" }, CultureInfo.InvariantCulture, 
                    TimeSpanStyles.None, out TimeSpan timePart))
                {
                    // 3. Combine Date and Time
                    return datePart.Date.Add(timePart);
                }

                return DateTime.MinValue;
            }
        }
    }
}