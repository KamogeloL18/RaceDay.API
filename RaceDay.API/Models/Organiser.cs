using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RaceDay.API.Models
{
    public class Organiser
    {
        [Key]
        public int OrganiserID { get; set; }

        [Required]
        public int UserID { get; set; }

        [MaxLength(255)]
        public string? CompanyName { get; set; }

        [MaxLength(20)]
        public string? OrganisationPhoneNumber { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("UserID")]

        [JsonIgnore]
        public User? User { get; set; }

        [JsonIgnore]
        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}