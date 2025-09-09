using Household.Web.Models.Data.Mapping;
using LinqToDB;

namespace Household.Web.Models.Services
{
    public sealed class HistoryWriter : IHistoryWriter
    {
        private readonly Household.Web.Models.Data.HouseholdDb _db;
        public HistoryWriter(Household.Web.Models.Data.HouseholdDb db) => _db = db;

        public Task AppendAsync(long paymentId, string op, object snapshot, string? reason, string user, CancellationToken ct)
        {
            var rec = new PaymentHistoryRecord {
                PaymentId = paymentId, Operation = op,
                SnapshotJson = System.Text.Json.JsonSerializer.Serialize(snapshot),
                Reason = reason, PerformedBy = user, PerformedAt = DateTime.UtcNow
            };
            return _db.InsertAsync(rec, token: ct);
        }
    }
}