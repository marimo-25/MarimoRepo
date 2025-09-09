using LinqToDB;
using Household.Web.Models.Domain;
using Household.Web.Models.Data.Mapping;

namespace Household.Web.Models.Data.Repositories
{
    public sealed class PaymentRepository : IPaymentRepository
    {
        private readonly HouseholdDb _db;
        public PaymentRepository(HouseholdDb db) => _db = db;

        public async Task<long> InsertAsync(Payment e, CancellationToken ct)
        {
            var now = DateTime.UtcNow;
            var id = await _db.InsertWithInt64IdentityAsync(new PaymentRecord {
                PaymentDate = e.PaymentDate.ToDateTime(TimeOnly.MinValue),
                StoreName = e.StoreName, Amount = e.Amount, Payer = e.Payer,
                Status = e.Status.ToString(), IsActive = e.IsActive, CreatedAt = now, UpdatedAt = now
            }, token: ct);
            return id;
        }

        public async Task<Payment?> GetAsync(long id, CancellationToken ct)
        {
            var r = await _db.Payments.FirstOrDefaultAsync(p => p.Id == id, ct);
            return r == null ? null : new Payment(r.Id, DateOnly.FromDateTime(r.PaymentDate), r.StoreName, r.Amount, r.Payer, Enum.Parse<PaymentStatus>(r.Status), r.IsActive);
        }

        public async Task<List<Payment>> QueryAsync(DateOnly? from, DateOnly? to, string? store, string? payer, string? status, CancellationToken ct)
        {
            var q = _db.Payments.Where(p => true);
            if (from.HasValue) q = q.Where(p => p.PaymentDate >= from.Value.ToDateTime(TimeOnly.MinValue));
            if (to.HasValue) q = q.Where(p => p.PaymentDate < to.Value.AddDays(1).ToDateTime(TimeOnly.MinValue));
            if (!string.IsNullOrEmpty(store)) q = q.Where(p => p.StoreName.Contains(store));
            if (!string.IsNullOrEmpty(payer)) q = q.Where(p => p.Payer == payer);
            if (!string.IsNullOrEmpty(status)) q = q.Where(p => p.Status == status);

            var list = await q.OrderByDescending(p => p.PaymentDate).ThenByDescending(p => p.Id).ToListAsync(ct);
            return list.Select(r => new Payment(r.Id, DateOnly.FromDateTime(r.PaymentDate), r.StoreName, r.Amount, r.Payer, Enum.Parse<PaymentStatus>(r.Status), r.IsActive)).ToList();
        }

        public async Task UpdateAsync(Payment e, byte[]? concurrency, CancellationToken ct)
        {
            var now = DateTime.UtcNow;
            var upd = _db.Payments
                .Where(p => p.Id == e.Id && (concurrency == null || p.ConcurrencyToken == concurrency))
                .Set(p => p.Status, e.Status.ToString())
                .Set(p => p.IsActive, e.IsActive)
                .Set(p => p.UpdatedAt, now);

            var count = await upd.UpdateAsync(ct);
            if (count == 0) throw new DBConcurrencyException("Optimistic concurrency violation.");
        }
    }
}