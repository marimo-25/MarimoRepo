namespace Household.Web.ViewModels.Dashboard
{
    public sealed class WeeklySummaryViewModel
    {
        public List<string> Labels { get; set; } = new();
        public List<decimal> Amounts { get; set; } = new();
        public List<DailySummaryViewModel> DailySummaries { get; set; } = new();
    }

    public sealed class DailySummaryViewModel
    {
        public string Date { get; set; } = string.Empty;
        public string DayOfWeek { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int TransactionCount { get; set; }
    }
}