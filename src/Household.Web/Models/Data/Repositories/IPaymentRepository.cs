using Household.Web.Models.Domain;

namespace Household.Web.Models.Data.Repositories
{
    public interface IPaymentRepository
    {
        Task<long> InsertAsync(Payment entity, CancellationToken ct);
        Task<Payment?> GetAsync(long id, CancellationToken ct);
        Task<List<Payment>> QueryAsync(DateOnly? from, DateOnly? to, string? store, string? payer, string? status, CancellationToken ct);
        Task UpdateAsync(Payment entity, byte[]? concurrencyToken, CancellationToken ct);
    }
}