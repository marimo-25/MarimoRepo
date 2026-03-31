using Household.Web.Models.Services;

namespace Household.Web.Models.Mock
{
    public sealed class NullApiCallLogger : IApiCallLogger
    {
        public Task LogAsync(object log, CancellationToken ct) => Task.CompletedTask;
    }
}
