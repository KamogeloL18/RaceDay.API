using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RaceDay.API.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Role { get; set; } = string.Empty; // Organiser or Participant

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        // Navigation properties
        [JsonIgnore]
        public Participant? Participant { get; set; }
        [JsonIgnore]
        public Organiser? Organiser { get; set; }
    }
}