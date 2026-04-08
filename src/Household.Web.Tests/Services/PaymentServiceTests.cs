using Household.Web.Common.Dtos.Payments;
using Xunit;
using Household.Web.Models.Data.Repositories;
using Household.Web.Models.Domain;
using Household.Web.Models.Services;
using Moq;

namespace Household.Web.Tests.Services;

public class PaymentServiceTests
{
    private readonly Mock<IPaymentRepository> _repo = new();
    private readonly Mock<IHistoryWriter> _history = new();
    private readonly PaymentService _sut;

    public PaymentServiceTests()
    {
        _sut = new PaymentService(_repo.Object, _history.Object);

        _history
            .Setup(h => h.AppendAsync(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<object>(),
                It.IsAny<string?>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }

    // ── CreateAsync ─────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_ReturnsIdFromRepository()
    {
        var dto = new CreatePaymentDto { PaymentDate = new DateOnly(2026, 4, 1), StoreName = "スーパー", Amount = 1000m, Payer = "テスト太郎" };
        _repo.Setup(r => r.InsertAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>())).ReturnsAsync(99L);

        var id = await _sut.CreateAsync(dto, "テスト太郎", CancellationToken.None);

        Assert.Equal(99L, id);
    }

    [Fact]
    public async Task CreateAsync_InsertsPaymentWithPendingStatus()
    {
        var dto = new CreatePaymentDto { PaymentDate = new DateOnly(2026, 4, 1), StoreName = "スーパー", Amount = 1000m, Payer = "テスト太郎" };
        Payment? captured = null;
        _repo.Setup(r => r.InsertAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()))
             .Callback<Payment, CancellationToken>((p, _) => captured = p)
             .ReturnsAsync(1L);

        await _sut.CreateAsync(dto, "テスト太郎", CancellationToken.None);

        Assert.NotNull(captured);
        Assert.Equal(PaymentStatus.Pending, captured.Status);
        Assert.True(captured.IsActive);
        Assert.Equal(0, captured.Id);
        Assert.Equal("スーパー", captured.StoreName);
        Assert.Equal(1000m, captured.Amount);
        Assert.Equal("テスト太郎", captured.Payer);
    }

    [Fact]
    public async Task CreateAsync_WritesCreateHistory()
    {
        var dto = new CreatePaymentDto { PaymentDate = new DateOnly(2026, 4, 1), StoreName = "スーパー", Amount = 500m, Payer = "テスト花子" };
        _repo.Setup(r => r.InsertAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>())).ReturnsAsync(7L);

        await _sut.CreateAsync(dto, "テスト花子", CancellationToken.None);

