using Panda.Models;
using System.ComponentModel.DataAnnotations;

namespace panda.Models
{
    public enum MentorAssessment { VeryBad = 1, Bad = 2, OK = 3, Good = 4, Great = 5 }

    public class MentorCheckIn
    {
        public int Id { get; set; }

        public int AssignmentId { get; set; }
        public Assignment Assignment { get; set; } = default!;

        public string MentorId { get; set; } = default!;
        public AppUser Mentor { get; set; } = default!;

        [DataType(DataType.Date)]
        public DateTime WeekStart { get; set; }   // samma definition som för TalentCheckIn

        // Mentorens bedömning av läget
        public MentorAssessment Assessment { get; set; } = MentorAssessment.OK; // helhetsbild
        public int Performance { get; set; } = 3;   // 1–5
        public int Collaboration { get; set; } = 3; // 1–5
        public int Pace { get; set; } = 3;          // 1–5 (leveranstakt)

        // Kvalitativ input
        [MaxLength(2000)] public string Observations { get; set; } = "";
        [MaxLength(2000)] public string Recommendations { get; set; } = "";
        [MaxLength(2000)] public string Risks { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

}
