namespace AllInSkateChallengeFunctions.Functions.Events
{
    using System.Data.SqlClient;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Configuration;

    public class EventStatisticRepository : IEventStatisticRepository
    {
        private readonly IConfiguration configuration;

        public EventStatisticRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task Update()
        {
            var sql = @"TRUNCATE TABLE [dbo].[EventStatistics];

                        WITH SkateLog_CTE ([ApplicationUserId], [SkaterMiles]) AS
                        (  
                            SELECT [ApplicationUserId], SUM([DistanceInMiles]) AS SkaterMiles
                            FROM [dbo].[SkateLogEntries] ske
                            INNER JOIN [dbo].[AspNetUsers] apu ON apu.[Id] = ske.[ApplicationUserId]
                            WHERE apu.[HasPaid] = 1
                            GROUP BY [ApplicationUserId]
                        )
                        INSERT INTO [dbo].[EventStatistics]
                           ([EventStatisticId], [NumberOfSkaters], [CumulativeMiles])
                        SELECT NEWID(), COUNT(1) AS Skaters, SUM([SkaterMiles]) As TotalMiles
                        FROM SkateLog_CTE;";

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
    }
}