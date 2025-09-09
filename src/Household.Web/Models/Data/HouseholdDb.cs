using LinqToDB;
using LinqToDB.Data;

namespace Household.Web.Models.Data
{
    public sealed class HouseholdDb : DataConnection
    {
        public HouseholdDb(DataOptions<HouseholdDb> options) : base(options.Options) { }
        public ITable<Mapping.PaymentRecord> Payments => GetTable<Mapping.PaymentRecord>();
        public ITable<Mapping.PaymentHistoryRecord> PaymentHistories => GetTable<Mapping.PaymentHistoryRecord>();
    }
}