using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RaceDay.API.Models
{
    public class Result
    {
        [Key]
        public int ResultID { get; set; }

        [Required]
        public int EnrolmentID { get; set; }

        public TimeSpan? FinishTime { get; set; }

        public int? Position { get; set; }

        public bool IsCompleted { get; set; } = false;

        [MaxLength(500)]
        public string? Notes { get; set; }

        public DateTime RecordedAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("EnrolmentID")]

        [JsonIgnore]
        public Enrolment? Enrolment { get; set; }
    }
}