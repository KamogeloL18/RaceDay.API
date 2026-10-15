using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RaceDay.API.Models
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }

        [Required]
        public int EventID { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Description { get; set; }

        public int? MinAge { get; set; }

        public int? MaxAge { get; set; }

        public decimal? Distance { get; set; }

        public decimal? EntryFee { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("EventID")]

        [JsonIgnore]
        public Event? Event { get; set; }
        [JsonIgnore] 
        public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
    }
}