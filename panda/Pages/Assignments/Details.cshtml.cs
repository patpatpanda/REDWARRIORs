using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using panda.Models;
using panda.Pages;
using panda.Services;
using Panda.Data;
using Panda.Models;

namespace Panda.Pages.Assignments
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<AppUser> _userManager;

        public DetailsModel(ApplicationDbContext db, UserManager<AppUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public Assignment Assignment { get; set; } = default!;
        public IList<MentorCheckIn> RecentMentorCheckIns { get; set; } = new List<MentorCheckIn>();
        public MentorCheckIn? LatestMentorCheckIn { get; set; }
        // Översikt
        public TalentCheckIn? LatestCheckIn { get; set; }
        public StatusSummary Summary { get; set; } = new();
        public IList<TalentCheckIn> RecentCheckIns { get; set; } = new List<TalentCheckIn>(); // t.ex. senaste 8 veckor

        // Talangens inmatningsfält (finns redan)
        [BindProperty] public string Message { get; set; } = "";
        [BindProperty] public string Category { get; set; } = "Arbete";
        [BindProperty] public int? Rating { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            Assignment = await _db.Assignments
                .Include(a => a.Employer)
                .Include(a => a.Talent)
                .Include(a => a.Mentor)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (Assignment == null ||
                (user.Id != Assignment.EmployerId &&
                 user.Id != Assignment.TalentId &&
                 user.Id != Assignment.MentorId))
            {
                return Forbid();
            }

            // Hämta senaste rapporter för Talang och Mentor
            RecentCheckIns = await _db.TalentCheckIns
                .Where(ci => ci.AssignmentId == id && ci.TalentId == Assignment.TalentId)
                .OrderByDescending(ci => ci.WeekStart)
                .Take(8)
                .ToListAsync();

            RecentMentorCheckIns = await _db.MentorCheckIns
                .Where(mi => mi.AssignmentId == id && mi.MentorId == Assignment.MentorId)
                .OrderByDescending(mi => mi.WeekStart)
                .Take(8)
                .ToListAsync();

            // hitta senaste vecka där båda har en rapport
            var commonWeek = RecentCheckIns
                .Select(ci => ci.WeekStart)
                .Intersect(RecentMentorCheckIns.Select(mi => mi.WeekStart))
                .OrderByDescending(d => d)
                .FirstOrDefault();

            TalentCheckIn? latestTalent = null;
            MentorCheckIn? latestMentor = null;

            if (commonWeek != default)
            {
                latestTalent = RecentCheckIns.FirstOrDefault(ci => ci.WeekStart == commonWeek);
                latestMentor = RecentMentorCheckIns.FirstOrDefault(mi => mi.WeekStart == commonWeek);
            }

            LatestCheckIn = latestTalent;
            LatestMentorCheckIn = latestMentor;

            // Sammanfattning
            Summary = AssessmentService.Compare(latestTalent, latestMentor, RecentCheckIns);

            return Page();
        }


        public async Task<IActionResult> OnPostAsync(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var assignment = await _db.Assignments.FindAsync(id);
            if (assignment == null) return NotFound();

            // befintlig fria-meddelanden feedback (behåll om du använder detta)
            if (!string.IsNullOrWhiteSpace(Message) || Rating.HasValue || !string.IsNullOrWhiteSpace(Category))
            {
                var entry = new FeedbackEntry
                {
                    AssignmentId = assignment.Id,
                    AuthorId = user.Id,
                    Category = Category,
                    Rating = Rating,
                    Message = Message
                };
                _db.FeedbackEntries.Add(entry);
                await _db.SaveChangesAsync();
            }

            return RedirectToPage(new { id = assignment.Id });
        }
        public string GetWeekStatus(DateTime weekStart)
        {
            var hasTalent = RecentCheckIns.Any(ci => ci.WeekStart == weekStart);
            var hasMentor = RecentMentorCheckIns.Any(mi => mi.WeekStart == weekStart);

            if (hasTalent && hasMentor)
                return "Båda rapporterat";
            if (hasTalent && !hasMentor)
                return "Mentor ej fyllt i";
            if (!hasTalent && hasMentor)
                return "Talang ej fyllt i";

            return "Ingen rapport"; // borde inte hända om listorna laddas korrekt
        }

    }
}
