using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RaceDay.API.Models
{
    public class Event
    {
        [Key]
        public int EventID { get; set; }

        [Required]
        public int OrganiserID { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        [MaxLength(255)]
        public string Location { get; set; } = string.Empty;

        [Required]
        public decimal Distance { get; set; }

        [Required]
        [MaxLength(20)]
        public string EventType { get; set; } = string.Empty; // Run, Walk, Cycle

        [MaxLength(500)]
        public string? BannerImage { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        // Navigation properties
        [ForeignKey("OrganiserID")]

        [JsonIgnore]
        public Organiser? Organiser { get; set; }

        [JsonIgnore]
        public ICollection<Category> Categories { get; set; } = new List<Category>();
        [JsonIgnore] 
        public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
    }
}