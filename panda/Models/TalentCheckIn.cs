using Panda.Models;
using System.ComponentModel.DataAnnotations;

namespace panda.Models
{
    public enum Mood { VeryBad = 1, Bad = 2, OK = 3, Good = 4, Great = 5 }
    public enum Workload { Under = 1, Balanced = 2, Over = 3 }

    public class TalentCheckIn
    {
        public int Id { get; set; }

        // Koppling
        public int AssignmentId { get; set; }
        public Assignment Assignment { get; set; }

        public string TalentId { get; set; } = default!;
        public AppUser Talent { get; set; } = default!;

        // Vecka (vi sparar måndag som start)
        [DataType(DataType.Date)]
        public DateTime WeekStart { get; set; }  // ex måndag

        // Status
        public Mood Mood { get; set; } = Mood.OK;
        public Workload Workload { get; set; } = Workload.Balanced;

        // Progress
        [MaxLength(2000)] public string WhatWentWell { get; set; } = "";
        [MaxLength(2000)] public string Blockers { get; set; } = "";
        [MaxLength(2000)] public string HelpNeeded { get; set; } = "";
        [MaxLength(2000)] public string GoalsNextWeek { get; set; } = "";

        // Snabba siffror
        public int TasksDone { get; set; }                // hur många uppgifter slutförda
        public int CodeReviewsDone { get; set; }          // hur många code reviews
        public int MeetingsCount { get; set; }            // möten
        public double MentoringHours { get; set; }        // timmar med mentor

        // Tekniska områden (1–5)
        public int SkillCSharp { get; set; } = 3;
        public int SkillDotNet { get; set; } = 3;
        public int SkillSql { get; set; } = 3;
        public int SkillFrontend { get; set; } = 3;
        public int SkillCloud { get; set; } = 3;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

}
