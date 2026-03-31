using LinqToDB;
using LinqToDB.Data;

namespace Household.Web.Models.Data
{
    public static class DbConnectionFactory
    {
        public static DataOptions<HouseholdDb> CreateOptions(string connectionString)
        {
            return new DataOptions<HouseholdDb>(new DataOptions().UseSqlServer(connectionString));
        }
    }
}