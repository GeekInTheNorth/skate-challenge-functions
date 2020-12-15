namespace AzureFunctionTest
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    public class SkaterUpdates
    {
        private readonly IGravatarResolver gravatarResolver;

        private readonly IConfiguration configuration;

        public SkaterUpdates(IGravatarResolver gravatarResolver, IConfiguration configuration)
        {
            this.gravatarResolver = gravatarResolver;
            this.configuration = configuration;
        }

        [FunctionName("SkaterUpdates")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "SkaterUpdates/List/")]
            HttpRequest req,
            ILogger log)
        {
            var take = GetTakeValue(req);
            var skip = GetSkipValue(req);
            var sql = @"SELECT anu.[Id], anu.[Email], anu.[SkaterName], sle.[Logged], sle.[DistanceInMiles], sle.[Name]
                        FROM [dbo].[AspNetUsers] anu
                        INNER JOIN [SkateLogEntries] sle ON anu.[Id] = sle.[ApplicationUserId]
                        WHERE anu.[HasPaid] = 1
                        ORDER BY sle.[Logged] DESC
                        OFFSET {0} ROWS
                        FETCH NEXT {1} ROWS ONLY;";
            sql = string.Format(sql, skip, take);

            var skateLogEntries = new List<SkateLogEntry>();
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
                            skateLogEntries.Add(
                                new SkateLogEntry
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

            return new OkObjectResult(skateLogEntries);
        }

        private int GetTakeValue(HttpRequest req)
        {
            var takeValue = 15;
            if (int.TryParse(req.Query["take"], out var parsedValue))
            {
                takeValue = parsedValue;
            }

            return takeValue;
        }

        private int GetSkipValue(HttpRequest req)
        {
            var skipValue = 0;
            if (int.TryParse(req.Query["skip"], out var parsedValue))
            {
                skipValue = parsedValue;
            }

            return skipValue;
        }

        private class SkateLogEntry
        {
            public string Gravatar { get; set; }

            public string SkaterName { get; set; }

            public DateTime Logged { get; set; }

            public decimal Miles { get; set; }

            public string ActivityName { get; set; }
        }
    }
}
