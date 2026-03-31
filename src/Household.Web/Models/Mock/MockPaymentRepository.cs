using Household.Web.Models.Data.Repositories;
using Household.Web.Models.Domain;

namespace Household.Web.Models.Mock
{
    public sealed class MockPaymentRepository : IPaymentRepository
    {
        private static long _nextId = 4;
        private static readonly List<Payment> _store =
        [
            new Payment(1, new DateOnly(2026, 3, 28), "スーパーマルエツ", 3200m, "Alice", PaymentStatus.Settled, true),
            new Payment(2, new DateOnly(2026, 3, 29), "コンビニ", 580m,   "Bob",   PaymentStatus.Pending,  true),
            new Payment(3, new DateOnly(2026, 3, 31), "ドラッグストア",  1450m, "Alice", PaymentStatus.Pending,  true),
        ];

        public Task<long> InsertAsync(Payment entity, CancellationToken ct)
        {
            var id = Interlocked.Increment(ref _nextId);
            var p = new Payment(id, entity.PaymentDate, entity.StoreName, entity.Amount,
                                entity.Payer, entity.Status, entity.IsActive);
            _store.Add(p);
            return Task.FromResult(id);
        }

        public Task<Payment?> GetAsync(long id, CancellationToken ct)
            => Task.FromResult(_store.FirstOrDefault(p => p.Id == id));

        public Task<List<Payment>> QueryAsync(DateOnly? from, DateOnly? to, string? store,
                                              string? payer, string? status, CancellationToken ct)
        {
            var q = _store.AsEnumerable();
            if (from.HasValue)  q = q.Where(p => p.PaymentDate >= from.Value);
            if (to.HasValue)    q = q.Where(p => p.PaymentDate <= to.Value);
            if (!string.IsNullOrEmpty(store))  q = q.Where(p => p.StoreName.Contains(store));
            if (!string.IsNullOrEmpty(payer))  q = q.Where(p => p.Payer == payer);
            if (!string.IsNullOrEmpty(status)) q = q.Where(p => p.Status.ToString() == status);
            return Task.FromResult(q.OrderByDescending(p => p.PaymentDate).ThenByDescending(p => p.Id).ToList());
        }

        public Task UpdateAsync(Payment entity, byte[]? concurrencyToken, CancellationToken ct)
        {
            var idx = _store.FindIndex(p => p.Id == entity.Id);
            if (idx < 0) throw new System.Data.DBConcurrencyException("Not found.");
            _store[idx] = entity;
            return Task.CompletedTask;
        }
    }
}
