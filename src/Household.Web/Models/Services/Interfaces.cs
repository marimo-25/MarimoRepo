using Household.Web.Common.Dtos.Payments;
using Household.Web.Common.Dtos.Dashboard;
using Household.Web.Models.Domain;

namespace Household.Web.Models.Services
{
    public interface IPaymentService
    {
        Task<long> CreateAsync(CreatePaymentDto dto, string user, CancellationToken ct);
        Task<List<PaymentDto>> ListAsync(PaymentQueryDto q, CancellationToken ct);
        Task SettleAsync(long id, string user, byte[]? concurrency, CancellationToken ct);
        Task CancelAsync(long id, string reason, string user, byte[]? concurrency, CancellationToken ct);
    }

    public interface IDashboardQuery
    {
        Task<WeeklySummaryDto> GetWeeklyAsync(CancellationToken ct);
    }

    public interface IHistoryWriter
    {
        Task AppendAsync(long paymentId, string op, object snapshot, string? reason, string user, CancellationToken ct);
    }

    public interface IApiCallLogger
    {
        Task LogAsync(object log, CancellationToken ct);
    }
}