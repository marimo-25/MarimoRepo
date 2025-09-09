namespace Household.Web.Common.Dtos.Payments
{
    public sealed class CreatePaymentDto
    {
        public DateOnly PaymentDate { get; set; }
        public string StoreName { get; set; } = "";
        public decimal Amount { get; set; }
        public string Payer { get; set; } = "";
    }
    public sealed class PaymentDto
    {
        public long Id { get; set; }
        public DateOnly PaymentDate { get; set; }
        public string StoreName { get; set; } = "";
        public decimal Amount { get; set; }
        public string Payer { get; set; } = "";
        public string Status { get; set; } = "Pending";
        public bool IsActive { get; set; } = true;

        public static PaymentDto From(Models.Domain.Payment p) => new()
        {
            Id = p.Id, PaymentDate = p.PaymentDate, StoreName = p.StoreName,
            Amount = p.Amount, Payer = p.Payer, Status = p.Status.ToString(), IsActive = p.IsActive
        };
    }
    public sealed class PaymentQueryDto
    {
        public DateOnly? From { get; set; }
        public DateOnly? To { get; set; }
        public string? Store { get; set; }
        public string? Payer { get; set; }
        public string? Status { get; set; }
    }
    public sealed class CancelRequestDto
    {
        public string? Reason { get; set; }
    }
}