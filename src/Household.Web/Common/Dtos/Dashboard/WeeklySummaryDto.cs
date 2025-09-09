namespace Household.Web.Common.Dtos.Dashboard
{
    public sealed class WeeklySummaryDto
    {
        public List<string> Labels { get; set; } = new();
        public List<decimal> Amounts { get; set; } = new();
    }
}