using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using panda.Models;
using Panda.Data;
using Panda.Models;
using System.Globalization;

namespace Panda.Pages.Talang
{
    [Authorize(Roles = "Talang")]
    public class CheckInModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<AppUser> _userManager;

        public CheckInModel(ApplicationDbContext db, UserManager<AppUser> userManager)
        {
            _db = db; _userManager = userManager;
        }

        // visas i formuläret
        public Assignment Assignment { get; set; } = default!;

        [BindProperty] public int AssignmentId { get; set; }
        [BindProperty] public DateTime WeekStart { get; set; }

        [BindProperty] public Mood Mood { get; set; } = Mood.OK;
        [BindProperty] public Workload Workload { get; set; } = Workload.Balanced;

        [BindProperty] public string WhatWentWell { get; set; } = "";
        [BindProperty] public string Blockers { get; set; } = "";
        [BindProperty] public string HelpNeeded { get; set; } = "";
        [BindProperty] public string GoalsNextWeek { get; set; } = "";

        [BindProperty] public int TasksDone { get; set; }
        [BindProperty] public int CodeReviewsDone { get; set; }
        [BindProperty] public int MeetingsCount { get; set; }
        [BindProperty] public double MentoringHours { get; set; }

        [BindProperty] public int SkillCSharp { get; set; } = 3;
        [BindProperty] public int SkillDotNet { get; set; } = 3;
        [BindProperty] public int SkillSql { get; set; } = 3;
        [BindProperty] public int SkillFrontend { get; set; } = 3;
        [BindProperty] public int SkillCloud { get; set; } = 3;

        private static DateTime StartOfWeek(DateTime date, DayOfWeek start = DayOfWeek.Monday)
        {
            int diff = (7 + (date.DayOfWeek - start)) % 7;
            return date.Date.AddDays(-1 * diff);
        }

        public async Task<IActionResult> OnGetAsync(int assignmentId)
        {
            var talent = await _userManager.GetUserAsync(User);
            Assignment = await _db.Assignments
                .Include(a => a.Employer)
                .Include(a => a.Mentor)
                .FirstOrDefaultAsync(a => a.Id == assignmentId);

            if (Assignment == null || Assignment.TalentId != talent.Id)
                return Forbid();

            AssignmentId = assignmentId;
            WeekStart = StartOfWeek(DateTime.UtcNow);

            // Om veckans check-in redan finns: ladda för redigering
            var existing = await _db.TalentCheckIns
                .FirstOrDefaultAsync(x => x.AssignmentId == AssignmentId
                                       && x.TalentId == talent.Id
                                       && x.WeekStart == WeekStart);

            if (existing != null)
            {
                Mood = existing.Mood; Workload = existing.Workload;
                WhatWentWell = existing.WhatWentWell; Blockers = existing.Blockers;
                HelpNeeded = existing.HelpNeeded; GoalsNextWeek = existing.GoalsNextWeek;
                TasksDone = existing.TasksDone; CodeReviewsDone = existing.CodeReviewsDone;
                MeetingsCount = existing.MeetingsCount; MentoringHours = existing.MentoringHours;
                SkillCSharp = existing.SkillCSharp; SkillDotNet = existing.SkillDotNet;
                SkillSql = existing.SkillSql; SkillFrontend = existing.SkillFrontend; SkillCloud = existing.SkillCloud;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var talent = await _userManager.GetUserAsync(User);
            var assignment = await _db.Assignments.FindAsync(AssignmentId);
            if (assignment == null || assignment.TalentId != talent.Id) return Forbid();

            WeekStart = StartOfWeek(WeekStart == default ? DateTime.UtcNow : WeekStart);

            var existing = await _db.TalentCheckIns
                .FirstOrDefaultAsync(x => x.AssignmentId == AssignmentId
                                       && x.TalentId == talent.Id
                                       && x.WeekStart == WeekStart);

            if (existing == null)
            {
                var ci = new TalentCheckIn
                {
                    AssignmentId = AssignmentId,
                    TalentId = talent.Id,
                    WeekStart = WeekStart,
                    Mood = Mood,
                    Workload = Workload,
                    WhatWentWell = WhatWentWell,
                    Blockers = Blockers,
                    HelpNeeded = HelpNeeded,
                    GoalsNextWeek = GoalsNextWeek,
                    TasksDone = TasksDone,
                    CodeReviewsDone = CodeReviewsDone,
                    MeetingsCount = MeetingsCount,
                    MentoringHours = MentoringHours,
                    SkillCSharp = SkillCSharp,
                    SkillDotNet = SkillDotNet,
                    SkillSql = SkillSql,
                    SkillFrontend = SkillFrontend,
                    SkillCloud = SkillCloud
                };
                _db.TalentCheckIns.Add(ci);
            }
            else
            {
                existing.Mood = Mood; existing.Workload = Workload;
                existing.WhatWentWell = WhatWentWell; existing.Blockers = Blockers;
                existing.HelpNeeded = HelpNeeded; existing.GoalsNextWeek = GoalsNextWeek;
                existing.TasksDone = TasksDone; existing.CodeReviewsDone = CodeReviewsDone;
                existing.MeetingsCount = MeetingsCount; existing.MentoringHours = MentoringHours;
                existing.SkillCSharp = SkillCSharp; existing.SkillDotNet = SkillDotNet; existing.SkillSql = SkillSql;
                existing.SkillFrontend = SkillFrontend; existing.SkillCloud = SkillCloud;
                existing.UpdatedAt = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();
            return RedirectToPage("/Assignments/Details", new { id = AssignmentId });
        }
    }
}
