namespace AllInSkateChallengeFunctions.Functions.LeaderBoard
{
    using System;
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
            var targetDistance = GetTargetDistance(target);
            var sql = @"WITH SkateLog_CTE ([ApplicationUserId], [TotalMiles])  
                        AS  
                        (  
                            SELECT [ApplicationUserId], SUM([DistanceInMiles]) AS TotalMiles
                            FROM [dbo].[SkateLogEntries]
                            GROUP BY [ApplicationUserId]
                        )  
                        SELECT anu.[Id], anu.[Email], anu.[SkaterName], anu.[ExternalProfileImage], slcte.[TotalMiles]
                        FROM [dbo].[AspNetUsers] anu
                        INNER JOIN [SkateLog_CTE] slcte ON anu.[Id] = slcte.[ApplicationUserId]
                        WHERE anu.[HasPaid] = 1 AND (anu.[Target] = @target OR slcte.TotalMiles <= @targetDistance)
                        ORDER BY slcte.TotalMiles DESC";

            var position = 1;
            var leaderBoardEntries = new List<LeaderBoardEntry>();
            var connectionString = configuration.GetConnectionString("AllInDbConnection");
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("target", (int)target);
                    command.Parameters.AddWithValue("targetDistance", targetDistance);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var email = reader.GetString(1);
                            var name = reader[2] != DBNull.Value ? reader.GetString(2) : null;
                            var externalProfileImage = reader[3] != DBNull.Value ? reader.GetString(3) : null;
                            var totalMiles = reader.GetDecimal(4);
                            var profileImage = GetProfileImage(email, externalProfileImage);

                            leaderBoardEntries.Add(new LeaderBoardEntry { Position = position++, ProfileImage = profileImage, SkaterName = name, TotalMiles = totalMiles });
                        }
                    }
                }
            }

            return leaderBoardEntries;
        }

        private string GetProfileImage(string emailAddress, string profileImage)
        {
            return string.IsNullOrWhiteSpace(profileImage) ? gravatarResolver.GetGravatarUrl(emailAddress) : profileImage;
        }

        private decimal GetTargetDistance(SkateTarget skateTarget)
        {
            switch (skateTarget)
            {
                case SkateTarget.AireValleyMarina:
                    return 2.1M;
                case SkateTarget.Saltaire:
                    return 13M;
                case SkateTarget.BingleyFiveRiseLocks:
                    return 16M;
                case SkateTarget.SkiptonCastle:
                    return 30M;
                case SkateTarget.EastMartonDoubleArchedBridge:
                    return 38M;
                case SkateTarget.FoulridgeSummit:
                    return 47M;
                case SkateTarget.Burnley:
                    return 57M;
                case SkateTarget.HalfwayThere:
                    return 63M;
                case SkateTarget.BlackburnFlight:
                    return 72M;
                case SkateTarget.WiganPier:
                    return 93M;
                case SkateTarget.TheScotchPiperInn:
                    return 113M;
                case SkateTarget.ThereAndBackAgain:
                    return 255M;
                default:
                    return 127.5M;
            }
        }
    }
}