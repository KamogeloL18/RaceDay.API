using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RaceDay.API.Models
{
    public class Enrolment
    {
        [Key]
        public int EnrolmentID { get; set; }

        [Required]
        public int ParticipantID { get; set; }

        [Required]
        public int EventID { get; set; }

        [Required]
        public int CategoryID { get; set; }

        public DateTime EnrolmentDate { get; set; } = DateTime.Now;

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Confirmed, Completed, Cancelled

        [Required]
        [MaxLength(20)]
        public string PaymentStatus { get; set; } = "Unpaid"; // Unpaid, Paid, Refunded

        // Navigation properties
        [ForeignKey("ParticipantID")]

        [JsonIgnore]
        public Participant? Participant { get; set; }

        [ForeignKey("EventID")]

        [JsonIgnore]
        public Event? Event { get; set; }

        [ForeignKey("CategoryID")]

        [JsonIgnore]
        public Category? Category { get; set; }

        [JsonIgnore]
        public Result? Result { get; set; }
    }
}