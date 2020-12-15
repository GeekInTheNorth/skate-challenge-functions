namespace AllInSkateChallengeFunctions.Functions.LeaderBoard
{
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Threading.Tasks;

    using AllInSkateChallengeFunctions.Gravatar;

    using Microsoft.Extensions.Configuration;

    public class LeaderBoardRepository : ILeaderBoardRepository
    {
        private readonly IGravatarResolver gravatarResolver;

        private readonly IConfiguration configuration;

        public LeaderBoardRepository(IGravatarResolver gravatarResolver, IConfiguration configuration)
        {
            this.gravatarResolver = gravatarResolver;
            this.configuration = configuration;
        }

        public async Task<IList<LeaderBoardEntry>> Get(SkateTarget target)
        {
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

            var leaderBoardEntries = new List<LeaderBoardEntry>();
            var connectionString = configuration.GetConnectionString("AllInDbConnection");
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("target", (int)target);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            leaderBoardEntries.Add(
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

            return leaderBoardEntries;
        }
    }
}