using Household.Web.Common.Dtos.Payments;
using Xunit;
using Household.Web.Models.Domain;

namespace Household.Web.Tests.Dtos;

public class PaymentDtoTests
{
    [Fact]
    public void From_MapsAllFieldsCorrectly()
    {
        var date = new DateOnly(2026, 4, 1);
        var payment = new Payment(42, date, "スーパー", 3200m, "Alice", PaymentStatus.Settled, true);

        var dto = PaymentDto.From(payment);

        Assert.Equal(42, dto.Id);
        Assert.Equal(date, dto.PaymentDate);
        Assert.Equal("スーパー", dto.StoreName);
        Assert.Equal(3200m, dto.Amount);
        Assert.Equal("Alice", dto.Payer);
        Assert.Equal("Settled", dto.Status);
        Assert.True(dto.IsActive);
    }

    [Theory]
    [InlineData(PaymentStatus.Pending, "Pending")]
    [InlineData(PaymentStatus.Settled, "Settled")]
    [InlineData(PaymentStatus.Cancelled, "Cancelled")]
    public void From_StringifiesStatusCorrectly(PaymentStatus status, string expected)
    {
        var payment = new Payment(1, new DateOnly(2026, 4, 1), "店舗", 100m, "Bob", status, true);

        var dto = PaymentDto.From(payment);

        Assert.Equal(expected, dto.Status);
    }

    [Fact]
    public void From_WhenCancelled_IsActiveFalse()
    {
        var payment = new Payment(1, new DateOnly(2026, 4, 1), "店舗", 100m, "Bob", PaymentStatus.Pending, true);
        payment.Cancel();

        var dto = PaymentDto.From(payment);

        Assert.False(dto.IsActive);
        Assert.Equal("Cancelled", dto.Status);
    }
}
