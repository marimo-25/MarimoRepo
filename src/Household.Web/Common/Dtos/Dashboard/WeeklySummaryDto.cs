namespace Household.Web.Common.Dtos.Dashboard
{
    public sealed class WeeklySummaryDto
    {
        public List<string> Labels { get; set; } = new();
        public List<decimal> Amounts { get; set; } = new();
        public List<DailySummaryDto> DailySummaries { get; set; } = new();
    }

    public sealed class DailySummaryDto
    {
        public string Date { get; set; } = string.Empty;
        public string DayOfWeek { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int TransactionCount { get; set; }
    }
}