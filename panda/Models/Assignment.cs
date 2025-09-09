using Panda.Models;

namespace panda.Models
{
    public class Assignment
    {
        public int Id { get; set; }

        public string EmployerId { get; set; }
        public AppUser Employer { get; set; }

        public string TalentId { get; set; }
        public AppUser Talent { get; set; }

        public string MentorId { get; set; }
        public AppUser Mentor { get; set; }

        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; }

        public ICollection<FeedbackEntry> FeedbackEntries { get; set; } = new List<FeedbackEntry>();
    }

}
