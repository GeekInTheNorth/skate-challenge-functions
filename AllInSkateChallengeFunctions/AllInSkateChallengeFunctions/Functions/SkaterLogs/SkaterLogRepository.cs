namespace AzureFunctionTest.Functions.SkaterLogs
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Threading.Tasks;

    using AzureFunctionTest.Gravatar;

    using Microsoft.Extensions.Configuration;

    public class SkaterLogRepository : ISkaterLogRepository
    {
        private readonly IGravatarResolver gravatarResolver;

        private readonly IConfiguration configuration;

        public SkaterLogRepository(IGravatarResolver gravatarResolver, IConfiguration configuration)
        {
            this.gravatarResolver = gravatarResolver;
            this.configuration = configuration;
        }

        public async Task<IList<SkaterLogEntry>> Get(int skip, int take)
        {
            var sql = @"SELECT anu.[Id], anu.[Email], anu.[SkaterName], sle.[Logged], sle.[DistanceInMiles], sle.[Name]
                        FROM [dbo].[AspNetUsers] anu
                        INNER JOIN [SkateLogEntries] sle ON anu.[Id] = sle.[ApplicationUserId]
                        WHERE anu.[HasPaid] = 1
                        ORDER BY sle.[Logged] DESC
                        OFFSET {0} ROWS
                        FETCH NEXT {1} ROWS ONLY;";
            sql = string.Format(sql, skip, take);

            var skaterLogEntries = new List<SkaterLogEntry>();
            var connectionString = configuration.GetConnectionString("AllInDbConnection");
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(sql, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            skaterLogEntries.Add(
                                new SkaterLogEntry
                                    {
                                        Gravatar = gravatarResolver.GetGravatarUrl(reader.GetString(1)),
                                        SkaterName = reader[2] != DBNull.Value ? reader.GetString(2) : "Private Skater",
                                        Logged = reader.GetDateTime(3),
                                        Miles = reader.GetDecimal(4),
                                        ActivityName = reader[5] != DBNull.Value ? reader.GetString(5) : null
                                    });
                        }
                    }
                }
            }

            return skaterLogEntries;
        }
    }
}