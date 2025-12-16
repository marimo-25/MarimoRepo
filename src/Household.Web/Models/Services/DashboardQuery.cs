using Household.Web.Common.Dtos.Dashboard;
using LinqToDB;

namespace Household.Web.Models.Services
{
    public sealed class DashboardQuery : IDashboardQuery
    {
        private readonly Household.Web.Models.Data.HouseholdDb _db;
        public DashboardQuery(Household.Web.Models.Data.HouseholdDb db) => _db = db;

        public async Task<WeeklySummaryDto> GetWeeklyAsync(CancellationToken ct)
        {
            var today = DateTime.UtcNow.Date;
            var start = today.AddDays(-((int)today.DayOfWeek));
            var end = start.AddDays(7);

            var data = await _db.Payments
                .Where(p => p.PaymentDate >= start && p.PaymentDate < end && p.Status != "Cancelled")
                .GroupBy(p => p.PaymentDate)
                .Select(g => new { Date = g.Key, Total = g.Sum(x => x.Amount), Count = g.Count() })
                .ToListAsync(ct);

            var labels = Enumerable.Range(0, 7).Select(i => start.AddDays(i).ToString("MM/dd")).ToList();
            var amounts = labels.Select(label => {
                var d = DateTime.Parse(label);
                return data.FirstOrDefault(x => x.Date.Date == d.Date)?.Total ?? 0m;
            }).ToList();

            var dayOfWeekNames = new[] { "日", "月", "火", "水", "木", "金", "土" };
            var dailySummaries = Enumerable.Range(0, 7).Select(i =>
            {
                var date = start.AddDays(i);
                var dayData = data.FirstOrDefault(x => x.Date.Date == date.Date);
                return new DailySummaryDto
                {
                    Date = date.ToString("MM/dd"),
                    DayOfWeek = dayOfWeekNames[(int)date.DayOfWeek],
                    Amount = dayData?.Total ?? 0m,
                    TransactionCount = dayData?.Count ?? 0
                };
            }).ToList();

            return new WeeklySummaryDto 
            { 
                Labels = labels, 
                Amounts = amounts,
                DailySummaries = dailySummaries
            };
        }
    }
}