using Panda.Models;

namespace panda.Models
{
     public class FeedbackEntry
    {
         public int Id { get; set; }

        public int AssignmentId { get; set; }
        public Assignment Assignment { get; set; }

        public string AuthorId { get; set; }
        public AppUser Author { get; set; }

        // Ny struktur
        public string Category { get; set; }   // t.ex. "Arbete", "Samarbete", "Trivsel", "Förslag"
        public int? Rating { get; set; }       // 1–5, används om relevant
        public string Message { get; set; }    // fritext

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