        _history.Verify(h => h.AppendAsync(7L, "Create", It.IsAny<object>(), null, "テスト花子", It.IsAny<CancellationToken>()), Times.Once);
    }

    // ── ListAsync ───────────────────────────────────────────

    [Fact]
    public async Task ListAsync_ReturnsMappedDtos()
    {
        var payments = new List<Payment>
        {
            new(1, new DateOnly(2026, 4, 1), "スーパー", 1000m, "テスト太郎", PaymentStatus.Pending, true),
            new(2, new DateOnly(2026, 4, 2), "コンビニ", 500m, "テスト花子", PaymentStatus.Settled, true),
        };
        _repo.Setup(r => r.QueryAsync(It.IsAny<DateOnly?>(), It.IsAny<DateOnly?>(), It.IsAny<string?>(),
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(payments);

        var result = await _sut.ListAsync(new PaymentQueryDto(), CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].Id);
        Assert.Equal("Pending", result[0].Status);
        Assert.Equal(2, result[1].Id);
        Assert.Equal("Settled", result[1].Status);
    }

    [Fact]
    public async Task ListAsync_PassesQueryParametersToRepository()
    {
        var query = new PaymentQueryDto
        {
            From = new DateOnly(2026, 4, 1),
            To = new DateOnly(2026, 4, 30),
            Store = "スーパー",
            Payer = "テスト太郎",
            Status = "Pending"
        };
        _repo.Setup(r => r.QueryAsync(It.IsAny<DateOnly?>(), It.IsAny<DateOnly?>(), It.IsAny<string?>(),
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(new List<Payment>());

        await _sut.ListAsync(query, CancellationToken.None);

        _repo.Verify(r => r.QueryAsync(
            new DateOnly(2026, 4, 1),
            new DateOnly(2026, 4, 30),
            "スーパー",
            "テスト太郎",
            "Pending",
            It.IsAny<CancellationToken>()), Times.Once);
    }

    // ── SettleAsync ─────────────────────────────────────────

    [Fact]
    public async Task SettleAsync_WhenPaymentNotFound_ThrowsKeyNotFoundException()
    {
        _repo.Setup(r => r.GetAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((Payment?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.SettleAsync(99, "テスト太郎", null, CancellationToken.None));
    }

    [Fact]
    public async Task SettleAsync_CallsUpdateAndHistory()
    {
        var payment = new Payment(1, new DateOnly(2026, 4, 1), "スーパー", 1000m, "テスト太郎", PaymentStatus.Pending, true);
        _repo.Setup(r => r.GetAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(payment);
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Payment>(), It.IsAny<byte[]?>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        await _sut.SettleAsync(1, "テスト太郎", null, CancellationToken.None);

        Assert.Equal(PaymentStatus.Settled, payment.Status);
        _repo.Verify(r => r.UpdateAsync(payment, null, It.IsAny<CancellationToken>()), Times.Once);
        _history.Verify(h => h.AppendAsync(1, "Settle", It.IsAny<object>(), null, "テスト太郎", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SettleAsync_PassesConcurrencyTokenToRepository()
    {
        var token = new byte[] { 1, 2, 3 };
        var payment = new Payment(1, new DateOnly(2026, 4, 1), "スーパー", 1000m, "テスト太郎", PaymentStatus.Pending, true);
        _repo.Setup(r => r.GetAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(payment);
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Payment>(), It.IsAny<byte[]?>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        await _sut.SettleAsync(1, "テスト太郎", token, CancellationToken.None);

        _repo.Verify(r => r.UpdateAsync(payment, token, It.IsAny<CancellationToken>()), Times.Once);
    }

    // ── CancelAsync ─────────────────────────────────────────

    [Fact]
    public async Task CancelAsync_WhenPaymentNotFound_ThrowsKeyNotFoundException()
    {
        _repo.Setup(r => r.GetAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((Payment?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.CancelAsync(99, "返品", "テスト太郎", null, CancellationToken.None));
    }

    [Fact]
    public async Task CancelAsync_CallsUpdateAndHistory()
    {
        var payment = new Payment(1, new DateOnly(2026, 4, 1), "スーパー", 1000m, "テスト太郎", PaymentStatus.Pending, true);
        _repo.Setup(r => r.GetAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(payment);
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Payment>(), It.IsAny<byte[]?>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        await _sut.CancelAsync(1, "返品", "テスト太郎", null, CancellationToken.None);

        Assert.Equal(PaymentStatus.Cancelled, payment.Status);
        Assert.False(payment.IsActive);
        _repo.Verify(r => r.UpdateAsync(payment, null, It.IsAny<CancellationToken>()), Times.Once);
        _history.Verify(h => h.AppendAsync(1, "Cancel", It.IsAny<object>(), "返品", "テスト太郎", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CancelAsync_PassesReasonToHistory()
    {
        var payment = new Payment(1, new DateOnly(2026, 4, 1), "スーパー", 1000m, "テスト太郎", PaymentStatus.Pending, true);
        _repo.Setup(r => r.GetAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(payment);
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Payment>(), It.IsAny<byte[]?>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        await _sut.CancelAsync(1, "間違い購入", "テスト花子", null, CancellationToken.None);

        _history.Verify(h => h.AppendAsync(1, "Cancel", It.IsAny<object>(), "間違い購入", "テスト花子", It.IsAny<CancellationToken>()), Times.Once);
    }
}
