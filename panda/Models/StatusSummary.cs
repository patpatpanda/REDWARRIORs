namespace panda.Models
{
    public enum RiskLevel { Green, Amber, Red }

    public class StatusSummary
    {
        public RiskLevel Level { get; set; } = RiskLevel.Green;
        public List<string> Reasons { get; set; } = new();
        public List<string> Suggestions { get; set; } = new();
        public DateTime WeekStart { get; set; }
    }

}
