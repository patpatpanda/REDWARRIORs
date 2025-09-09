using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;      // <-- Viktigt för Include/FirstOrDefaultAsync
using panda.Models;
using Panda.Data;                          // <-- Din DbContext
using Panda.Models;                        // <-- AppUser, Assignment, MentorCheckIn
using System;

namespace Panda.Pages.Mentor
{
    [Authorize(Roles = "Mentor")]
    public class CheckInModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<AppUser> _userManager;

        public CheckInModel(ApplicationDbContext db, UserManager<AppUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public Assignment Assignment { get; set; } = default!;

        [BindProperty] public int AssignmentId { get; set; }
        [BindProperty] public DateTime WeekStart { get; set; }

        [BindProperty] public MentorAssessment Assessment { get; set; } = MentorAssessment.OK;
        [BindProperty] public int Performance { get; set; } = 3;
        [BindProperty] public int Collaboration { get; set; } = 3;
        [BindProperty] public int Pace { get; set; } = 3;

        [BindProperty] public string Observations { get; set; } = "";
        [BindProperty] public string Recommendations { get; set; } = "";
        [BindProperty] public string Risks { get; set; } = "";

        private static DateTime StartOfWeek(DateTime date, DayOfWeek start = DayOfWeek.Monday)
        {
            int diff = (7 + (date.DayOfWeek - start)) % 7;
            return date.Date.AddDays(-diff);
        }

        public async Task<IActionResult> OnGetAsync(int assignmentId)
        {
            var mentor = await _userManager.GetUserAsync(User);

            Assignment = await _db.Assignments
                .Include(a => a.Talent)
                .Include(a => a.Employer)
                .FirstOrDefaultAsync(a => a.Id == assignmentId);

            if (Assignment == null || Assignment.MentorId != mentor!.Id)
                return Forbid();

            AssignmentId = assignmentId;
            WeekStart = StartOfWeek(DateTime.UtcNow);

            var existing = await _db.MentorCheckIns
                .FirstOrDefaultAsync(x => x.AssignmentId == AssignmentId
                                        && x.MentorId == mentor.Id
                                        && x.WeekStart == WeekStart);

            if (existing != null)
            {
                Assessment = existing.Assessment;
                Performance = existing.Performance;
                Collaboration = existing.Collaboration;
                Pace = existing.Pace;
                Observations = existing.Observations;
                Recommendations = existing.Recommendations;
                Risks = existing.Risks;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var mentor = await _userManager.GetUserAsync(User);
            var assignment = await _db.Assignments.FindAsync(AssignmentId);
            if (assignment == null || assignment.MentorId != mentor!.Id) return Forbid();

            WeekStart = StartOfWeek(WeekStart == default ? DateTime.UtcNow : WeekStart);

            var existing = await _db.MentorCheckIns
                .FirstOrDefaultAsync(x => x.AssignmentId == AssignmentId
                                        && x.MentorId == mentor.Id
                                        && x.WeekStart == WeekStart);

            if (existing == null)
            {
                _db.MentorCheckIns.Add(new MentorCheckIn
                {
                    AssignmentId = AssignmentId,
                    MentorId = mentor.Id,
                    WeekStart = WeekStart,
                    Assessment = Assessment,
                    Performance = Performance,
                    Collaboration = Collaboration,
                    Pace = Pace,
                    Observations = Observations,
                    Recommendations = Recommendations,
                    Risks = Risks
                });
            }
            else
            {
                existing.Assessment = Assessment;
                existing.Performance = Performance;
                existing.Collaboration = Collaboration;
                existing.Pace = Pace;
                existing.Observations = Observations;
                existing.Recommendations = Recommendations;
                existing.Risks = Risks;
                existing.UpdatedAt = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();
            return RedirectToPage("/Mentor/Index");

        }
    }
}
