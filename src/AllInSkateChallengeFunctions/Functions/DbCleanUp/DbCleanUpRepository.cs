using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

namespace AllInSkateChallengeFunctions.Functions.DbCleanUp
{
    public class DbCleanUpRepository : IDbCleanUpRepository
    {
        private readonly IConfiguration configuration;

        public DbCleanUpRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task CleanStravaEvents()
        {
            var sql = @"DELETE FROM [dbo].[StravaEvents] WHERE Imported = 1";

            var connectionString = configuration.GetConnectionString("AllInDbConnection");
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(sql, connection))
                {
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task CleanStravaIntegrationLogs()
        {
            var sql = @"DELETE FROM [dbo].[StravaIntegrationLogs] Where Recieved <= @RecievedDate";

            var connectionString = configuration.GetConnectionString("AllInDbConnection");
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@RecievedDate", DateTime.Today.AddDays(-30));
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
