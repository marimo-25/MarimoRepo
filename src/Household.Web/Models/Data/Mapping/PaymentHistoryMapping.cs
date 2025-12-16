using LinqToDB.Mapping;

namespace Household.Web.Models.Data.Mapping
{
    [Table(Name = "PaymentHistories", Schema = "dbo")]
    public class PaymentHistoryRecord
    {
        [PrimaryKey, Identity] public long Id { get; set; }
        [Column, NotNull] public long PaymentId { get; set; }
        [Column, NotNull] public string Operation { get; set; } = "";
        [Column, NotNull] public string SnapshotJson { get; set; } = "";
        [Column] public string? Reason { get; set; }
        [Column] public string? Comment { get; set; }
        [Column, NotNull] public string PerformedBy { get; set; } = "";
        [Column, NotNull] public DateTime PerformedAt { get; set; }
    }
}