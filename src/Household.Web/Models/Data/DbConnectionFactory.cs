using LinqToDB;
using LinqToDB.Data;
using Microsoft.Data.SqlClient;

namespace Household.Web.Models.Data
{
    public static class DbConnectionFactory
    {
        public static DataOptions<HouseholdDb> CreateOptions(string connectionString)
        {
            var conn = new SqlConnection(connectionString);
            return new DataOptions<HouseholdDb>()
                .UseConnectionFactory(LinqToDB.ProviderName.SqlServer, () => conn);
        }
    }
}