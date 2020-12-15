using AllInSkateChallengeFunctions;

using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(StartUp))]

namespace AllInSkateChallengeFunctions
{
    using AllInSkateChallengeFunctions.Functions.LeaderBoard;
    using AllInSkateChallengeFunctions.Functions.SkaterLogs;
    using AllInSkateChallengeFunctions.Gravatar;

    using Microsoft.Azure.Functions.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection;

    public class StartUp : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IGravatarResolver, GravatarResolver>();
            builder.Services.AddSingleton<ILeaderBoardRepository, LeaderBoardRepository>();
            builder.Services.AddSingleton<ISkaterLogRepository, SkaterLogRepository>();
        }
    }
}
