using Household.Web.Models.Services;

namespace Household.Web.Models.Mock
{
    public sealed class MockHistoryWriter : IHistoryWriter
    {
        public Task AppendAsync(long paymentId, string op, object snapshot,
                                string? reason, string user, CancellationToken ct)
            => Task.CompletedTask;
    }
}
