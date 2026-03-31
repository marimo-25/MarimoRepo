using Household.Web.Common.Dtos.Dashboard;
using Household.Web.Models.Services;

namespace Household.Web.Models.Mock
{
    public sealed class MockDashboardQuery : IDashboardQuery
    {
        public Task<WeeklySummaryDto> GetWeeklyAsync(CancellationToken ct)
        {
            var today = DateTime.UtcNow.Date;
            var start = today.AddDays(-((int)today.DayOfWeek));

            var amounts = new[] { 3200m, 580m, 0m, 1450m, 0m, 2100m, 0m };
            var labels = Enumerable.Range(0, 7)
                .Select(i => start.AddDays(i).ToString("MM/dd"))
                .ToList();

            return Task.FromResult(new WeeklySummaryDto
            {
                Labels = labels,
                Amounts = amounts.Take(7).ToList()
            });
        }
    }
}
