namespace LeRayBookingSystem.Models.DTOs
{
    public class UserDto
    {
       public required string Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }
        public required string Role { get; set; }
    }
}