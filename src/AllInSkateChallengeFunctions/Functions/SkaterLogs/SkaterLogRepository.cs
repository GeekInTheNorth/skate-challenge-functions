namespace AllInSkateChallengeFunctions.Functions.SkaterLogs
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Threading.Tasks;

    using AllInSkateChallengeFunctions.Gravatar;

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

        public async Task<SkaterLogResult> Get(int skip, int take)
        {
            var total = await GetTotal();
            var logs = await GetLogs(skip, take);
            var remaining = total - (skip + take);
            remaining = remaining < 0 ? 0 : remaining;

            return new SkaterLogResult { Total = total, Log = logs, Remaining = remaining };
        }

        private string GetProfileImage(string emailAddress, string profileImage)
        {
            return string.IsNullOrWhiteSpace(profileImage) ? gravatarResolver.GetGravatarUrl(emailAddress) : profileImage;
        }

        private async Task<int> GetTotal()
        {
            var sql = "SELECT COUNT(*) FROM [dbo].[SkateLogEntries]";

            var connectionString = configuration.GetConnectionString("AllInDbConnection");
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(sql, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            return reader.GetInt32(0);
                        }
                    }
                }
            }

            return 0;
        }

        private async Task<List<SkaterLogEntry>> GetLogs(int skip, int take)
        {
            var sql = @"SELECT anu.[Id], anu.[Email], anu.[SkaterName], anu.[ExternalProfileImage], sle.[Logged], sle.[DistanceInMiles], sle.[Name]
                        FROM [dbo].[AspNetUsers] anu
                        INNER JOIN [dbo].[SkateLogEntries] sle ON anu.[Id] = sle.[ApplicationUserId]
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
                            var email = reader.GetString(1);
                            var name = reader[2] != DBNull.Value ? reader.GetString(2) : null;
                            var externalProfileImage = reader[3] != DBNull.Value ? reader.GetString(3) : null;
                            var dateLogged = reader.GetDateTime(4);
                            var miles = reader.GetDecimal(5);
                            var activityName = reader[6] != DBNull.Value ? reader.GetString(6) : null;
                            var profileImage = GetProfileImage(email, externalProfileImage);

                            skaterLogEntries.Add(new SkaterLogEntry { ProfileImage = profileImage, SkaterName = name, Logged = dateLogged, Miles = miles, ActivityName = activityName });
                        }
                    }
                }
            }

            return skaterLogEntries;
        }
    }
}