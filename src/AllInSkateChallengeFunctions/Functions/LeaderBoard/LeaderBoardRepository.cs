namespace AllInSkateChallengeFunctions.Functions.LeaderBoard
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Threading.Tasks;

    using AllInSkateChallenge.Features.Common;

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
            var targetKilometres = GetTargetDistance(target);
            var targetMiles = Conversion.KilometresToMiles(targetKilometres);
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
                    command.Parameters.AddWithValue("targetDistance", targetMiles);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var email = reader.GetString(1);
                            var name = reader[2] != DBNull.Value ? reader.GetString(2) : null;
                            var externalProfileImage = reader[3] != DBNull.Value ? reader.GetString(3) : null;
                            var totalMiles = reader.GetDecimal(4);

                            leaderBoardEntries.Add(
                                new LeaderBoardEntry 
                                { 
                                    Position = position++, 
                                    ProfileImage = GetProfileImage(email, externalProfileImage), 
                                    SkaterName = GetDisplayName(name), 
                                    TotalMiles = totalMiles 
                                });
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

        private string GetDisplayName(string skaterName)
        {
            return skaterName?.Replace("_", " ");
        }

        private decimal GetTargetDistance(SkateTarget skateTarget)
        {
            switch (skateTarget)
            {
                case SkateTarget.CornExchange:
                    return 0;
                case SkateTarget.SoveriegnSquare:
                    return 0.9M;
                case SkateTarget.GranaryWharf:
                    return 1.3M;
                case SkateTarget.TetleyBreweryWharf:
                    return 2.3M;
                case SkateTarget.LeedsIndustrialMuseum:
                    return 6;
                case SkateTarget.ArmleyPark:
                    return 7.8M;
                case SkateTarget.EllandRoad:
                    return 10.8M;
                case SkateTarget.MiddletonRailway:
                    return 13.6M;
                case SkateTarget.Carlton:
                    return 19.9M;
                case SkateTarget.TempleNewsamPark:
                    return 29.3M;
                case SkateTarget.LsTen:
                    return 35.5M;
                case SkateTarget.RoyalArmouriesMuseum:
                    return 36.7M;
                case SkateTarget.KirkgateMarket:
                    return 37.9M;
                case SkateTarget.LeedsGrandTheatre:
                    return 38.2M;
                case SkateTarget.MilleniumSquare:
                    return 39;
                case SkateTarget.RamgarhiaSikhSportsCentre:
                    return 40.5M;
                case SkateTarget.PotternewtonPark:
                    return 42.2M;
                case SkateTarget.MeanwoodValleyUrbanFarm:
                    return 45;
                case SkateTarget.YorkshireCricketGround:
                    return 47.4M;
                case SkateTarget.KirkstallAbbey:
                    return 49.6M;
                case SkateTarget.SunnyBankMillsGallery:
                    return 55.5M;
                case SkateTarget.BrownleeCentre:
                    return 63.6M;
                case SkateTarget.GoldenAcrePark:
                    return 67.4M;
                case SkateTarget.EccupReservoir:
                    return 71.5M;
                case SkateTarget.EmmerdaleTheTour:
                    return 77.1M;
                case SkateTarget.HarewoodHouseTrust:
                    return 82.2M;
                case SkateTarget.OtleyChevinForestPark:
                    return 93.4M;
                case SkateTarget.YeadonTarn:
                    return 97.3M;
                case SkateTarget.LeedsBradfordAirport:
                    return 100;
                case SkateTarget.ThereAndBackAgain:
                    return 200;
                default:
                    return 100;
            }
        }
    }
}