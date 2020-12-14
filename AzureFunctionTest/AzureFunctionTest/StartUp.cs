using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(AzureFunctionTest.StartUp))]

namespace AzureFunctionTest
{
    using Microsoft.Extensions.Configuration;

    public class StartUp : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IGravatarResolver, GravatarResolver>();
        }
    }
}
