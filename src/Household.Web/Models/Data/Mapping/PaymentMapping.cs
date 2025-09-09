using LinqToDB.Mapping;

namespace Household.Web.Models.Data.Mapping
{
    [Table(Name = "Payments", Schema = "dbo")]
    public class PaymentRecord
    {
        [PrimaryKey, Identity] public long Id { get; set; }
        [Column, NotNull] public DateTime PaymentDate { get; set; }
        [Column, NotNull] public string StoreName { get; set; } = "";
        [Column, NotNull] public decimal Amount { get; set; }
        [Column, NotNull] public string Payer { get; set; } = "";
        [Column, NotNull] public string Status { get; set; } = "Pending";
        [Column, NotNull] public bool IsActive { get; set; } = true;
        [Column, NotNull] public DateTime CreatedAt { get; set; }
        [Column, NotNull] public DateTime UpdatedAt { get; set; }
        [Column(DataType=DataType.VarBinary)] public byte[]? ConcurrencyToken { get; set; }
    }
}