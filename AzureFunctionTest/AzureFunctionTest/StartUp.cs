using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(AzureFunctionTest.StartUp))]

namespace AzureFunctionTest
{
    using AzureFunctionTest.Functions.LeaderBoard;
    using AzureFunctionTest.Functions.SkaterLogs;
    using AzureFunctionTest.Gravatar;

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
