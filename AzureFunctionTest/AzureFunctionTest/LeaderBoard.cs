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

    public class LeaderBoard
    {
        private readonly IGravatarResolver gravatarResolver;

        private readonly IConfiguration configuration;

        public LeaderBoard(IGravatarResolver gravatarResolver, IConfiguration configuration)
        {
            this.gravatarResolver = gravatarResolver;
            this.configuration = configuration;
        }

        [FunctionName("LeaderBoard")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "LeaderBoard/List/{target}")] HttpRequest req,
            string target,
            ILogger log)
        {
            var skateTarget = GetSkateTarget(target);
            var sql = @"WITH SkateLog_CTE ([ApplicationUserId], [TotalMiles])  
                        AS  
                        (  
                            SELECT [ApplicationUserId], SUM([DistanceInMiles]) AS TotalMiles
                            FROM [dbo].[SkateLogEntries]
                            GROUP BY [ApplicationUserId]
                        )  
                        SELECT anu.[Id], anu.[Email], anu.[SkaterName], slcte.[TotalMiles]
                        FROM [dbo].[AspNetUsers] anu
                        INNER JOIN [SkateLog_CTE] slcte ON anu.[Id] = slcte.[ApplicationUserId]
                        WHERE anu.[HasPaid] = 1 AND anu.[Target] = @target
                        ORDER BY slcte.TotalMiles DESC";

            var leaderBoardEntry = new List<LeaderBoardEntry>();
            var connectionString = configuration.GetConnectionString("AllInDbConnection");
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("target", (int)skateTarget);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            leaderBoardEntry.Add(
                                new LeaderBoardEntry
                                    {
                                        Gravatar = gravatarResolver.GetGravatarUrl(reader.GetString(1)),
                                        SkaterName = reader.GetString(2),
                                        TotalMiles = reader.GetDecimal(3)
                                    });
                        }
                    }
                }
            }

            return new OkObjectResult(leaderBoardEntry);
        }

        private static SkateTarget GetSkateTarget(string target)
        {
            if (!Enum.TryParse<SkateTarget>(target, out var skateTarget))
            {
                return SkateTarget.LiverpoolCanningDock;
            }

            return skateTarget;
        }

        private class LeaderBoardEntry
        {
            public string Gravatar { get; set; }

            public string SkaterName { get; set; }

            public decimal TotalMiles { get; set; }
        }
    }
}
