namespace RaceDay.Web.Models
{
    public class ParticipantEnrolmentDto
    {
        public int EnrolmentID { get; set; }
        public int EventID { get; set; }
        public int CategoryID { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public DateTime EnrolmentDate { get; set; }

        public EventDto? Event { get; set; }
        public CategoryDto? Category { get; set; }
    }
}