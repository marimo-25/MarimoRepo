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
                .Select(g => new { Date = g.Key, Total = g.Sum(x => x.Amount) })
                .ToListAsync(ct);

            var labels = Enumerable.Range(0,7).Select(i => start.AddDays(i).ToString("MM/dd")).ToList();
            var amounts = labels.Select(label => {
                var d = DateTime.Parse(label);
                return data.FirstOrDefault(x => x.Date.Date == d.Date)?.Total ?? 0m;
            }).ToList();

            return new WeeklySummaryDto { Labels = labels, Amounts = amounts };
        }
    }
}