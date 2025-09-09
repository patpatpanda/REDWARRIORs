using panda.Models;

namespace panda.Services
{
    public static class AssessmentService
    {
        private const int BigGap = 2; // skillnad på 2 steg = stor avvikelse
        private const int LowTasks = 1;

        public static StatusSummary Compare(
            TalentCheckIn? talent,
            MentorCheckIn? mentor,
            IEnumerable<TalentCheckIn> talentTrend)
        {
            var summary = new StatusSummary
            {
                WeekStart = talent?.WeekStart ?? mentor?.WeekStart ?? DateTime.UtcNow
            };

            // Om ingen av dem har rapporterat
            if (talent is null && mentor is null)
            {
                summary.Level = RiskLevel.Amber;
                summary.Reasons.Add("Varken Talang eller Mentor har lämnat in en rapport denna vecka.");
                summary.Suggestions.Add("Påminn båda parter att lämna veckorapport.");
                return summary;
            }

            // Om bara talangen har rapporterat
            if (talent is not null && mentor is null)
            {
                summary.Level = RiskLevel.Amber;
                summary.Reasons.Add("Mentor saknar rapport denna vecka.");
                summary.Suggestions.Add("Be mentor fylla i sin veckorapport.");
                return summary;
            }

            // Om bara mentorn har rapporterat
            if (mentor is not null && talent is null)
            {
                summary.Level = RiskLevel.Amber;
                summary.Reasons.Add("Talangen saknar rapport denna vecka.");
                summary.Suggestions.Add("Be talangen fylla i sin veckorapport.");
                return summary;
            }

            // --- Härifrån vet vi att både Talent och Mentor finns ---
            summary.Level = RiskLevel.Green;

            var mood = (int)talent!.Mood;
            var assess = (int)mentor!.Assessment;

            // Stora skillnader mellan Talang och Mentor
            if (Math.Abs(mood - assess) >= BigGap)
            {
                summary.Level = RiskLevel.Amber;
                summary.Reasons.Add("Stor skillnad mellan Talangens humör och Mentorns helhetsbedömning.");
                summary.Suggestions.Add("Ta ett gemensamt samtal för att stämma av upplevelser.");
            }

            // Belastning från Talang
            if (talent.Workload == Workload.Over)
            {
                summary.Level = Max(summary.Level, RiskLevel.Amber);
                summary.Reasons.Add("Talangen rapporterar hög arbetsbelastning.");
                summary.Suggestions.Add("Diskutera scope och prioriteringar för veckan.");
            }

            // Låg output + hinder
            if (talent.TasksDone <= LowTasks && !string.IsNullOrWhiteSpace(talent.Blockers))
            {
                summary.Level = Max(summary.Level, RiskLevel.Amber);
                summary.Reasons.Add("Talangen rapporterar lågt antal slutförda uppgifter och hinder.");
                summary.Suggestions.Add("Identifiera blockare och undanröj dem.");
            }

            // Mentor flaggar risker
            if (!string.IsNullOrWhiteSpace(mentor.Risks))
            {
                summary.Level = Max(summary.Level, RiskLevel.Amber);
                summary.Reasons.Add("Mentor har flaggat för risker.");
                summary.Suggestions.Add("Planera åtgärder baserat på mentorens observationer.");
            }

            // Trend (talangens senaste 3 veckor)
            if (talentTrend != null && talentTrend.Count() >= 3)
            {
                var ordered = talentTrend.OrderByDescending(t => t.WeekStart).Take(3).ToList();
                var w3 = (int)ordered[0].Mood;
                var w2 = (int)ordered[1].Mood;
                var w1 = (int)ordered[2].Mood;

                if (w3 < w2 && w2 < w1)
                {
                    summary.Level = RiskLevel.Red;
                    summary.Reasons.Add("Talangens humör har sjunkit tre veckor i rad.");
                    summary.Suggestions.Add("Ta ett strukturerat uppföljningssamtal och se över stödinsatser.");
                }
            }

            return summary;
        }

        private static RiskLevel Max(RiskLevel a, RiskLevel b)
            => (RiskLevel)Math.Max((int)a, (int)b);
    }

}
