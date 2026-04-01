using Household.Web.Models.Domain;
using Xunit;

namespace Household.Web.Tests.Domain;

public class PaymentTests
{
    private static Payment CreatePayment(PaymentStatus status = PaymentStatus.Pending, bool isActive = true) =>
        new(1, new DateOnly(2026, 4, 1), "テスト店舗", 1000m, "Alice", status, isActive);

    // ── Settle ─────────────────────────────────────────────

    [Fact]
    public void Settle_WhenPending_StatusBecomesSettled()
    {
        var payment = CreatePayment(PaymentStatus.Pending);
        payment.Settle();
        Assert.Equal(PaymentStatus.Settled, payment.Status);
    }

    [Fact]
    public void Settle_WhenPending_IsActiveRemainsTrue()
    {
        var payment = CreatePayment(PaymentStatus.Pending);
        payment.Settle();
        Assert.True(payment.IsActive);
    }

    [Fact]
    public void Settle_WhenCancelled_ThrowsInvalidOperationException()
    {
        var payment = CreatePayment(PaymentStatus.Cancelled);
        var ex = Assert.Throws<InvalidOperationException>(() => payment.Settle());
        Assert.Equal("Cancelled cannot be settled.", ex.Message);
    }

    [Fact]
    public void Settle_WhenAlreadySettled_StatusRemainsSettled()
    {
        var payment = CreatePayment(PaymentStatus.Settled);
        payment.Settle();
        Assert.Equal(PaymentStatus.Settled, payment.Status);
    }

    // ── Cancel ─────────────────────────────────────────────

    [Fact]
    public void Cancel_WhenPending_StatusBecomesCancelled()
    {
        var payment = CreatePayment(PaymentStatus.Pending);
        payment.Cancel();
        Assert.Equal(PaymentStatus.Cancelled, payment.Status);
    }

    [Fact]
    public void Cancel_WhenPending_IsActiveBecomeFalse()
    {
        var payment = CreatePayment(PaymentStatus.Pending);
        payment.Cancel();
        Assert.False(payment.IsActive);
    }

    [Fact]
    public void Cancel_WhenSettled_ThrowsInvalidOperationException()
    {
        var payment = CreatePayment(PaymentStatus.Settled);
        var ex = Assert.Throws<InvalidOperationException>(() => payment.Cancel());
        Assert.Equal("Settled cannot be cancelled.", ex.Message);
    }

    // ── コンストラクタ ──────────────────────────────────────

    [Fact]
    public void Constructor_SetsAllPropertiesCorrectly()
    {
        var date = new DateOnly(2026, 4, 1);
        var payment = new Payment(42, date, "スーパー", 3200m, "Bob", PaymentStatus.Pending, true);

        Assert.Equal(42, payment.Id);
        Assert.Equal(date, payment.PaymentDate);
        Assert.Equal("スーパー", payment.StoreName);
        Assert.Equal(3200m, payment.Amount);
        Assert.Equal("Bob", payment.Payer);
        Assert.Equal(PaymentStatus.Pending, payment.Status);
        Assert.True(payment.IsActive);
    }
}
