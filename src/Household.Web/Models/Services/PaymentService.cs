using Household.Web.Common.Dtos.Payments;
using Household.Web.Models.Data.Repositories;
using Household.Web.Models.Domain;

namespace Household.Web.Models.Services
{
    public sealed class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _repo;
        private readonly IHistoryWriter _history;

        public PaymentService(IPaymentRepository repo, IHistoryWriter history)
        { _repo = repo; _history = history; }

        public async Task<long> CreateAsync(CreatePaymentDto dto, string user, CancellationToken ct)
        {
            var p = new Payment(0, dto.PaymentDate, dto.StoreName, dto.Amount, dto.Payer, PaymentStatus.Pending, true);
            var id = await _repo.InsertAsync(p, ct);
            await _history.AppendAsync(id, "Create", dto, null, user, ct);
            return id;
        }

        public async Task<List<PaymentDto>> ListAsync(PaymentQueryDto q, CancellationToken ct)
        {
            var list = await _repo.QueryAsync(q.From, q.To, q.Store, q.Payer, q.Status, ct);
            return list.Select(PaymentDto.From).ToList();
        }

        public async Task SettleAsync(long id, string user, byte[]? concurrency, CancellationToken ct)
        {
            var p = await _repo.GetAsync(id, ct) ?? throw new KeyNotFoundException();
            p.Settle();
            await _repo.UpdateAsync(p, concurrency, ct);
            await _history.AppendAsync(id, "Settle", PaymentDto.From(p), null, user, ct);
        }

        public async Task CancelAsync(long id, string reason, string user, byte[]? concurrency, CancellationToken ct)
        {
            var p = await _repo.GetAsync(id, ct) ?? throw new KeyNotFoundException();
            p.Cancel();
            await _repo.UpdateAsync(p, concurrency, ct);
            await _history.AppendAsync(id, "Cancel", PaymentDto.From(p), reason, user, ct);
        }
    }
}