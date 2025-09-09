namespace Household.Web.Models.Domain
{
    public enum PaymentStatus { Pending, Settled, Cancelled }

    public sealed class Payment
    {
        public long Id { get; private set; }
        public DateOnly PaymentDate { get; private set; }
        public string StoreName { get; private set; }
        public decimal Amount { get; private set; }
        public string Payer { get; private set; }
        public PaymentStatus Status { get; private set; } = PaymentStatus.Pending;
        public bool IsActive { get; private set; } = true;

        public Payment(long id, DateOnly date, string store, decimal amount, string payer, PaymentStatus status, bool isActive)
        { Id = id; PaymentDate = date; StoreName = store; Amount = amount; Payer = payer; Status = status; IsActive = isActive; }

        public void Settle()
        {
            if (Status == PaymentStatus.Cancelled) throw new InvalidOperationException("Cancelled cannot be settled.");
            Status = PaymentStatus.Settled;
        }

        public void Cancel()
        {
            if (Status == PaymentStatus.Settled) throw new InvalidOperationException("Settled cannot be cancelled.");
            Status = PaymentStatus.Cancelled; IsActive = false;
        }
    }
}