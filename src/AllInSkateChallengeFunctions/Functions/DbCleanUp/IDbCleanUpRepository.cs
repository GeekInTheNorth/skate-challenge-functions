using System.Threading.Tasks;

namespace AllInSkateChallengeFunctions.Functions.DbCleanUp
{
    public interface IDbCleanUpRepository
    {
        Task CleanStravaEvents();

        Task CleanStravaIntegrationLogs();
    }
}
